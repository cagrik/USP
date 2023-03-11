using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USP.Models
{
    public class SignatureStatusM
    {
        public string jsonrpc { get; set; }
        public resultsg result { get; set; }


        public UInt64 id { get; set; }
    }
    public class resultsg
    {
        public resulvalue[] value { get; set; }
        public sigctx context { get; set; }
    }
    public class resulvalue
    {
        public UInt64 slot { get; set; }
        public UInt64[] confirmations { get; set; }
        public object err { get; set; }
        public string confirmationStatus { get; set; }
    }
    public class sigctx {
        public UInt64 slot { get; set; }
        public string apiVersion { get; set; }
    }
}
