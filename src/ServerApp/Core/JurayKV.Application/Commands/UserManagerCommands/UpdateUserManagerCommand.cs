using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;

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

        public UpdateUserManagerCommandHandler(
UserManager<ApplicationUser> userManager, IUserManagerCacheHandler userManagerCacheHandler)
        {
            _userManager = userManager;
            _userManagerCacheHandler = userManagerCacheHandler;
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
            user.IsDisabled = request.Data.IsDisabled;
            user.DisableEmailNotification = request.Data.DisableEmailNotification;
            await _userManager.UpdateAsync(user);

            //remove catch
            await _userManagerCacheHandler.RemoveListAsync();
            await _userManagerCacheHandler.RemoveDetailsByIdAsync(user.Id);
        }
    }
}
