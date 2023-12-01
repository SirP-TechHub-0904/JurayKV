﻿using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Transactions;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace JurayKV.Application.Commands.IdentityKvAdCommands;

public sealed class CreateIdentityKvAdCommand : IRequest<Guid>
{
    public CreateIdentityKvAdCommand(IFormFile file, Guid userId, Guid kvAdId, DateTime date)
    {
        VideoFile = file;
        UserId = userId;
        KvAdId = kvAdId;
        Date = date;
    }

    public IFormFile VideoFile { get; set; }
    public Guid UserId { get; private set; }
    public Guid KvAdId { get; private set; }
    public DateTime Date { get; private set; }
}

internal class CreateIdentityKvAdCommandHandler : IRequestHandler<CreateIdentityKvAdCommand, Guid>
{
    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private static readonly object lockObject = new object();
    private readonly IIdentityKvAdRepository _departmentRepository;
    private readonly IIdentityKvAdCacheHandler _departmentCacheHandler;
    private readonly IRepository _repository;
    public CreateIdentityKvAdCommandHandler(
            IIdentityKvAdRepository departmentRepository,
            IIdentityKvAdCacheHandler departmentCacheHandler,
            IRepository repository)
    {
        _departmentRepository = departmentRepository;
        _departmentCacheHandler = departmentCacheHandler;
        _repository = repository;
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
        create.Active = true;
        create.KvAdHash = request.Date.ToString("ddMMyyyy");
       create.AdsStatus = Domain.Primitives.Enum.AdsStatus.Pending;
        await semaphoreSlim.WaitAsync();
        Guid result = Guid.Empty;
        try
        {
           result = await DoWorkAsync(create);
        }
        finally
        {
            semaphoreSlim.Release();
        }
        
        // Remove the cache
        await _departmentCacheHandler.RemoveListAsync();
        await _departmentCacheHandler.RemoveListActiveTodayAsync();
        await _departmentCacheHandler.RemoveGetByUserIdAsync(create.UserId);
        await _departmentCacheHandler.RemoveGetActiveByUserIdAsync(create.UserId);
        await _departmentCacheHandler.RemoveDetailsByIdAsync(create.Id);
        await _departmentCacheHandler.RemoveGetAsync(create.Id);

        return result;
    }
    static string GenerateUniqueIdForDay()
    {
        DateTime currentDateTime = DateTime.Now.AddHours(22);

        // Check if it's a new day
        if (currentDateTime.Hour < 6)
        {
            // If before 6 am, consider it as part of the previous day
            currentDateTime = currentDateTime.AddDays(-1);
        }

        // Set the time to 6 am
        DateTime adjustedDateTime = new DateTime(
            currentDateTime.Year,
            currentDateTime.Month,
            currentDateTime.Day,
            6,
            0,
            0
        );

        // Generate a unique ID based on the adjusted date and time
        string uniqueIdForDay = adjustedDateTime.ToString("");

        return uniqueIdForDay;
    }
    private async Task<Guid> DoWorkAsync(IdentityKvAd create)
    {
        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            },
            TransactionScopeAsyncFlowOption.Enabled))
        {
           Guid result = await _departmentRepository.InsertAsync(create);
            scope.Complete(); // Mark the transaction as complete

            return result;
        }
    }
}