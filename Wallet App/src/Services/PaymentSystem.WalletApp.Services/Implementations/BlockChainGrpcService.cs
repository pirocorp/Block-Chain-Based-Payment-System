namespace PaymentSystem.WalletApp.Services.Implementations
{
    using System.Linq;
    using System.Threading.Tasks;

    using Grpc.Net.Client;

    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
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

        public async Task<Block> GetBlock(long height)
        {
            var request = new HeightRequest()
            {
                Height = height,
            };

            var block = await this.service.GetBlockByHeightAsync(request);
            return ConvertBlockModelToBlock(block);
        }

        private static Block ConvertBlockModelToBlock(BlockModel blockModel)
            => new ()
            {
                BlockHeader = new BlockHeader()
                {
                    Difficulty = blockModel.BlockHeader.Difficulty,
                    MerkleRoot = blockModel.BlockHeader.MerkleRoot,
                    PreviousHash = blockModel.BlockHeader.PreviousHash,
                    TimeStamp = blockModel.BlockHeader.TimeStamp,
                    Validator = blockModel.BlockHeader.Validator,
                    Version = blockModel.BlockHeader.Version,
                },
                Hash = blockModel.Hash,
                Height = blockModel.Height,
                Transactions = blockModel.Transactions
                    .Select(t => new Transaction()
                    {
                        Amount = t.Amount,
                        BlockHash = blockModel.Hash,
                        Fee = t.Fee,
                        Hash = t.Hash,
                        Recipient = t.Recipient,
                        Sender = t.Sender,
                        TimeStamp = t.TimeStamp,
                    })
                    .ToList(),
            };
    }
}
