using System;
using System.Threading.Tasks;
using CompanyName.MyMeetings.API.Configuration.Authorization;
using CompanyName.MyMeetings.API.Configuration.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace CompanyName.MyMeetings.API
{
    [Route("api/cache")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private const string ProbeCacheKey = "cache:probe";

        private readonly IDistributedCache _distributedCache;

        private readonly CacheOptions _cacheOptions;

        public CacheController(IDistributedCache distributedCache, IOptions<CacheOptions> cacheOptions)
        {
            _distributedCache = distributedCache;
            _cacheOptions = cacheOptions.Value;
        }

        [HttpGet("probe")]
        [NoPermissionRequired]
        [ProducesResponseType(typeof(CacheProbeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Probe()
        {
            var cachedValue = await _distributedCache.GetStringAsync(ProbeCacheKey);
            var retrievedFromCache = cachedValue != null;

            var value = cachedValue ?? DateTime.UtcNow.ToString("O");

            if (!retrievedFromCache)
            {
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                };

                await _distributedCache.SetStringAsync(ProbeCacheKey, value, options);
            }

            var response = new CacheProbeResponse
            {
                Provider = _cacheOptions.Provider,
                RetrievedFromCache = retrievedFromCache,
                Value = value,
                Message = retrievedFromCache
                    ? "Value retrieved from configured cache provider."
                    : "New value stored using configured cache provider.",
            };

            return Ok(response);
        }
    }

    public class CacheProbeResponse
    {
        public string Provider { get; set; }

        public string Value { get; set; }

        public bool RetrievedFromCache { get; set; }

        public string Message { get; set; }
    }
}
