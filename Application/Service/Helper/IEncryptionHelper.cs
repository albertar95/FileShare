using System;

namespace Application.Helper
{
    public interface IEncryptionHelper
    {
        string EncryptString(string text);
        string DecryptString(string cipher);
        string RSADecrypt(string data);
        string RSAEncrypt(string data);
        string LocalRSADecrypt(string data);
        string LocalRSAEncrypt(string data);
        Tuple<string, string> GeneratePrivatePublicKey();
        string GenerateToken(string client, string timestamp);
        bool AccessCheckByToken(string cipher, string client, string timestamp);
    }
}
