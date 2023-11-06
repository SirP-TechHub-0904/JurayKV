using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.IdentityKvAdCommands;

public sealed class CreateIdentityKvAdCommand : IRequest<Guid>
{
    public CreateIdentityKvAdCommand(IFormFile file, Guid userId, Guid kvAdId)
    {
        VideoFile = file;
        UserId = userId;
        KvAdId = kvAdId;
    }

    public IFormFile VideoFile { get; set; }
    public Guid UserId { get; private set; }
    public Guid KvAdId { get; private set; }
}

internal class CreateIdentityKvAdCommandHandler : IRequestHandler<CreateIdentityKvAdCommand, Guid>
{
    private readonly IIdentityKvAdRepository _departmentRepository;
    private readonly IIdentityKvAdCacheHandler _departmentCacheHandler;

    public CreateIdentityKvAdCommandHandler(
            IIdentityKvAdRepository departmentRepository,
            IIdentityKvAdCacheHandler departmentCacheHandler)
    {
        _departmentRepository = departmentRepository;
        _departmentCacheHandler = departmentCacheHandler;
    }

    public async Task<Guid> Handle(CreateIdentityKvAdCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));
        //run the video upload;
        var videourl = String.Empty;
        var videokey = String.Empty;
        IdentityKvAd create = new IdentityKvAd(Guid.NewGuid());
        create.KvAdId = request.KvAdId;
        create.UserId = request.UserId;
        create.VideoUrl = videourl;
        create.VideoKey = videokey;

         // Persist to the database

         await _departmentRepository.InsertAsync(create);

        // Remove the cache
        await _departmentCacheHandler.RemoveListAsync();
        await _departmentCacheHandler.RemoveGetByUserIdAsync(create.UserId);
        await _departmentCacheHandler.RemoveGetActiveByUserIdAsync(create.UserId);
        await _departmentCacheHandler.RemoveDetailsByIdAsync(create.Id);
        await _departmentCacheHandler.RemoveGetAsync(create.Id);

        return create.Id;
    }
}