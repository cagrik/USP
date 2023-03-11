using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USP.Models
{
    public class SignatureForAddressResult
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public _SignatureForAddressResult[] result { get; set; } 
    }
    public class _SignatureForAddressResult
    {
        public object err { get; set; }
        public string memo { get; set; }

        public string signature { get; set; }
        public UInt64 slot { get; set; }
        public UInt64 blocktime { get; set; }

    }
}
