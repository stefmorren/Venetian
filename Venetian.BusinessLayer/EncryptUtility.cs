using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BusinessLayer
{
    public class EncryptUtility
    {
        public static string GenerateSalt()
        {
           return Guid.NewGuid().ToString();
        }

        public static string GenerateSHA256(string saltedPassword)
        {
            SHA256 mySha256 = SHA256Managed.Create();
            Encoding enc = Encoding.UTF8;
            Byte[] result = mySha256.ComputeHash(enc.GetBytes(saltedPassword));
            StringBuilder Sb = new StringBuilder();
            foreach (Byte b in result)
            {
                Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public static List<string> GeneratePublicAndPrivateKeys()
        {
            var csp = new RSACryptoServiceProvider(2048);
            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);
            string pubKeyString;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
            }

            string privKeyString;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                privKeyString = sw.ToString();
            }

            return new List<string> {pubKeyString, privKeyString};
        }
    }
}
