using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyAuthentication.Types
{
    public interface ISigner
    {
        string Scheme { get; }
        string Sign(string user, string message, string signKey, string password);
        bool Verify(string user, string message, string signature, string verifyKey);
    }
}
