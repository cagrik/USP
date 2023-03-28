using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USP.RPC;
namespace USP.Utilities
{
    public class PaymentRequest
    {
        public string Recipient { get; set; }
        public string Label { get; set; }
        public string Message { get; set; }
        public string Memo { get; set; }
        public double Amount { get; set; }
        public string SPL { get; set; }
        public string Reference { get; set; }
        public Clusters cluster { get; set; }
        public PaymentRequest(string recipient, double amount, string reference, string label, string message, string memo, string sPL)
        {
            Recipient = recipient;
            Label = label;
            Message = message;
            Memo = memo;
            Amount = amount;
            SPL = sPL;
            Reference = reference;
        }
        public PaymentRequest(string recipient, double amount, string reference, string label, string message, string memo)
        {
            Recipient = recipient;
            Label = label;
            Message = message;
            Memo = memo;
            Amount = amount;
            Reference = reference;
        }
        public PaymentRequest(string recipient,  double amount, string reference)
        {
            Recipient = recipient;
            Amount = amount;
            Reference = reference;
        }
        public string CreatePaymentLink()
        {
            
            StringBuilder sb = new StringBuilder("solana:");
            if (String.IsNullOrEmpty(Recipient) || String.IsNullOrEmpty(Reference) || Amount<=0.0) {
                throw new Exception("Missing required Parameters");
            }
            else
            {
              sb.Append(Recipient);

                sb.Append("?amount=").Append(Amount);
                sb.Append("&reference=").Append(Reference);

                if (!String.IsNullOrEmpty(Label))
                {
                    sb.Append("&label=").Append(Label);
                }
                if (!String.IsNullOrEmpty(Message))
                {
                    sb.Append("&message=").Append(Message);
                }
                if (!String.IsNullOrEmpty(Memo))
                {
                    sb.Append("&memo=").Append(Memo);
                }
            }
            return sb.ToString();
        }

        public async Task<bool> isFinalized(string signature)
        {
            bool r = false;
            RpcClient rc = new RpcClient(cluster);
            try
            {
                var result = await rc.getSignatureStatuses(signature);
                if (result != null&&(result.result!=null&&result.result.value.Length>0)) {
                  r=  result.result.value[0].confirmationStatus == "finalized";
                }
            }
            catch (Exception)
            {

                
            }
            return r;
        }
        //Check the received amount and Reciever Wallet address which is same with created payment request link(prevent to manipulate qr codes)
        public async Task<bool> isFullPayment(string signature)
        {
            bool r = false;
            RpcClient rc = new RpcClient(cluster);
            try
            {
                var result = await rc.getTransaction(signature);
                if (result != null&& result.result.meta!=null)
                {
                    var fark = result.result.meta.postBalances[1] - result.result.meta.preBalances[1];
                    double fk = Convert.ToDouble(fark);
                    double solSend = fk / 1000000000;
                    r = solSend == Amount;
                    if (!result.result.transaction.message.accountKeys.Contains(Recipient))
                    {
                        r = false;

                    }
                }
            }
            catch (Exception)
            {


            }
            return r;
        }
    }
}
