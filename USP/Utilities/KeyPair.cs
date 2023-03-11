using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace USP.Utilities
{
    public static class KeyPair
    {
        public static string Generate() {
            var ecdsa = new ECDsaCng(ECCurve.NamedCurves.nistP256);
            ecdsa.GenerateKey(ECCurve.NamedCurves.nistP256);
            //ecdsa.KeySize = 44;
            var cc = ecdsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
            var publicKeyBase58 = Base58Encoding.Encode(cc);
            publicKeyBase58 = publicKeyBase58.Substring(0, 44);
            return publicKeyBase58;
        }
    }
}
