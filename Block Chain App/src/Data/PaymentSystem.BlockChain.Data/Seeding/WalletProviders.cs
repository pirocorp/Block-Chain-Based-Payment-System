namespace PaymentSystem.BlockChain.Data.Seeding
{
    using System.Collections.Generic;

    public static class WalletProviders
    {
        public static IEnumerable<IcoAccount> Accounts => new List<IcoAccount>()
        {
            new IcoAccount()
            {
                Address = "d5fcbb68d47a0178068d02a33830cdd6a3ba89c650c3cac3308d589a687a919f",
                PublicKey = "0691D82FA69E7D2523906C43179B354347D76B94196A14FC35CA5A69B07D93F41197195F61A240524217CB1251150576457401C6DDF0790B0856EB5ED11CC60D",
                Balance = 15_000_000,
                Secret = "49C8B4F1C230128EE0DDBAF3AF5FA1576D0F648855D7EE4D5DF36161E7F5DC2C",
            },
            new IcoAccount()
            {
                Address = "ec4abb4f54ab9ad9d7e4a12578dd312062485a758a1c5e8dc1e533f182447d11",
                PublicKey = "95D5C39F780AB4107874B586DAA0891180546541FAB055C80776B030CDF48F8689ECF6D8990972A4CDF69DD5ACE5F7FDE4EB250CF592A94911F60D0BD4608E91",
                Balance = 5_000_000,
                Secret = "6ACC1633FC9B42377BB517C1FFBC7C4B75A99082373607694DE4C566B3B08EFB",
            },
            new IcoAccount()
            {
                Address = "9b9256542f091cf270b4b7dd69eca4e1cff320cac6ea970a914cd35afe382ec2",
                PublicKey = "DEF3FDF6429ACA33F32940241D5E4AC974D2780F4EB596F5678361FAFE102C8C9D41B6AC7D5CB06BFC64F4101E28C5B193C0FA34436A6C7508B51CC5FB4E8855",
                Balance = 10_000_000,
                Secret = "1D3FC65F340A15A8523BEE9A7D8DE92A2F7FBB3898694A320BF7EFC5CDC1641A",
            },
            new IcoAccount()
            {
                Address = "4a01fc19f646e2386f8d109ab0ce2bd31741d1bcfb376b94c81123629d44aeb7",
                PublicKey = "47B733B5EF4D7B7930A22893FA847E31D4CB4AA62827419AB58A534513D01814C68AC91A4F9F38632F83A25DB539483D534C9AACF67E532E5FD60FF68D6DCA7F",
                Balance = 21_000_000,
                Secret = "0ECB9944B311994C36A5598D9856F982B30C395E5EF5EA3E68C37A6A58B7C130",
            },
        };

        public class IcoAccount
        {
            public string Address { get; set; }

            public double Balance { get; set; }

            public string PublicKey { get; set; }

            /// <summary>
            /// Secret is used for restoring private key.
            /// Only for testing purposes is here.
            /// Secrets are never stored.
            /// </summary>
            public string Secret { get; set; }
        }
    }
}
