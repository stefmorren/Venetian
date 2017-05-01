using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BusinessLayer
{
    public class HashUtility
    {
        public static string GenerateSalt()
        {
           return Guid.NewGuid().ToString();
        }

        public static string GenerateSHA256FromString(string text)
        {
            SHA256 mySha256 = SHA256Managed.Create();
            Encoding enc = Encoding.UTF8;
            Byte[] result = mySha256.ComputeHash(enc.GetBytes(text));
            StringBuilder Sb = new StringBuilder();
            foreach (Byte b in result)
            {
                Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public static byte[] GenerateSHA256FromStringToByteArray(string text)
        {
            SHA256 mySha256 = SHA256Managed.Create();
            Encoding enc = Encoding.UTF8;
            byte[] result = mySha256.ComputeHash(enc.GetBytes(text));
            return result;
        }
    }
}
