namespace Application.Helper
{
    public interface IEncryptionHelper
    {
        string EncryptString(string text);
        string DecryptString(string cipher);
    }
}
