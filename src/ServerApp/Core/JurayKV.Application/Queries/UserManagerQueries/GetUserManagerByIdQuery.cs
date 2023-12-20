using System.Linq.Expressions;
using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Queries.WalletQueries;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.Aggregates.WalletAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace JurayKV.Application.Queries.UserManagerQueries;

public sealed class GetUserManagerByIdQuery : IRequest<UserManagerDetailsDto>
{
    public GetUserManagerByIdQuery(Guid id)
    {
        Id = id.ThrowIfEmpty(nameof(id));
    }

    public Guid Id { get; }

    // Handler
    private class GetUserManagerByIdQueryHandler : IRequestHandler<GetUserManagerByIdQuery, UserManagerDetailsDto>
    {
        private readonly IUserManagerCacheRepository _userManager;
        private readonly IQueryRepository _repository;
        private readonly IMediator _mediator;
 


        public GetUserManagerByIdQueryHandler(IUserManagerCacheRepository userManager, IQueryRepository repository, IMediator mediator)
        {
            _userManager = userManager;
            _repository = repository;
            _mediator = mediator;
         }

        public async Task<UserManagerDetailsDto> Handle(GetUserManagerByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            var user = await _userManager.GetByIdAsync(request.Id);
            var reff = await _userManager.GetReferralInfoByPhoneAsync(user.RefferedBy);
            UserManagerDetailsDto outcome = new UserManagerDetailsDto
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreationUTC = user.CreationUTC, // Replace with the actual property in ApplicationUser
                IsDisabled = user.IsDisabled,
                LastLoggedInAtUtc = user.LastLoggedInAtUtc,
                Surname = user.Surname,
                Firstname = user.Firstname, Lastname = user.Lastname,
                IsCompany = user.IsCompany,
                RefferedBy = reff.Fullname,
                PhoneOfRefferedBy = user.RefferedBy
            };

            

            return outcome;
        }
    }
}

