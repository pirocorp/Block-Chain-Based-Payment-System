﻿namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<bool> Exists(string address);

        Task Create(string address, string publicKey);

        Task<bool> TryDeposit(string address, double amount);

        Task<bool> TryWithdraw(string address, double amount);
    }
}
