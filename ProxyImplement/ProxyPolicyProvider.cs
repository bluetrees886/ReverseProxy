using CommonTypes;
using CommonTypes.Invoke;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyImplement
{
    public class ProxyPolicyProvider : IProxyPolicyProvider
    {
        private IMemoryCache _cache;
        private IEnumerable<IProxyPolicy> _listSettings()
        {
            if (_settings?.Value.Policies == null)
                yield break;
            foreach(var policy in _settings.Value.Policies)
                yield return new ProxyPolicy(policy);
        }
        private IEnumerable<IProxyPolicy> _proxyPolicies;
        private IOptions<ProxySettings>? _settings;
        private IEnumerable<IProxyPolicy> _doFilter(Uri url)
        {
            foreach (var policy in _proxyPolicies)
            {
                if (policy.Match(url))
                    yield return policy;
            }
        }
        public IEnumerable<IProxyPolicy> Filter(Uri url)
        {
            return _cache.GetOrCreate(url, (k) =>
            {
                return  _doFilter(url).ToArray();
            })!;
        }
        public ProxyPolicyProvider(IMemoryCache cache, IOptions<ProxySettings> settings)
        {
            _cache = cache;
            _settings = settings;
            _proxyPolicies = _listSettings();
        }
    }
}
