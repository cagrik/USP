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
using System.Security.Cryptography;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimplePayment : Page
    {
        public SimplePayment()
        {
            this.InitializeComponent();
        }
        PaymentRequest pr;
        string signature = String.Empty;
        bool isFinalized = false;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           
            string refKey =KeyPair.Generate();// Generates a reference key for track payment on chain
             pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", 0.1, refKey,"USP Sample","Thanks For Using USP","OrderId123");
            pr.cluster = Clusters.DevNet;
            string paymentLink = pr.CreatePaymentLink();
            var qcbytes = QrCodeGenerator.Generate(paymentLink);
            qrCodeImg.Source = await QrCodeGenerator.QrCodeImage(qcbytes);

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
            else { 
            
                if (isFinalized) {
                    bool isCompleted = await pr.isFullPayment(signature);
                    if(isCompleted) {
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
            string url = $"https://explorer.solana.com/tx/{signature}?cluster=devnet";
            wv.Source = new Uri(url);
        }
    }
}
