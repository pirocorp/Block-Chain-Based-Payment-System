﻿namespace PaymentSystem.Common.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;

    public static class ConvertExtensions
    {
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
    }
}