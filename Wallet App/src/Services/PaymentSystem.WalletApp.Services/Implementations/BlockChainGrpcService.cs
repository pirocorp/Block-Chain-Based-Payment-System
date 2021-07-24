namespace PaymentSystem.WalletApp.Services.Implementations
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
            var chanel = GrpcChannel.ForAddress(GlobalConstants.GrpcChanelAddress);
            this.service = new ComunicationService.ComunicationServiceClient(chanel);
        }

        public async Task<AccountCreationResponse> CreateAccount()
            => await this.service.CreateAccountAsync(new EmptyRequest());
    }
}
