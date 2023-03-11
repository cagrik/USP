using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using USP;
using USP.RPC;
using USP.Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InAppPurchase : Page
    {
        int magId = 0;
        PaymentRequest pr;
        string signature = String.Empty;
        bool isFinalized = false;
        Button snd;
        public InAppPurchase()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
             snd = sender as Button;
            int selectedMagId = Convert.ToInt32(snd.Tag);
            magId = selectedMagId;
            string refKey = KeyPair.Generate();// Generates a reference key for track payment on chain
            pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", 0.5, refKey, "Buy Science Mag", "Thanks For Using USP", "OrderId"+magId);
            pr.cluster = Clusters.DevNet;
            string paymentLink = pr.CreatePaymentLink();
            var qcbytes = QrCodeGenerator.Generate(paymentLink);
            qrcodeImg.Source = await QrCodeGenerator.QrCodeImage(qcbytes);

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
                        snd.IsEnabled = false;
                        foreach (var item in coversg.Children)
                        {
                            Image img = item as Image;
                            int tagid = Convert.ToInt32(img.Tag);
                            if (tagid == magId) { img.Opacity = 1.0; }
                            qrcodeImg.Source = null;
                        }
                    }
                }
                else
                {
                    isFinalized = await pr.isFinalized(signature);

                }
            }
        }
    }
}
