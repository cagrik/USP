
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using USP;
using USP.RPC;
using USP.Utilities;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
          /*   PayScreen ps = new PayScreen();
             //ps.Show();
             //ps.Wr();
             RpcClient rpc = new RpcClient(Clusters.DevNet);
            // var r=rpc.getSignaturesForAddress("Fc7aCzx4uVaaFAU8RqYRQXwpdi3sR67b3rvuT9wuq2rR");
            // var c = await rpc.getTransaction(r.result[0].signature);
            //  var c = await rpc.getSignatureStatuses(r.result[0].signature);
            // QrCodeCtrl qc = new QrCodeCtrl();
            // g1.Children.Add(qc);
            string refr = KeyPair.Generate();
            PaymentRequest pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", 0.001, refr,"Uni Sol Pay","Thanks for purchase","saleid=1111");
            string paymentlink=pr.CreatePaymentLink();
            
            var qrcd= QrCodeGenerator.Generate(paymentlink);*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SimplePayment), null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PointOfSale), null);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InAppPurchase), null);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PointOfSaleWithControl), null);
        }
    }
}
