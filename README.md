# Universal SolPay
Universal Solpay; allows to developers build Universal Windows Platform Applications that accept Solana Payments. 

Now, Universal SolPay Supports Windows Forms on .Net Framework V>=4.62 & .Net V>6.0
![Universal Solpay Logo](https://raw.githubusercontent.com/cagrik/USP/master/UspLogo-b2.png)

# Start Building

Add Universal SolPay Package from Nuget [https://www.nuget.org/packages/USP](https://www.nuget.org/packages/USP)
[Check Samples for Windows Forms](https://github.com/cagrik/USP/tree/master/USP.Samples)

.Net CLI
```
dotnet add package USP --version 1.0.7
```

Package Manager
```
NuGet\Install-Package USP -Version 1.0.7
```

##### Add Name Spaces to top of your code

```
using USP;
using USP.RPC;
using USP.Utilities;
```

##### Declare Global Variables
```
PaymentRequest pr;
string signature = String.Empty;
bool isFinalized = false;
```

##### Create a Payment Request
```
double amount = 0.0;
Double.TryParse(txtTotal.Text, out  amount);
string refKey = KeyPair.Generate();
pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", amount, refKey, Lable, Message, Memo);
pr.cluster = Clusters.DevNet;
```

##### Create a Payment Request Link and Generate a QrCode from that Link
```
string paymentLink = pr.CreatePaymentLink();
var qcbytes = QrCodeGenerator.Generate(paymentLink);
qrcodeimg.Source = await QrCodeGenerator.QrCodeImage(qcbytes); //qrcodeimg is an Image Element
```

##### Start a Timer 
```
DispatcherTimer tmr = new DispatcherTimer();
tmr.Tick += Tmr_Tick;
tmr.Interval = TimeSpan.FromSeconds(1);
tmr.Start();
```

##### Check the Payment is Completed Successfully in timer's Tick
```
  private async void Tmr_Tick(object sender, object e)
        {
            RpcClient rpc = new RpcClient(Clusters.DevNet);
            var res = rpc.getSignaturesForAddress(pr.Reference);

            if (res.result != null && res.result.Length > 0) signature = res.result[0].signature;
            if (String.IsNullOrEmpty(signature)) { }
            else
            {

                if (isFinalized)
                {
                    bool isCompleted = await pr.isFullPayment(signature);
                    if (isCompleted)
                    {
                        DispatcherTimer dpt = sender as DispatcherTimer;
                        dpt.Stop();
                        lblPaymentStatus.Text = "The Payment is completed. Click For details";
                        //The transaction is Finalized and A Full Payment is made.  You can write your app logic below
                    }
                }
                else
                {
                    isFinalized = await pr.isFinalized(signature);

                }
            }

        }
```
