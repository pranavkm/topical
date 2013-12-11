using System;
using System.Security.Cryptography;

namespace Topical
{
    public static class IdProvider
    {
        public static string GenerateTopicId()
        {
            byte[] array = new byte[6];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(array);
                return Convert.ToBase64String(array).ToLowerInvariant();
            }
        }
    }
}