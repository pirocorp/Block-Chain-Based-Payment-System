﻿namespace PaymentSystem.WalletApp.Services.Implementations
{
    using System.Threading.Tasks;

    using Grpc.Net.Client;

    using PaymentSystem.Common;
    using PaymentSystem.Common.GrpcService;

    public class BlockChainGrpcService : IBlockChainGrpcService
    {
        private readonly ComunicationService.ComunicationServiceClient service;

        public BlockChainGrpcService()
        {
            var chanel = GrpcChannel.ForAddress(GlobalConstants.ChanelAddress);
            this.service = new ComunicationService.ComunicationServiceClient(chanel);
        }

        public async Task<AccountCreationResponse> CreateAccount()
            => await this.service.CreateAccountAsync(new EmptyRequest());

        public async Task<BoolResponse> DeleteAccount(string address)
            => await this.service.DeleteAccountAsync(new AccountAddressRequest()
            {
                Address = address,
            });

        public async Task<TransactionResponse> AddTransactionToPool(SendRequest sendRequest)
            => await this.service.AddTransactionToPoolAsync(sendRequest);
    }
}
