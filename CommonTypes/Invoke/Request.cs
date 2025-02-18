using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Invoke
{
    public class Request
    {
        public string? DnsServer { get; set; }
        public string? Address { get; set; }
        public string? Url {  get; set; }
        public IEnumerable<Header>? Headers { get; set; }
    }
}
