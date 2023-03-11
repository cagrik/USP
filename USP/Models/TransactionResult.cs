using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USP.Models
{
    public class TransactionResult
    {
        public string jsonrpc { get; set; }
        public resultt result { get; set; }
        
        
        public UInt64 id { get; set; }
        
    }
    public class resultt
    {
        public UInt64 slot { get; set; }
        public UInt64 blocktime { get; set; }
        public metap meta { get; set; }
        public TransactionM transaction { get; set; }
    }
    public class metap
    {
        public object err { get; set; }
        public UInt64 fee{ get; set; }
        public object innerInstructions { get; set; }
        public UInt64[] preBalances { get; set; }
        public UInt64[] postBalances { get; set; }
        public TokenBalance[] preTokenBalances { get; set; }
        public TokenBalance[] postTokenBalances { get; set; }
        public UInt64 computeUnitsConsumed { get; set; }
        //rewards
    }
}
