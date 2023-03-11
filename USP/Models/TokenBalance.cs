using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USP.Models
{
    public class TokenBalance
    {
        public UInt64 accountIndex { get; set; }
        public string mint { get; set; }
        public string owner { get; set; }
        public string programId { get; set; }
        public _uiTokenAmount uiTokenAmount { get; set; }
    }
    public class _uiTokenAmount
    {
        public string amount { get; set; }
        public int decimals { get; set; }
        public string uiAmountString { get; set; }

    }
}
