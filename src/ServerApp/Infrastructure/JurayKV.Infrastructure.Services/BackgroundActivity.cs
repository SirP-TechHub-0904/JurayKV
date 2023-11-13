using JurayKV.Application.Infrastructures;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Infrastructure.Services
{

    public sealed class BackgroundActivity : IBackgroundActivity
    {
        private readonly IConfiguration _configManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExceptionLogger _exceptionLogger;
        public BackgroundActivity(IExceptionLogger exceptionLogger, UserManager<ApplicationUser> userManager, IConfiguration configManager)
        {
            _exceptionLogger = exceptionLogger;
            _userManager = userManager;
            _configManager = configManager;
        }

        public Task SendEmailToActiveAdsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
