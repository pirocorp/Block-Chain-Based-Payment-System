﻿namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Services.Data.Models.Accounts;

    public interface IAccountService
    {
        Task<bool> Exists(string address);

        Task<bool> UserOwnsAccount(string userId, string address);

        Task<T> GetAccount<T>(string address);

        Task<IEnumerable<T>> GetAccounts<T>(string userId);

        Task<AccountCreationResponse> Create(string userId);

        Task EditAccount(EditAccountServiceModel model);

        Task Delete(string address);
    }
}