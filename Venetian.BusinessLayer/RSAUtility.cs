using System.Collections.Generic;
using System.Security.Cryptography;

namespace Venetian.BusinessLayer
{
    public class RSAUtility
    {
        //Sources:
        //    http://stackoverflow.com/questions/17128038/c-sharp-rsa-encryption-decryption-with-transmission

        public static List<string> GenerateRSAPublicAndPrivateKeys()
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

            return new List<string> { pubKeyString, privKeyString };
        }

        public static byte[] RSAEncrypt(string key, byte[] message)
        {
            var csp = new RSACryptoServiceProvider(2048);

            //convert public key
            //get a stream from the string
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var pubKey = (RSAParameters)xs.Deserialize(sr);

            csp.ImportParameters(pubKey);
            return csp.Encrypt(message, false);
        }

        public static byte[] RSADecrypt(string key, byte[] encryptedMessage)
        {
            var csp = new RSACryptoServiceProvider(2048);
            //convert private key
            //get a stream from the string
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var privKey = (RSAParameters)xs.Deserialize(sr);

            csp.ImportParameters(privKey);
            return csp.Decrypt(encryptedMessage, false);
        }

        public static byte[] RSASign(string key, byte[] message)
        {
            var csp = new RSACryptoServiceProvider(2048);
            //convert private key
            //get a stream from the string
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var privKey = (RSAParameters)xs.Deserialize(sr);
            csp.ImportParameters(privKey);
            return csp.SignData(message, new SHA256CryptoServiceProvider());
        }

        public static bool RSAVerifySignedHash(byte[] DataToVerify, byte[] SignedData, string key)
        {
            var csp = new RSACryptoServiceProvider(2048);
            //convert private key
            //get a stream from the string
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var DeserializedKey = (RSAParameters)xs.Deserialize(sr);
            csp.ImportParameters(DeserializedKey);
            return csp.VerifyData(DataToVerify, new SHA256CryptoServiceProvider(), SignedData );
        }
    }
}