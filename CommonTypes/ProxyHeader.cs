using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public struct ProxyHeader
    {
        public bool Local {  get; set; }
        public string Key { get; set; }
        public IEnumerable<string> Value { get; set; }
    }
}
