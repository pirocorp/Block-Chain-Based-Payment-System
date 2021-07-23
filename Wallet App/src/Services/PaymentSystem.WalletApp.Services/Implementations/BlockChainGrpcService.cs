namespace PaymentSystem.WalletApp.Services.Implementations
{
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
    }
}
