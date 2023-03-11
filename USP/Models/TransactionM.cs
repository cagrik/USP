using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USP.Models
{
    public class TransactionM
    {
        public Messagee message { get; set; }
        public string[] signatures { get; set; }

    }
    public class Messagee {
        public string[] accountKeys { get; set; }
        public Headerr header { get; set; }
        public InstructionM[] instructions { get; set; }
        public string recentBlockhash { get; set; }
       
    }
    public class Headerr
    {
        public int numReadonlySignedAccounts { get; set; }
        public int numReadonlyUnsignedAccounts { get; set; }
        public int numRequiredSignatures { get; set; }
    }
    public class getTransactionM
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public string method { get; set; }
        public string[]  @params { get; set; }
    }
}
