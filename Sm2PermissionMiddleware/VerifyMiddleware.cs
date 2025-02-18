using CommonTypes;
using HttpProxyAuthentication;
using HttpProxyAuthentication.Types;
using HttpProxyUtility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PermissionMiddleware
{
    internal class VerifyMiddleware
    {
        private const int _authHeaderSchemeLength = 7;

        private ISignerProviderStorage _signerProviderStorage;
        private ILogger<VerifyMiddleware> _logger;
        private readonly RequestDelegate _next;
        private IProxyPolicyProvider _proxyPolicyProvider;

        private Task _checkPermission(HttpContext context)
        {
            var policy = context.GetPolicy(_proxyPolicyProvider);
            if (policy != null && policy.NeedAuthorization())
            {
                var auth = context.Request.Headers.ProxyAuthorization.ToString();
                if (!string.IsNullOrEmpty(auth))
                {
                    if (auth.StartsWith(HeaderConsts.AuthorizationScheme, StringComparison.OrdinalIgnoreCase))
                    {
                        var token = auth.Substring(_authHeaderSchemeLength);
                        _logger.LogInformation($"Get token {token}");
                        if (!string.IsNullOrEmpty(token) && Permission.TryParse(token, out var permisson))
                        {
                            var result = permisson.Verify(_signerProviderStorage, policy.GetAuthorizationKey, DateTime.UtcNow, 32);
                            if (result == PermissionVerifyState.Success)
                            {
                                context.Request.Headers["X-Forwarded-User"] = permisson.User;
                                return _next(context);
                            }
                        }
                    }
                }
                context.Response.Headers.ProxyAuthenticate = $"{HeaderConsts.AuthorizationScheme} {DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            }
            return _next(context);
        }
        public VerifyMiddleware(ISignerProviderStorage signerProviderStorage, IProxyPolicyProvider proxyPolicyProvider, ILogger<VerifyMiddleware> logger, RequestDelegate next)
        {
            _signerProviderStorage = signerProviderStorage;
            _proxyPolicyProvider = proxyPolicyProvider;
            _logger = logger;
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            return _checkPermission(context);
        }
    }
    public static class VerifyMiddlewareExtensions
    {
        public static IApplicationBuilder UseVerifyMiddleware(this IApplicationBuilder builder, Action<ISignerProviderStorage>? addScheme = null)
        {
            addScheme?.Invoke(builder.ApplicationServices.GetRequiredService<ISignerProviderStorage>());
            return builder.UseMiddleware<VerifyMiddleware>();
        }
    }
}
