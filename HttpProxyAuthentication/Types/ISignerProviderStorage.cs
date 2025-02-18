using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyAuthentication.Types
{
    public interface ISignerProviderStorage : ISignerProvider
    {
        void Register(ISignerProvider provider);
        IEnumerable<ISignerProvider> GetSigners();
    }
}
