using System;
using System.Security.Cryptography;
using System.Text;

namespace Topical
{
    public static class IdProvider
    {
        private const string Alphabets = "abcdefghijklmnopqrstuvwxyz234567";

        public static string GenerateId()
        {
            byte[] array = new byte[6];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(array);
                return Base32Encode(array).ToLowerInvariant();
            }
        }

        /// <remarks>
        /// Courtesy of http://www.codeproject.com/Tips/76650/Base32-base32url-base64url-and-z-base-32-encoding
        /// </remarks>
        private static string Base32Encode(byte[] data)
		{
            var result = new StringBuilder(Math.Max((int)Math.Ceiling(data.Length * 8 / 5.0), 1));
			// take input five bytes at a time to chunk it up for encoding
			for (int i = 0; i < data.Length; i += 5)
			{
				int bytes = Math.Min(data.Length - i, 5);
				// parse five bytes at a time using an 8 byte ulong
                byte[] buff = new byte[8];
				Array.Copy(data, i, buff, buff.Length - (bytes+1), bytes);
				Array.Reverse(buff);
				ulong val = BitConverter.ToUInt64(buff, 0);
				for (int bitOffset = ((bytes+1) * 8) - 5; bitOffset > 3; bitOffset -= 5)
				{
					result.Append(Alphabets[(int)((val >> bitOffset) & 0x1f)]);
				}
			}
			return result.ToString();
		}
    }
}