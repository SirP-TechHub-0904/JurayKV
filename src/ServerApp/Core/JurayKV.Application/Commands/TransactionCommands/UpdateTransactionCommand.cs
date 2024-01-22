using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.TransactionAggregate;
using JurayKV.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Commands.TransactionCommands;

public sealed class UpdateTransactionCommand : IRequest
{
    public UpdateTransactionCommand(Guid id, Guid walletId,
        Guid userId,
        string uniqueReference,
        string optionalNote,
        decimal amount,
        TransactionTypeEnum transactionType,
        EntityStatus status,
        string transactionReference,
        string description,
        string trackCode)
    {
        Id = id;
        WalletId = walletId;
        UserId = userId;
        UniqueReference = uniqueReference;
        OptionalNote = optionalNote;
        Amount = amount;
        TransactionType = transactionType;
        Status = status;
        TransactionReference = transactionReference;
        Description = description;
        TrackCode = trackCode;


    }
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public Guid UserId { get; set; }
    public string UniqueReference { get; set; }
    public string OptionalNote { get; set; }
    public decimal Amount { get; set; }

    public TransactionTypeEnum TransactionType { get; set; }
    public EntityStatus Status { get; set; }
    public string TransactionReference { get; set; }
    public string Description { get; set; }
    public string TrackCode { get; set; }
}

internal class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionCacheHandler _transactionCacheHandler;

    public UpdateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ITransactionCacheHandler transactionCacheHandler)
    {
        _transactionRepository = transactionRepository;
        _transactionCacheHandler = transactionCacheHandler;
    }

    public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Transaction transactionToBeUpdated = await _transactionRepository.GetByIdAsync(request.Id);

        if (transactionToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(Transaction), request.Id);
        }
        transactionToBeUpdated.WalletId = request.WalletId;
        transactionToBeUpdated.TransactionType = request.TransactionType;
        transactionToBeUpdated.TransactionReference = request.TransactionReference;
        transactionToBeUpdated.Description = request.Description;
        transactionToBeUpdated.Status = request.Status;
        transactionToBeUpdated.UserId = request.UserId;
        transactionToBeUpdated.Amount = request.Amount;
        transactionToBeUpdated.UniqueReference = request.UniqueReference;
        transactionToBeUpdated.OptionalNote = request.OptionalNote;

        await _transactionRepository.UpdateAsync(transactionToBeUpdated);

        // Remove the cache
        await _transactionCacheHandler.RemoveListAsync();
        await _transactionCacheHandler.RemoveGetAsync(transactionToBeUpdated.Id);
        await _transactionCacheHandler.RemoveDetailsByIdAsync(transactionToBeUpdated.Id);
        await _transactionCacheHandler.RemoveList10ByUserAsync(transactionToBeUpdated.UserId);

    }
}
