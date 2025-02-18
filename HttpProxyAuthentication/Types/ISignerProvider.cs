using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyAuthentication.Types
{
    public interface ISignerProvider
    {
        IEnumerable<string> Supports();
        bool TryGetSigner(string algorithm, out ISigner signer);
        ISigner GetSigner();
    }
}
