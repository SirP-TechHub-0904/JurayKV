using JurayKV.Application.Caching.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Queries.UserManagerQueries
{
     public sealed class GetUserByStatusListQuery : IRequest<List<UserManagerListDto>>
    {

        public GetUserByStatusListQuery(AccountStatus accountStatus)
        {
            AccountStatus = accountStatus;
        }

        public AccountStatus AccountStatus { get; }
        private class GetUserByStatusListQueryHandler : IRequestHandler<GetUserByStatusListQuery, List<UserManagerListDto>>
        {
            private readonly IUserManagerCacheRepository _userManager;

            public GetUserByStatusListQueryHandler(IUserManagerCacheRepository userManager)
            {
                _userManager = userManager;
            }

            public async Task<List<UserManagerListDto>> Handle(GetUserByStatusListQuery request, CancellationToken cancellationToken)
            {
                request.ThrowIfNull(nameof(request));
                List<UserManagerListDto> data = await _userManager.GetListByStatusAsync(request.AccountStatus);

                return data;
            }
        }
    }
}
