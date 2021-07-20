namespace PaymentSystem.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    using Data.Models;
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

        [Fact]
        public void ObjectToBytesArrayAndByteArrayToObject()
        {
            var random = new Random();

            var block = new Block()
            {
                Hash = Guid.NewGuid().ToString(),
                Height = 2,
                BlockHeader = new BlockHeader()
                {
                    Difficulty = random.Next(),
                    MerkleRoot = Guid.NewGuid().ToString(),
                    PreviousHash = Guid.NewGuid().ToString(),
                    TimeStamp = random.Next(),
                    Validator = Guid.NewGuid().ToString(),
                    Version = random.Next(),
                },
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Recipient = Guid.NewGuid().ToString(),
                        Amount = random.Next(),
                        TimeStamp = random.Next(),
                        Fee = random.Next(),
                        Sender = Guid.NewGuid().ToString(),
                        BlockHash = Guid.NewGuid().ToString(),
                    },
                    new Transaction()
                    {
                        Recipient = Guid.NewGuid().ToString(),
                        Amount = random.Next(),
                        TimeStamp = random.Next(),
                        Fee = random.Next(),
                        Sender = Guid.NewGuid().ToString(),
                        BlockHash = Guid.NewGuid().ToString(),
                    },
                }
            };

            var result = block.ToByteArray().ToObject<Block>();

            Assert.Equal(block.Hash, result.Hash);
            Assert.Equal(block.Height, result.Height);
            Assert.Equal(block.Transactions.Count(), result.Transactions.Count());
            Assert.Equal(block.BlockHeader.Difficulty, result.BlockHeader.Difficulty);
            Assert.Equal(block.BlockHeader.TimeStamp, result.BlockHeader.TimeStamp);
            Assert.Equal(block.BlockHeader.MerkleRoot, result.BlockHeader.MerkleRoot);
            Assert.Equal(block.BlockHeader.PreviousHash, result.BlockHeader.PreviousHash);
            Assert.Equal(block.BlockHeader.Validator, result.BlockHeader.Validator);
            Assert.Equal(block.BlockHeader.Version, result.BlockHeader.Version);
            Assert.Equal(block.Transactions.First().Recipient, result.Transactions.First().Recipient);
            Assert.Equal(block.Transactions.First().Amount, result.Transactions.First().Amount);
            Assert.Equal(block.Transactions.First().TimeStamp, result.Transactions.First().TimeStamp);
            Assert.Equal(block.Transactions.First().Fee, result.Transactions.First().Fee);
            Assert.Equal(block.Transactions.First().Sender, result.Transactions.First().Sender);
            Assert.Equal(block.Transactions.First().BlockHash, result.Transactions.First().BlockHash);
            Assert.Equal(block.Transactions.Last().Recipient, result.Transactions.Last().Recipient);
            Assert.Equal(block.Transactions.Last().Amount, result.Transactions.Last().Amount);
            Assert.Equal(block.Transactions.Last().TimeStamp, result.Transactions.Last().TimeStamp);
            Assert.Equal(block.Transactions.Last().Fee, result.Transactions.Last().Fee);
            Assert.Equal(block.Transactions.Last().Sender, result.Transactions.Last().Sender);
            Assert.Equal(block.Transactions.Last().BlockHash, result.Transactions.Last().BlockHash);
        }
    }
}
