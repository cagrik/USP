using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using USP.Models;
using Windows.Web.Http;

namespace USP.RPC
{
    public class RpcClient
    {
        public string httpclient { get; set; }
        public RpcClient(Clusters c)
        {

            switch (c)
            {
                case Clusters.MainNet:
                    httpclient = "https://api.mainnet-beta.solana.com";
                    break;
                case Clusters.TestNet:
                    httpclient = "https://api.testnet.solana.com";
                    break;
                case Clusters.DevNet:
                    httpclient = "https://api.devnet.solana.com";
                    break;
                default:
                    httpclient = "https://api.devnet.solana.com";
                    break;
            }
        }

        //address; reference parameter in transfer req link
        public SignatureForAddressResult getSignaturesForAddress(string address)
        {
            SignatureForAddressResult result = new SignatureForAddressResult();
            var reques = WebReq();
          
            using (var streamWriter = new StreamWriter(reques.GetRequestStream()))
            {
               
                string js= @"{""jsonrpc"": ""2.0"",""id"": 1,""method"": ""getSignaturesForAddress"",""params"": [""referenceaddress"",{""limit"": 1}]}";
               js= js.Replace("referenceaddress", address);
                streamWriter.Write(js);
            }
           
            var httpResponse = (HttpWebResponse)reques.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                
                var _result = streamReader.ReadToEnd();
                result =    System.Text.Json.JsonSerializer.Deserialize<SignatureForAddressResult>(_result);
               
                
            }
            return result;
        }
           
        public async Task<TransactionResult> getTransaction(string signature)
        {
            TransactionResult result = new TransactionResult();
           
string js = @"{""method"":""getTransaction"",""params"":[""Signature_"",{""encoding"":""json"",""commitment"":""finalized""}],""jsonrpc"":""2.0"",""id"":0}";
            js = js.Replace("Signature_", signature);

            var res = await RpclCall(js);
                result = System.Text.Json.JsonSerializer.Deserialize<TransactionResult>(res);
            

          

            return result;
        }

        public async Task<SignatureStatusM> getSignatureStatuses(string signature)
        {
            SignatureStatusM result = new SignatureStatusM();
            string js = @"{""jsonrpc"": ""2.0"",""id"": 1,""method"": ""getSignatureStatuses"",""params"": [[""Signature_""],{""searchTransactionHistory"":true,""encoding"":""json""}]}";
            js = js.Replace("Signature_", signature);
            var res = await RpclCall(js);
            result = System.Text.Json.JsonSerializer.Deserialize<SignatureStatusM>(res);
            return result;

        }
        private HttpWebRequest WebReq()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(httpclient);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            return httpWebRequest;
        }

        private async Task<string> RpclCall(string _json)
        {
            var buffer = Encoding.UTF8.GetBytes(_json);
            var httpReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, (string)null)
            {
                Content = new ByteArrayContent(buffer)
                {
                    Headers = {
                            { "Content-Type", "application/json"}
                        }
                }
            };
            string result = String.Empty;
            System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(httpclient) };
            var response = await _httpClient.SendAsync(httpReq).ConfigureAwait(false);
            {
                 result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }
}
/* var buffer = Encoding.UTF8.GetBytes(js);
             var httpReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, (string)null)
            {
                Content = new ByteArrayContent(buffer)
                {
                    Headers = {
                            { "Content-Type", "application/json"}
                        }
                }
            };
            System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(httpclient) };
            var response = await _httpClient.SendAsync(httpReq).ConfigureAwait(false);
            {
                var result = await response.Content.ReadAsStringAsync();
                result2 = System.Text.Json.JsonSerializer.Deserialize<TransactionResult>(result);
            }

            var fark = result2.result.meta.postBalances[1] - result2.result.meta.preBalances[1];
            double fk = Convert.ToDouble(fark);
            double solSend = fk / 1000000000;
*/