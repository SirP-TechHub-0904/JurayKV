using JurayKV.Application.Caching.Handlers;
using JurayKV.Application.Infrastructures;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.UserManagerCommands
{
      public sealed class CsaUpdateUserManagerCommand : IRequest<ResponseCsaUpgrade>
    {
        public CsaUpdateUserManagerCommand(Guid id, UserUpgradeDto data, IFormFile? iDcard, IFormFile? passport)
        {
            Id = id;
            Data = data;
            IDcard = iDcard;
            Passport = passport;
        }
        public UserUpgradeDto Data { get; set; }
        public Guid Id { get; set; }
        public IFormFile? IDcard { get; set; }
        public IFormFile? Passport { get; set; }
    }

    internal class CsaUpdateUserManagerCommandHandler : IRequestHandler<CsaUpdateUserManagerCommand, ResponseCsaUpgrade>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManagerCacheHandler _userManagerCacheHandler;
        private readonly IStorageService _storage;

        public CsaUpdateUserManagerCommandHandler(
UserManager<ApplicationUser> userManager, IUserManagerCacheHandler userManagerCacheHandler, IStorageService storage)
        {
            _userManager = userManager;
            _userManagerCacheHandler = userManagerCacheHandler;
            _storage = storage;
        }

        public async Task<ResponseCsaUpgrade> Handle(CsaUpdateUserManagerCommand request, CancellationToken cancellationToken)
        {
            ResponseCsaUpgrade response = new ResponseCsaUpgrade();
            request.ThrowIfNull(nameof(request));
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            try
            {

                var xresult = await _storage.MainUploadFileReturnUrlAsync("", request.IDcard);
                // 
                if (xresult.Message.Contains("200"))
                {
                    user.IDCardUrl = xresult.Url;
                 user.IDCardKey = xresult.Key;
                }
                else
                {
                    response.Message += "<br>Unable to Upload ID Card";
                }

            }
            catch (Exception c)
            {
                response.Message += "<br>Unable to Upload ID Card";
            }
            try
            {

                var xresult = await _storage.MainUploadFileReturnUrlAsync("", request.Passport);
                // 
                if (xresult.Message.Contains("200"))
                {
                    user.PassportUrl = xresult.Url;
                    user.PassportKey = xresult.Key;
                }
                else
                {
                    response.Message += "<br>Unable to Upload passport";
                }

            }
            catch (Exception c)
            {
                response.Message += "<br>Unable to Upload passport";
            }
            user.DateUpgraded = DateTime.UtcNow.AddHours(1);
            user.About = request.Data.About;
            user.AlternativePhone = request.Data.AlternativePhone;
            user.Address = request.Data.Address;
            user.State = request.Data.State;
            user.LGA = request.Data.LGA;
            user.Occupation = request.Data.Occupation;
            user.FbHandle = request.Data.FbHandle;
            user.InstagramHandle = request.Data.InstagramHandle;
            user.TwitterHandle = request.Data.TwitterHandle;
            user.TiktokHandle = request.Data.TiktokHandle; 
           var result = await _userManager.UpdateAsync(user);

            //update role
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CSA");
                response.Message += "<br>Information Update.";
                response.Success = true;
            }
            else
            {
                response.Message += "<br>Unable to Update";
                response.Success = false;
            }
            //remove catch
            await _userManagerCacheHandler.RemoveListAsync();
            await _userManagerCacheHandler.RemoveDetailsByIdAsync(user.Id);

            return response;
        }
    }
}
