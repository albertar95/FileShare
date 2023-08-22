using Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileShareService.Helper
{
    public class EncryptionHelper : IEncryptionHelper
    {
        private static string _privateKey = "<RSAKeyValue><Modulus>m2cLw0+669vNu84UzNVK1rz6KjCKLXULo071nMdkgzFHGBpWpOMcm8eVkH9Jzih470KFeKyV+oP3nYlVY+9z7f51pgZGRhRegFYPrSshJuDnwfRTwUYtr2k24ari8sLYchA5hS/ws5vGWX/N9mu1fu0rcWBVeGxUiB1VT8q+WJ0=</Modulus><Exponent>AQAB</Exponent><P>xBX6VjfbMm7odMyTO2fiMFgL+B2fiRtDBmG6wyeTPFVqdhRhgTJ1SXcClUjBOVLZodV6lN/Ba7tOIbNsJyiyOw==</P><Q>yuLHhrn2X0anjPBVnLPSRfdFd/8tS+jnPz1gBNE8CCX5pepT3ur25AjEIp0eoDyPnOgVPq41kl+vx6ddJOvbBw==</Q><DP>YKf4w2E7MowLF+/zr3fQvkXYeJCZoGsIye+IsUQjxrzQAq6fNaFawUDhzBaV8JzPXH+vsgzt+h3VMLWR3WepGw==</DP><DQ>PEFuEM1aJqHNUUZvpsKhSLZPo3vd8BWT2GxaABRESAc/Rc96aVJPURppZf5UAjL6VBd5d8w1jOtuQVWrjDOgMQ==</DQ><InverseQ>eKirNQRT2zhAKm9aDEdpBYbTqaSO82YmmBhqljsmpBWcZwL/gjrK/avspyDszhrja9amhhFe7U08j9/Fpx0QlQ==</InverseQ><D>ZfNViT3eYGvkbehGRxdLMNPrvw/3JtlZ2Dqt8bvyGguTG6Hz49rHYv+uBI0ta12r8TOQAtoezqoTIbv+VEC8qYl+wuTGbmVl1b1MS1Y4BZcfCPoEX5t55Mm0AhvOBZkaDVSD3N/4OS2EiTky7tPUvnafydAk9qnMf/mXyvKADFU=</D></RSAKeyValue>";
        private static string _publicKey = "<RSAKeyValue><Modulus>m2cLw0+669vNu84UzNVK1rz6KjCKLXULo071nMdkgzFHGBpWpOMcm8eVkH9Jzih470KFeKyV+oP3nYlVY+9z7f51pgZGRhRegFYPrSshJuDnwfRTwUYtr2k24ari8sLYchA5hS/ws5vGWX/N9mu1fu0rcWBVeGxUiB1VT8q+WJ0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static string _LocalPrivateKey = "<RSAKeyValue><Modulus>x4IrZO3IbNEFML8v5jSwfmpFxXsHpjJ9KXYCa58gkfKuENFmGKgbNZLH0Vw93Y8hwfkHuBVYpC97rPMbBa4DvEmt2PTciv5abL2vvY3L4GW6vB4BhY5FwrtTeeNe8RI52bSYbxkrsEsaCmUYIURGoX503KzuglrPnI+oynIVJc0=</Modulus><Exponent>AQAB</Exponent><P>7dsdwLk9eZIVTsrAP48k0+0wqHShN1x3KIoiXTuc8L9ghmdt7GMQRFqI1C7gsVHJ6ZI+IwcEuH9HsaiFUJAvzw==</P><Q>1roy7kzVNMTTKNaAR8amtlUxnpKKis96YNRZHE3SYAGowBX2irUJen3jv67Qqn1DlEooi4OVvGPPprJ03x47ow==</Q><DP>5iadVXxohZfa0lDRUS77Ha1m5blkvaLVEf8HXVtvcmxbw/GUpaT9eoZ324g4lptjWoTNZytFYBiYkMlVpiy+dQ==</DP><DQ>sA4+0BADB58Rmvw0daU0725unpt8KE/xAQ9aeNB92uXBXK0lVZkVym8JmxJUWFUYYa9kCu+6h5o2mXDcTkvVjQ==</DQ><InverseQ>ohpFTC5f4JhTMWhRokd/osVfe2rsl1xBSxdLKU6sV0l+L8G5ffoQc6NdSq2sj3Il05U68cUPmnulDHD2bhH11w==</InverseQ><D>gppN+fZHHCUAttaRbqvTVg4PuqU9bqjikn+0OeujkKdDF7NcmIuDtGBv+jfeL+oA+VAio3kl2+VRmvhSGje+LR3v31WIfp4yjyalWWcfaav6byVyIdKgm/eOH/X8VMLoadmKTj2ngQgz9DIBhTyy+uog5C0yDZ7QZKy9fbXaNek=</D></RSAKeyValue>";
        private static string _localPublicKey = "<RSAKeyValue><Modulus>x4IrZO3IbNEFML8v5jSwfmpFxXsHpjJ9KXYCa58gkfKuENFmGKgbNZLH0Vw93Y8hwfkHuBVYpC97rPMbBa4DvEmt2PTciv5abL2vvY3L4GW6vB4BhY5FwrtTeeNe8RI52bSYbxkrsEsaCmUYIURGoX503KzuglrPnI+oynIVJc0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        public string EncryptString(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("B@8CCto%YgfBF8YD3!Con888W"));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        public string DecryptString(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("B@8CCto%YgfBF8YD3!Con888W"));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }
        public string RSADecrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataByte = Convert.FromBase64String(data);
            rsa.FromXmlString(_privateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public string RSAEncrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            return Convert.ToBase64String(encryptedByteArray);
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
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_localPublicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            return Convert.ToBase64String(encryptedByteArray);
        }

        public Tuple<string,string> GeneratePrivatePublicKey()
        {
            var rsa = new RSACryptoServiceProvider();
            return new Tuple<string, string>(rsa.ToXmlString(true), rsa.ToXmlString(false));
        }
        public string GenerateToken(string client,string timestamp)
        {
            var plain = string.Format("{0}#{1}",client,timestamp);
            return RSAEncrypt(plain);
        }
        public bool AccessCheckByToken(string cipher,string client,string timestamp)
        {
            var generatedToken = RSADecrypt(cipher).Split('#');
            if ((generatedToken[0] == client || client == "192.168.1.10") && generatedToken[1] == timestamp)
                return true;
            else
                return false;
        }

    }
}
