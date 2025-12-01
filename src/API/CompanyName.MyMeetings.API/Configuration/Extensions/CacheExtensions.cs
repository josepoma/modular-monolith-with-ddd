using System;
using CompanyName.MyMeetings.API.Configuration.Caching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyName.MyMeetings.API.Configuration.Extensions
{
    internal static class CacheExtensions
    {
        internal static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheSection = configuration.GetSection(CacheOptions.SectionName);
            services.Configure<CacheOptions>(cacheSection);

            var cacheOptions = cacheSection.Get<CacheOptions>() ?? new CacheOptions();

            if (string.Equals(cacheOptions.Provider, CacheProvider.Redis, StringComparison.OrdinalIgnoreCase)
                && cacheOptions.Redis.IsConfigured())
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheOptions.Redis.Configuration;
                    options.InstanceName = cacheOptions.Redis.InstanceName;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            return services;
        }
    }
}
