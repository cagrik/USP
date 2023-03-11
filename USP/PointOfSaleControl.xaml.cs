using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace USP
{
    public sealed partial class PointOfSaleControl : UserControl
    {
        public string RecieverAdress
        {
            get { return (string)GetValue(RecieverAdressProperty); }
            set { SetValue(RecieverAdressProperty, value); }
        }
        public static readonly DependencyProperty RecieverAdressProperty =
            DependencyProperty.Register(
                "RecieverAdress",
                typeof(string),
                typeof(PointOfSaleControl),
                new PropertyMetadata(null));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(PointOfSaleControl),
                new PropertyMetadata(null));

        public string Memo
        {
            get { return (string)GetValue(MemoProperty); }
            set { SetValue(MemoProperty, value); }
        }
        public static readonly DependencyProperty MemoProperty =
            DependencyProperty.Register(
                "Memo",
                typeof(string),
                typeof(PointOfSaleControl),
                new PropertyMetadata(null));
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label",
                typeof(string),
                typeof(PointOfSaleControl),
                new PropertyMetadata(null));

        public Clusters ClusterNode
        {
            get { return (Clusters)GetValue(ClusterNodeProperty); }
            set { SetValue(ClusterNodeProperty, value); }
        }
        public static readonly DependencyProperty ClusterNodeProperty =
           DependencyProperty.Register(
               "ClusterNode",
               typeof(Clusters),
               typeof(PointOfSaleControl),
               new PropertyMetadata(null));

        public PointOfSaleControl()
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
            switch (value_)
            {
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
            Double.TryParse(txtTotal.Text, out amount);
            string refKey = KeyPair.Generate();
            pr = new PaymentRequest(RecieverAdress, amount, refKey,Label,Message,Memo);
            pr.cluster = ClusterNode;
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
            RpcClient rpc = new RpcClient(ClusterNode);
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
                        lblPaymentStatus.Text = "The Payment is completed.";
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
