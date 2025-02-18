using HttpProxyAuthentication.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyAuthentication
{
    public class SignerProviderStorage : ISignerProviderStorage
    {
        private ISigner defaultSigner;
        private ConcurrentBag<ISignerProvider> _list = new ConcurrentBag<ISignerProvider>();

        public IEnumerable<ISignerProvider> GetSigners()
        {
            return _list;
        }

        public void Register(ISignerProvider provider)
        {
            _list.Add(provider);
        }

        public IEnumerable<string> Supports()
        {
            foreach (ISignerProvider provider in _list)
            {
                foreach (var spt in provider.Supports())
                    yield return spt;
            }
        }

        public bool TryGetSigner(string algorithm, out ISigner signer)
        {
            foreach (ISignerProvider provider in _list)
            {
                if (provider.TryGetSigner(algorithm, out signer))
                    return true;
            }
            signer = default;
            return false;
        }

        public ISigner GetSigner()
        {
            return defaultSigner;
        }

        public SignerProviderStorage()
        {
            defaultSigner = new EmptySigner();
        }
    }
}
