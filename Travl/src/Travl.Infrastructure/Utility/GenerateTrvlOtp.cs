using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Infrastructure.Utility
{
    public class GenerateTrvlOtp
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public static string GenerateOtp(int length = 6)
        {
            if (length < 4 || length > 10) throw new ArgumentException("OTP length must be between 4 and 10.");

            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            var otp = new StringBuilder();
            foreach (byte b in randomBytes)
            {
                otp.Append((b % 10).ToString());
            }

            return otp.ToString();
        }
    }
}
