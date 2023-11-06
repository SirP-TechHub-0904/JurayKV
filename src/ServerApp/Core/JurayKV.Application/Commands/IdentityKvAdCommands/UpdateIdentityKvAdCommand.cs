using JurayKV.Application.Caching.Handlers;
using JurayKV.Application.Infrastructures;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.Exceptions;
using JurayKV.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.IdentityKvAdCommands;

public sealed class UpdateIdentityKvAdCommand : IRequest
{
    public UpdateIdentityKvAdCommand(
        Guid id,
        IFormFile videoFile)
    {
        Id = id;
        VideoFile = videoFile;
    }

    public Guid Id { get; }

    public IFormFile VideoFile { get; set; }
}

internal class UpdateIdentityKvAdCommandHandler : IRequestHandler<UpdateIdentityKvAdCommand>
{
    private readonly IIdentityKvAdRepository _identityKvAdRepository;
    private readonly IIdentityKvAdCacheHandler _identityKvAdCacheHandler;
    private readonly IStorageService _storage;

    public UpdateIdentityKvAdCommandHandler(
        IIdentityKvAdRepository identityKvAdRepository,
        IIdentityKvAdCacheHandler identityKvAdCacheHandler,
        IStorageService storage)
    {
        _identityKvAdRepository = identityKvAdRepository;
        _identityKvAdCacheHandler = identityKvAdCacheHandler;
        _storage = storage;
    }

    public async Task Handle(UpdateIdentityKvAdCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        IdentityKvAd identityKvAdToBeUpdated = await _identityKvAdRepository.GetByIdAsync(request.Id);

        if (identityKvAdToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(IdentityKvAd), request.Id);
        }

        //run the video upload;
        var videourl = "url";
        var videokey = "key";
        try
        {

            var xresult = await _storage.MainUploadFileReturnUrlAsync(identityKvAdToBeUpdated.VideoKey, request.VideoFile);
            // 
            if (xresult.Message.Contains("200"))
            {
                videourl = xresult.Url;
                videokey = xresult.Key;
            }

        }
        catch (Exception c)
        {

        }
        identityKvAdToBeUpdated.VideoUrl = videourl;
        identityKvAdToBeUpdated.VideoKey = videokey;

        identityKvAdToBeUpdated.LastModifiedAtUtc = DateTime.UtcNow;

        await _identityKvAdRepository.UpdateAsync(identityKvAdToBeUpdated);

         // Remove the cache
        await _identityKvAdCacheHandler.RemoveListAsync();
        await _identityKvAdCacheHandler.RemoveGetByUserIdAsync(identityKvAdToBeUpdated.UserId);
        await _identityKvAdCacheHandler.RemoveGetActiveByUserIdAsync(identityKvAdToBeUpdated.UserId);
        await _identityKvAdCacheHandler.RemoveDetailsByIdAsync(identityKvAdToBeUpdated.Id);
        await _identityKvAdCacheHandler.RemoveGetAsync(identityKvAdToBeUpdated.Id);
    }
}
