using PayPalCheckoutSdk.Core;
using PerAspera.Configurations;
using Umbraco.Cms.Core.Cache;
using HttpClient = PayPalHttp.HttpClient;

namespace PerAspera.Services.Implementation
{
    public class PayPalClientFactory
    {
        private const string CacheKey = "paypal-client";
        private readonly PayPalConfiguration _configuration;
        private readonly IAppPolicyCache _runtimeCache;

        public PayPalClientFactory(PayPalConfiguration configuration, IAppPolicyCache runtimeCache)
        {
            _configuration = configuration;
            _runtimeCache = runtimeCache;
        }

        public HttpClient Create()
            => _configuration.IsDevMode
                ? GetClient()
                : GetClientFromCache();

        private HttpClient GetClientFromCache()
        {
            var client = _runtimeCache.Get(CacheKey) as HttpClient;

            if (client is not null) return client;

            client = GetClient();
            AddClientToCache(client);

            return client;
        }

        private void AddClientToCache(HttpClient client)
        {
            _runtimeCache.Insert(CacheKey, () => client, TimeSpan.FromMinutes(2));
        }

        private HttpClient GetClient()
        {
            var clientId = _configuration.ClientId;
            var secret = _configuration.Secret;

            PayPalEnvironment environment = new SandboxEnvironment(clientId, secret);

            return new PayPalHttpClient(environment);
        }

    }
}
