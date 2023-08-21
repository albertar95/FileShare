using Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FileShareWebUI2.Helpers
{
    public class LocalEncryptionHelper : IEncryptionHelper
    {
        private static string _publicKey = "<RSAKeyValue><Modulus>m2cLw0+669vNu84UzNVK1rz6KjCKLXULo071nMdkgzFHGBpWpOMcm8eVkH9Jzih470KFeKyV+oP3nYlVY+9z7f51pgZGRhRegFYPrSshJuDnwfRTwUYtr2k24ari8sLYchA5hS/ws5vGWX/N9mu1fu0rcWBVeGxUiB1VT8q+WJ0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static string _LocalPrivateKey = "<RSAKeyValue><Modulus>x4IrZO3IbNEFML8v5jSwfmpFxXsHpjJ9KXYCa58gkfKuENFmGKgbNZLH0Vw93Y8hwfkHuBVYpC97rPMbBa4DvEmt2PTciv5abL2vvY3L4GW6vB4BhY5FwrtTeeNe8RI52bSYbxkrsEsaCmUYIURGoX503KzuglrPnI+oynIVJc0=</Modulus><Exponent>AQAB</Exponent><P>7dsdwLk9eZIVTsrAP48k0+0wqHShN1x3KIoiXTuc8L9ghmdt7GMQRFqI1C7gsVHJ6ZI+IwcEuH9HsaiFUJAvzw==</P><Q>1roy7kzVNMTTKNaAR8amtlUxnpKKis96YNRZHE3SYAGowBX2irUJen3jv67Qqn1DlEooi4OVvGPPprJ03x47ow==</Q><DP>5iadVXxohZfa0lDRUS77Ha1m5blkvaLVEf8HXVtvcmxbw/GUpaT9eoZ324g4lptjWoTNZytFYBiYkMlVpiy+dQ==</DP><DQ>sA4+0BADB58Rmvw0daU0725unpt8KE/xAQ9aeNB92uXBXK0lVZkVym8JmxJUWFUYYa9kCu+6h5o2mXDcTkvVjQ==</DQ><InverseQ>ohpFTC5f4JhTMWhRokd/osVfe2rsl1xBSxdLKU6sV0l+L8G5ffoQc6NdSq2sj3Il05U68cUPmnulDHD2bhH11w==</InverseQ><D>gppN+fZHHCUAttaRbqvTVg4PuqU9bqjikn+0OeujkKdDF7NcmIuDtGBv+jfeL+oA+VAio3kl2+VRmvhSGje+LR3v31WIfp4yjyalWWcfaav6byVyIdKgm/eOH/X8VMLoadmKTj2ngQgz9DIBhTyy+uog5C0yDZ7QZKy9fbXaNek=</D></RSAKeyValue>";
        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        public bool AccessCheckByToken(string cipher, string client, string timestamp)
        {
            throw new NotImplementedException();
        }

        public string DecryptString(string cipher)
        {
            throw new NotImplementedException();
        }

        public string EncryptString(string text)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, string> GeneratePrivatePublicKey()
        {
            throw new NotImplementedException();
        }

        public string GenerateToken(string client, string timestamp)
        {
            var plain = string.Format("{0}#{1}", client, timestamp);
            return RSAEncrypt(plain);
        }

        public string LocalRSADecrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataByte = Convert.FromBase64String(data);
            rsa.FromXmlString(_LocalPrivateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public string LocalRSAEncrypt(string data)
        {
            throw new NotImplementedException();
        }

        public string RSADecrypt(string data)
        {
            throw new NotImplementedException();
        }

        public string RSAEncrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            return Convert.ToBase64String(encryptedByteArray);
        }
    }
}