using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.KvPointAggregate;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TanvirArjel.ArgumentChecker;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Commands.KvPointCommands;

public sealed class CreateKvPointCommand : IRequest<Guid>
{
    public CreateKvPointCommand(Guid userId, Guid identityKvAdId, EntityStatus status, int point, string pointHash)
    {
        
        UserId = userId;
        IdentityKvAdId = identityKvAdId;
        Status = status;
        Point = point;
        PointHash = pointHash;
    }

    public Guid UserId { get; set; }
    public Guid IdentityKvAdId { get; set; }
    public EntityStatus Status { get; set; }

    public int Point { get; set; }
    public string PointHash { get; set; }
}

internal class CreateKvPointCommandHandler : IRequestHandler<CreateKvPointCommand, Guid>
{
    private readonly IKvPointRepository _kvPointRepository;
    private readonly IKvPointCacheHandler _kvPointCacheHandler;

    public CreateKvPointCommandHandler(
            IKvPointRepository kvPointRepository,
            IKvPointCacheHandler kvPointCacheHandler)
    {
        _kvPointRepository = kvPointRepository;
        _kvPointCacheHandler = kvPointCacheHandler;
    }

    public async Task<Guid> Handle(CreateKvPointCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        var check = await _kvPointRepository.GetByIdentityIdByUserAsync(request.IdentityKvAdId, request.UserId);
        if(check == null)
        {

            //if null, create
            KvPoint create = new KvPoint(Guid.NewGuid());
            create.UserId = request.UserId;
            create.IdentityKvAdId = request.IdentityKvAdId;
            create.Status = request.Status;
            create.Point = request.Point;
            create.PointHash = request.PointHash;
            create.LastModifiedAtUtc = DateTime.UtcNow;
            // Persist to the database

            await _kvPointRepository.InsertAsync(create);
            // Remove the cache
            await _kvPointCacheHandler.RemoveListAsync();
            await _kvPointCacheHandler.RemoveDetailsByIdAsync(create.Id);
            await _kvPointCacheHandler.RemoveListBy10ByUserAsync(create.UserId);
            await _kvPointCacheHandler.RemoveGetAsync(create.Id);

            return create.Id;
        }
        else
        {
            check.Point = request.Point;
            check.LastModifiedAtUtc = DateTime.UtcNow;
            await _kvPointRepository.UpdateAsync(check);

            // Remove the cache
            await _kvPointCacheHandler.RemoveListAsync();
            await _kvPointCacheHandler.RemoveDetailsByIdAsync(check.Id);
            await _kvPointCacheHandler.RemoveListBy10ByUserAsync(check.UserId);
            await _kvPointCacheHandler.RemoveGetAsync(check.Id);

            return check.Id;
        }
        //check



        //update




        

       
    }
}