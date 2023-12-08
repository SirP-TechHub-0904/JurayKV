using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Infrastructures;
using JurayKV.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender>();
            
            //services.AddInterswitch(configuration);
            return services;
        }
    }
}
