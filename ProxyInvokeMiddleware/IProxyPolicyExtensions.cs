using CommonTypes;
using CommonTypes.Invoke;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ProxyInvokeMiddleware
{
    public static class IProxyPolicyExtensions
    {
        private static ConcurrentDictionary<string, SocketsHttpHandler> _socketHandlers = new ConcurrentDictionary<string, SocketsHttpHandler>();
        public static HttpMessageInvoker CreateHttpInvoker(this IProxyPolicy proxyPolicy)
        {
            //handler 不能释放，对于同一个Proxy只需要一个handler，这里的设计诡异，主要是TCP协议的TIME_WAIT造成的，导致主动释放Socket并不能及时释放占用的端口
            //如果频繁的创建释放handler就导致客户端主动断开Socket，以至于大量的Socket链接处于TIME_WAIT，一般会占用4分钟即4MSL。
            //不主动释放就可以保持住Handler，而已经断开并且进入TIME_WAIT的链接，Handler并不会再使用，不需要担心Socket链接会混乱
            var handler = _socketHandlers.GetOrAdd(proxyPolicy.GetKey(), (k) =>
            {
                return new SocketsHttpHandler()
                {
                    Credentials = null,
                    MaxConnectionsPerServer = 1024,
                    MeterFactory = null,
                    AllowAutoRedirect = false,
                    AutomaticDecompression = DecompressionMethods.None,
                    UseCookies = false,
                    UseProxy = true,
                    PreAuthenticate = false,
                    Proxy = new AddressProxy(proxyPolicy),
                    ActivityHeadersPropagator = null,
                    EnableMultipleHttp2Connections = true,
                };
            });
            return new HttpMessageInvoker(handler, false);
        }
    }
}
