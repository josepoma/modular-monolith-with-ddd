namespace CompanyName.MyMeetings.API.Configuration.Caching
{
    public class CacheOptions
    {
        public const string SectionName = "Cache";

        public string Provider { get; set; } = CacheProvider.InMemory;

        public RedisCacheOptions Redis { get; set; } = new RedisCacheOptions();
    }

    public static class CacheProvider
    {
        public const string InMemory = "InMemory";

        public const string Redis = "Redis";
    }

    public class RedisCacheOptions
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; } = "MyMeetings:";

        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(Configuration);
        }
    }
}
