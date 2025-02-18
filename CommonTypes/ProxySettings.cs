using CommonTypes.Invoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public class ProxySettings
    {
        public Policy? DefaultPolicy { get; set; }
        public IEnumerable<Policy>? Policies { get; set; }
    }
}
