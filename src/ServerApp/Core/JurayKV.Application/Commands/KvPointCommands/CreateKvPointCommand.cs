﻿using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.Aggregates.KvPointAggregate;
using JurayKV.Domain.Aggregates.WalletAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;
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
    private readonly IWalletRepository _walletRepository;
    private readonly IIdentityKvAdRepository _identityKvAdRepository;
    private readonly IKvPointCacheHandler _kvPointCacheHandler;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWalletCacheHandler _walletCacheHandler;
    private readonly IIdentityKvAdCacheHandler _identityKvAdCacheHandler;
    private readonly IRepository _repository;

    public CreateKvPointCommandHandler(
            IKvPointRepository kvPointRepository,
            IKvPointCacheHandler kvPointCacheHandler,
            IWalletRepository walletRepository,
            IHttpContextAccessor httpContextAccessor,
            IWalletCacheHandler walletCacheHandler,
            IIdentityKvAdRepository identityKvAdRepository,
            IIdentityKvAdCacheHandler identityKvAdCacheHandler,
            IRepository repository)
    {
        _kvPointRepository = kvPointRepository;
        _kvPointCacheHandler = kvPointCacheHandler;
        _walletRepository = walletRepository;
        _httpContextAccessor = httpContextAccessor;
        _walletCacheHandler = walletCacheHandler;
        _identityKvAdRepository = identityKvAdRepository;
        _identityKvAdCacheHandler = identityKvAdCacheHandler;
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateKvPointCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        var check = await _kvPointRepository.GetByIdentityIdByUserAsync(request.IdentityKvAdId, request.UserId);
        if (check == null)
        {
            IDbContextTransaction dbContextTransaction = await _repository
             .BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);
            try
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

                Guid resultId = await _kvPointRepository.InsertAsync(create);
                if (resultId != Guid.Empty)
                {
                    //update wallet
                    var getwallet = await _walletRepository.GetByUserIdAsync(request.UserId);
                    getwallet.Amount += Convert.ToDecimal(request.Point);
                    getwallet.LastUpdateAtUtc = DateTime.Now;
                    var loguserId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                    getwallet.Log = "<br> Wallet Update from User Advert id " + request.IdentityKvAdId + " ::Amount: " + request.Point + " ::Balance: " + getwallet.Amount + " :: Date: " + getwallet.LastUpdateAtUtc + ":: Loggedin User: " + loguserId;

                    await _walletRepository.UpdateAsync(getwallet);

                    await _walletCacheHandler.RemoveListAsync();

                    await _walletCacheHandler.RemoveDetailsByIdAsync(getwallet.Id);
                    await _walletCacheHandler.RemoveDetailsByUserIdAsync(getwallet.UserId);
                    await _walletCacheHandler.RemoveGetAsync(getwallet.Id);

                    //update creadited
                    var getidentitykvads = await _identityKvAdRepository.GetByIdAsync(request.IdentityKvAdId);
                    if (getidentitykvads != null)
                    {
                        getidentitykvads.AdsStatus = AdsStatus.Credited;
                        await _identityKvAdRepository.UpdateAsync(getidentitykvads);
                    }
                    // Remove the cache
                    await _identityKvAdCacheHandler.RemoveListAsync();
                    await _identityKvAdCacheHandler.RemoveListActiveTodayAsync();
                    await _identityKvAdCacheHandler.RemoveGetByUserIdAsync(create.UserId);
                    await _identityKvAdCacheHandler.RemoveGetActiveByUserIdAsync(create.UserId);
                    await _identityKvAdCacheHandler.RemoveDetailsByIdAsync(create.Id);
                    await _identityKvAdCacheHandler.RemoveGetAsync(create.Id);
                }


                // Remove the cache
                await _kvPointCacheHandler.RemoveListAsync();
                await _kvPointCacheHandler.RemoveDetailsByIdAsync(create.Id);
                await _kvPointCacheHandler.RemoveListBy10ByUserAsync(create.UserId);
                await _kvPointCacheHandler.RemoveGetAsync(create.Id);

                return create.Id;

                await dbContextTransaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            //check.Point = request.Point;
            check.LastModifiedAtUtc = DateTime.UtcNow;
            await _kvPointRepository.UpdateAsync(check);

            // Remove the cache
            await _kvPointCacheHandler.RemoveListAsync();
            await _kvPointCacheHandler.RemoveDetailsByIdAsync(check.Id);
            await _kvPointCacheHandler.RemoveListBy10ByUserAsync(check.UserId);
            await _kvPointCacheHandler.RemoveListByUserAsync(check.UserId);
            await _kvPointCacheHandler.RemoveGetAsync(check.Id);

            return check.Id;
        }
        //check



        //update







    }
}