namespace PaymentSystem.Common.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Text;

    using EllipticCurve;
    using Newtonsoft.Json;

    public static class ConvertExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static byte[] HexToBytes(this string hex)
            => Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

        public static string BytesToHex(this byte[] bytes)
            => Convert.ToHexString(bytes).ToUpper();

        public static string BigIntegerToHex(this BigInteger input)
            => input.ToString("X");

        public static BigInteger HexToBigInteger(this string input)
            => BigInteger.Parse(input, NumberStyles.AllowHexSpecifier);

        public static string PublicKeyToString(this PublicKey key)
            => Convert.ToHexString(key.toString());

        public static byte[] ToByteArray(this object source)
        {
            var asString = JsonConvert.SerializeObject(source, SerializerSettings);
            return Encoding.UTF8.GetBytes(asString);
        }

        public static T ToObject<T>(this byte[] source)
        {
            var asString = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(asString);
        }
    }
}
