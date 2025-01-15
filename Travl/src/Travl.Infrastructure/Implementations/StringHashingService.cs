using System.Security.Cryptography;
using System.Text;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;

namespace Travl.Infrastructure.Implementations
{
    public class StringHashingService : IStringHashingService
    {
        private readonly HashingSettings _hashingSettings;
        private readonly byte[] _key;

        public StringHashingService(HashingSettings hashingSettings)
        {
            _hashingSettings = hashingSettings;
            _key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_hashingSettings.SecretKey));
        }

        public string CreateDESStringHash(string input)
        {
            using (var aes = Aes.Create())
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(input);
                aes.Key = _key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] resultArray = encryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray);
            }
        }

        public string DecodeDESStringHash(string input)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    byte[] toEncryptArray = Convert.FromBase64String(input);
                    aes.Key = _key;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    byte[] resultArray = decryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
