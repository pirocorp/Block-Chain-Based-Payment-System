﻿namespace PaymentSystem.WalletApp.Services
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.GrpcService;

    public interface IBlockChainGrpcService
    {
        Task<AccountCreationResponse> CreateAccount();

        Task<BoolResponse> DeleteAccount(string address);
    }
}
