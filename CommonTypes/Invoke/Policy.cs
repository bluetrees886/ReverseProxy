using CommonTypes.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Invoke
{
    public class Policy
    {
        public string? Name { get; set; }
        public string Protocal { get; set; } = "http";
        public string? Host {  get; set; }
        public string? Port { get; set; }
        public string? Path { get; set; }
        public IEnumerable<Authorization>? Authorizations { get; set; }
        public Request? Request { get; set; }
        public Response? Response { get; set; }
        public bool Forbidden { get; set; }
        public int? StatusCode { get; set; }
    }
}
