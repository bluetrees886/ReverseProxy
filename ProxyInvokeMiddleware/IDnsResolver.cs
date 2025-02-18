using System.Net;

namespace ProxyInvokeMiddleware
{
    public interface IDnsResolver
    {
        IPAddress[] GetHostAddresses(string host);
        ValueTask<IPAddress[]> GetHostAddressesAsync(string host);
        ValueTask<IPAddress[]> GetHostAddressesAsync(string host, CancellationToken cancellationToken);
    }
}
