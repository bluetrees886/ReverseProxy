
using CommonTypes;
using CommonTypes.Invoke;
using System.Net;
using System.Reflection.Metadata;

namespace ProxyInvokeMiddleware
{
    public class ProxyHttpClientFactory : IHttpClientFactory
    {
        private IProxyPolicy _policy;
        public HttpClient CreateClient(string name)
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false,
                UseProxy = true,
                PreAuthenticate = false,
                Proxy = new AddressProxy(_policy),
                EnableMultipleHttp2Connections = true,
            };
            return new HttpClient(handler, true);
        }
        public ProxyHttpClientFactory(IProxyPolicy policy)
        {
            _policy = policy;
        }
    }
}
