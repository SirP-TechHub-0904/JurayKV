using JurayKV.Application.Caching.Handlers;
using JurayKV.Application.Infrastructures;
using JurayKV.Application.Services.AwsDtos;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.KvAdCommands;

public sealed class CreateKvAdCommand : IRequest<Guid>
{
    public CreateKvAdCommand(IFormFile file, Guid userId, Guid bucketId, Guid companyId)
    {
        ImageFile = file;
        UserId = userId;
        BucketId = bucketId;
        CompanyId = companyId;
    }

    public IFormFile ImageFile { get; set; }
    public Guid UserId { get; private set; }
    public Guid BucketId { get; private set; }
    public Guid CompanyId { get; private set; }
}

internal class CreateKvAdCommandHandler : IRequestHandler<CreateKvAdCommand, Guid>
{
    private readonly IKvAdRepository _kvAdRepository;
    private readonly IKvAdCacheHandler _kvAdCacheHandler;
    private readonly IStorageService _storage;
    public CreateKvAdCommandHandler(
            IKvAdRepository kvAdRepository,
            IKvAdCacheHandler kvAdCacheHandler,
            IStorageService storage)
    {
        _kvAdRepository = kvAdRepository;
        _kvAdCacheHandler = kvAdCacheHandler;
        _storage = storage;
    }

    public async Task<Guid> Handle(CreateKvAdCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));
        //run the video upload;
        var imageurl = "url";
        var imagekey = "key";
        try
        {
          
            var xresult = await _storage.MainUploadFileReturnUrlAsync("", request.ImageFile);
            // 
            if (xresult.Message.Contains("200"))
            {
                imageurl  = xresult.Url;
                imagekey = xresult.Key;
            }
             
        }
        catch (Exception c)
        {

        }
     
       KvAd create = new KvAd(Guid.NewGuid());
        create.BucketId = request.BucketId;
        create.UserId = request.UserId;
        create.CompanyId = request.CompanyId;
        create.ImageUrl = imageurl;
        create.ImageKey = imagekey;
        create.Status = Domain.Primitives.Enum.DataStatus.Active;
        // Persist to the database

        await _kvAdRepository.InsertAsync(create);

        // Remove the cache
        await _kvAdCacheHandler.RemoveListAsync();
        await _kvAdCacheHandler.RemoveDetailsByIdAsync(create.Id);
        await _kvAdCacheHandler.RemoveByBucketIdAsync(create.BucketId);
        await _kvAdCacheHandler.RemoveGetAsync(create.Id);
         

        return create.Id;
    }
}