using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Invoke
{
    public class Response
    {
        public IEnumerable<Header>? Headers { get; set; }
        public int? StatusCode { get; set; }
        public string? Location { get; set; }
    }
}
