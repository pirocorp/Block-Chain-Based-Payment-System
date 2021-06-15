namespace PaymentSystem.Common.Tests
{
    using System.Numerics;
    using Utilities;
    using Xunit;

    public class ConvertExtensionsTests
    {
        [Fact]
        public void HexToBytesReturnsCorrectAnswer()
        {
            var hex =
                "0691D82FA69E7D2523906C43179B354347D76B94196A14FC35CA5A69B07D93F41197195F61A240524217CB1251150576457401C6DDF0790B0856EB5ED11CC60D";

            var bytesSmall = hex.ToLower().HexToBytes();
            var bytes = hex.HexToBytes();

            var hexFromBytesSmall = bytesSmall.BytesToHex();
            var hexFromBytes = bytes.BytesToHex();

            Assert.Equal(hex, hexFromBytesSmall);
            Assert.Equal(hex, hexFromBytes);
        }

        [Fact]
        public void BigIntegerToHexCorrectAnswer()
        {
            var bigInt = BigInteger.Parse("6692107653641313721595699763191817727899822167184919868551389686053939429680");

            var hex = bigInt.BigIntegerToHex();
            var bigIntFromHex = hex.HexToBigInteger();

            Assert.Equal(bigIntFromHex, bigInt);
        }
    }
}
