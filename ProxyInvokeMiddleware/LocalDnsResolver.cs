using System.Net;

namespace ProxyInvokeMiddleware
{
    public class LocalDnsResolver : IDnsResolver
    {
        public IPAddress[] GetHostAddresses(string host)
        {
            return Dns.GetHostAddresses(host);
        }

        public async ValueTask<IPAddress[]> GetHostAddressesAsync(string host)
        {
            return await Dns.GetHostAddressesAsync(host);
        }

        public async ValueTask<IPAddress[]> GetHostAddressesAsync(string host, CancellationToken cancellationToken)
        {
            //测试一下
            return await Dns.GetHostAddressesAsync(host, cancellationToken);
        }
    }
}
