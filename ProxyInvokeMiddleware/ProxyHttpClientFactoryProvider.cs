using CommonTypes;

namespace ProxyInvokeMiddleware
{
    public class ProxyHttpClientFactoryProvider
    {
        public IHttpClientFactory GetClientFactory(IProxyPolicy policy)
        {
            return new ProxyHttpClientFactory(policy);
        }
    }
}
