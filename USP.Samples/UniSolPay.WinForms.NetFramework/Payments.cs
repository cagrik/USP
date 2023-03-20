using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniSolPay.WinForms.NetFramework
{
    public class Payments
    {
        public double Amount { get; set; }
        public string Status { get; set; }
        public string RefKey { get; set; }
        [Browsable(false)]
        public Image QrCodeImage { get; set; }
    }
}
