﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UspNetFramework;
using UspNetFramework.Rpc;
using UspNetFramework.Utulities;
namespace UniSolPay.WinForms.NetFramework
{
    public partial class Form1 : Form
    {
        PaymentRequest pr;
        string signature = String.Empty;
        bool isFinalized = false;
        List<Payments> _payments;
        string refKey = String.Empty;
        List<PaymentRequest> paymentRequests;
        public Form1()
        {
            InitializeComponent();
            _payments = new List<Payments>();
            paymentRequests = new List<PaymentRequest>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string dgr = txtTotal.Text;
            string value_ = btn.Tag.ToString();

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
                    dgr += value_;
                    break;
                case "0":
                    if (dgr != "0")
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

        private void button13_Click(object sender, EventArgs e)
        {
            double amount = 0.0;
            double.TryParse(txtTotal.Text, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out amount);
            refKey = KeyPair.Generate();// 1Generates a reference key for track payment on chain
            pr = new PaymentRequest("8X5zHADZ4oTZXC59JmjVn1NRqcA6DGc649TrwL11ZdNr", amount, refKey, "USP Sample", "Thanks For Using USP", "OrderId123");
            pr.cluster = Clusters.DevNet;
            string paymentLink = pr.CreatePaymentLink();
            var qcbytes = QrCodeGenerator.Generate(paymentLink);
            MemoryStream ms = new MemoryStream(qcbytes);
            Payments p = new Payments();
            p.QrCodeImage = System.Drawing.Image.FromStream(ms);
            ImgQrCode.Image = p.QrCodeImage;


            p.Amount = amount;
            p.Status = "Requested";
            p.RefKey = refKey;
            _payments.Add(p);
            paymentRequests.Add(pr);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _payments;

            lblRequestedAmount.Text = $"Requested Amount: {amount} SOL";
            txtTotal.Clear();
            timer1.Start();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            var incompletedPayments = _payments.Where(x=>x.Status=="Requested").ToList();
            if (incompletedPayments.Count < 1) { timer1.Stop(); }
            foreach (var item in incompletedPayments)
            {
                var _pr = paymentRequests.SingleOrDefault(x=>x.Reference==item.RefKey);
                if (_pr == null)
                {
                    return;
                }
                RpcClient rpc = new RpcClient(_pr.cluster);
                var res = rpc.getSignaturesForAddress(_pr.Reference);

                if (res.result != null && res.result.Length > 0) signature = res.result[0].signature;
                if (String.IsNullOrEmpty(signature)) { }
                else
                {

                    if (isFinalized)
                    {
                        bool isCompleted = await _pr.isFullPayment(signature);
                        if (isCompleted)
                        {

                           
                            //  lblPaymentStatus.Text = "The Payment is completed.";
                            var pyr = _payments.Where(x => x.RefKey == _pr.Reference).SingleOrDefault();
                            pyr.Status = "Completed";
                            paymentRequests.Remove(_pr);
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = _payments;

                        }
                    }
                    else
                    {
                        isFinalized = await _pr.isFinalized(signature);

                    }
                } 
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRow = (Payments)dataGridView1.SelectedRows[0].DataBoundItem;
            var selectedPayment = _payments.SingleOrDefault(x => x.RefKey == selectedRow.RefKey);
            if (selectedPayment.Status == "Completed")
            {
                ImgQrCode.Image = null;
                lblRequestedAmount.Text = "This Payment is Completed";
            }
            else
            {
                lblRequestedAmount.Text = $"Requested Amount: {selectedPayment.Amount} SOL";
                ImgQrCode.Image = selectedPayment.QrCodeImage;
            }
        }

        private void txtTotal_KeyDown(object sender, KeyEventArgs e)
        {
            string kpValue = Char.ConvertFromUtf32(e.KeyValue).ToLower();
            string dgr = txtTotal.Text;


            switch (kpValue)
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
                    dgr += kpValue;
                    e.SuppressKeyPress = true;
                    break;
                case "0":
                    if (dgr != "0")
                        dgr += kpValue;
                    e.SuppressKeyPress = true;
                    break;
                case "c":
                case "C":
                    dgr = String.Empty;
                    e.SuppressKeyPress = true;
                    break;
                case ".":
                    if (!dgr.Contains(".")) { if (dgr.Length < 1) { dgr = "0."; } else { dgr += "."; } }
                    e.SuppressKeyPress = true;
                    break;
                default:

                    e.SuppressKeyPress = true;
                    break;
            }
            txtTotal.Text = dgr;
        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
            }
            catch
            {


            }
        }
    }
}
