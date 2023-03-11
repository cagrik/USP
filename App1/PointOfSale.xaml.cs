using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using USP;
using USP.RPC;
using USP.Utilities;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PointOfSale : Page
    {
        public PointOfSale()
        {
            this.InitializeComponent();
        }
        PaymentRequest pr;
        string signature = String.Empty;
        bool isFinalized = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string dgr = txtTotal.Text;
            string value_ = btn.Content.ToString();
            switch (value_) {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                    dgr += value_;
                    break;
                case "C":
                    dgr = String.Empty;
                    break;
                case ".":
                    if (!dgr.Contains(".")) { if (dgr.Length < 1) { dgr = "0."; } else { dgr += "."; } }
                    break;
            }
            txtTotal.Text = dgr;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double amount = 0.0;
           Double.TryParse(txtTotal.Text, out  amount);
            string refKey = KeyPair.Generate();
             pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", amount, refKey, "USP Sample", "Thanks For Using USP", "OrderId123");
            pr.cluster = Clusters.DevNet;
            string paymentLink = pr.CreatePaymentLink();
            var qcbytes = QrCodeGenerator.Generate(paymentLink);
            qrcodeimg.Source = await QrCodeGenerator.QrCodeImage(qcbytes);

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Tick += Tmr_Tick;
            tmr.Interval = TimeSpan.FromSeconds(1);
            tmr.Start();

        }

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
                    }
                }
                else
                {
                    isFinalized = await pr.isFinalized(signature);

                }
            }

        }

        private void lblPaymentStatus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string clusternet = "";
            switch (pr.cluster)
            {
                case Clusters.DevNet:
                    clusternet = "?cluster=devnet";
                    break;
                case Clusters.TestNet:
                    clusternet = "?cluster=testnet";
                    break;
                default:
                    clusternet = "";
                    break;
            }
            string url = $"https://explorer.solana.com/tx/{signature}{clusternet}";
            wv.Source = new Uri(url);
            wv.Navigate(new Uri(url));
        }
    }
}
