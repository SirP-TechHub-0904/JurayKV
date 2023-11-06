﻿using JurayKV.Application.Caching.Handlers;
using JurayKV.Persistence.Cache.Handlers;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Persistence.Cache;

public static class ServiceCollectionExtensions
{
    public static void AddCaching(this IServiceCollection services)
    {
        services.ThrowIfNull(nameof(services));

        services.AddDistributedMemoryCache();

        services.AddScoped<IEmployeeCacheHandler, EmployeeCacheHandler>();
        services.AddScoped<IDepartmentCacheHandler, DepartmentCacheHandler>();
        services.AddScoped<IBucketCacheHandler, BucketCacheHandler>();
        services.AddScoped<ICompanyCacheHandler, CompanyCacheHandler>();
        services.AddScoped<IExchangeRateCacheHandler, ExchangeRateCacheHandler>();
        services.AddScoped<IIdentityActivityCacheHandler, IdentityActivityCacheHandler>();
        services.AddScoped<IIdentityKvAdCacheHandler, IdentityKvAdCacheHandler>();
        services.AddScoped<IUserManagerCacheHandler, UserManagerCacheHandler>();

        services.AddScoped<IKvAdCacheHandler, KvAdCacheHandler>();
        services.AddScoped<IKvPointCacheHandler, KvPointCacheHandler>();
        services.AddScoped<ITransactionCacheHandler, TransactionCacheHandler>();
        services.AddScoped<IWalletCacheHandler, WalletCacheHandler>();
        services.AddTransient<INotificationCacheHandler, NotificationCacheHandler>();
        services.AddTransient<IDashboardCacheHandler, DashboardCacheHandler>();
        services.AddTransient<ISliderCacheHandler, SliderCacheHandler>();
    }
}
