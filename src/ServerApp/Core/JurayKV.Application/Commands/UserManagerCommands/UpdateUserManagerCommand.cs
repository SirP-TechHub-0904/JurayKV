using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JurayKV.Application.Commands.UserManagerCommands
{
    public sealed class UpdateUserManagerCommand : IRequest
    {
        public UpdateUserManagerCommand(Guid id, UpdateUserDto data)
        {
            Id = id;
            Data = data;
        }
        public UpdateUserDto Data { get; set; }
        public Guid Id { get; set; }
    }

    internal class UpdateUserManagerCommandHandler : IRequestHandler<UpdateUserManagerCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManagerCacheHandler _userManagerCacheHandler;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UpdateUserManagerCommandHandler(
UserManager<ApplicationUser> userManager, IUserManagerCacheHandler userManagerCacheHandler, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _userManagerCacheHandler = userManagerCacheHandler;
            _signInManager = signInManager;
        }

        public async Task Handle(UpdateUserManagerCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            user.SurName = request.Data.SurName;
            user.FirstName = request.Data.FirstName;
            user.LastName = request.Data.LastName;
            user.Email = request.Data.Email;
            user.PhoneNumber = request.Data.PhoneNumber;
            user.AccountStatus = request.Data.AccountStatus;
            user.DisableEmailNotification = request.Data.DisableEmailNotification;
            user.Tier = request.Data.Tier;
            user.Tie2Request = request.Data.Tie2Request;

            user.DateUpgraded = request.Data.DateUpgraded;
            await _userManager.UpdateAsync(user);
            //
            //await _signInManager.sign;
            //remove catch
            await _userManagerCacheHandler.RemoveListAsync();
            await _userManagerCacheHandler.RemoveDetailsByIdAsync(user.Id);
        }
    }
}
