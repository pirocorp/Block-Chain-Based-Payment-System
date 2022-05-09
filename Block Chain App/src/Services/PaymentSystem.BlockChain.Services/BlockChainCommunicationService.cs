namespace PaymentSystem.BlockChain.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Grpc.Core;

    using PaymentSystem.BlockChain.Services.Data;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.Common.Transactions;
    using PaymentSystem.Common.Utilities;

    public class BlockChainCommunicationService : ComunicationService.ComunicationServiceBase
    {
        private readonly IAccountService accountService;
        private readonly IBlockChainService blockChainService;
        private readonly ITransactionPool transactionPool;

        public BlockChainCommunicationService(
            IAccountService accountService,
            IBlockChainService blockChainService,
            ITransactionPool transactionPool)
        {
            this.accountService = accountService;
            this.blockChainService = blockChainService;
            this.transactionPool = transactionPool;
        }

        public override async Task<AccountCreationResponse> CreateAccount(EmptyRequest request, ServerCallContext context)
        {
            var account = await this.accountService.Create();

            return new AccountCreationResponse()
            {
                Address = account.Address,
                Balance = account.Balance,
                PublicKey = account.PublicKey,
                Secret = account.Secret
            };
        }

        public override async Task<BoolResponse> DeleteAccount(AccountAddressRequest request, ServerCallContext context)
            => new() { Success = await this.accountService.Delete(request.Address) };

        public override async Task<BlockResponse> GenesisBlock(EmptyRequest request, ServerCallContext context)
        {
            var block = await this.blockChainService.GetGenesisBlock();

            var model = new BlockResponse()
            {
                Block = ConvertBlockToBlockModel(block),
            };

            return model;
        }

        public override async Task<BlockResponse> LastBlock(EmptyRequest request, ServerCallContext context)
        {
            var block = await this.blockChainService.GetLastBlock();

            var model = new BlockResponse()
            {
                Block = ConvertBlockToBlockModel(block),
            };

            return model;
        }

        public override async Task<BlockModel> GetBlockByHash(HashRequest request, ServerCallContext context)
        {
            var blockHash = request.Hash;
            var block = await this.blockChainService.GetBlockByHash(blockHash);

            var model = ConvertBlockToBlockModel(block);
            return model;
        }

        public override async Task<BlockModel> GetBlockByHeight(HeightRequest request, ServerCallContext context)
        {
            var blockHeight = request.Height;
            var block = await this.blockChainService.GetBlockByHeight(blockHeight);

            var model = ConvertBlockToBlockModel(block);
            return model;
        }

        public override async Task<BlocksResponse> GetBlocks(BlockRequest request, ServerCallContext context)
        {
            var pageNumber = Math.Max(0, request.PageNumber);
            var resultsPerPage = Math.Min(GlobalConstants.MaxBlockPageSize, request.ResultPerPage);

            var blocks = await this.blockChainService
                .GetBlocks(pageNumber, resultsPerPage);

            var response = new BlocksResponse();

            foreach (var block in blocks)
            {
                response.Blocks.Add(ConvertBlockToBlockModel(block));
            }

            return response;
        }

        public override async Task GetBlockChain(BlockChainRequest request, IServerStreamWriter<BlockModel> responseStream, ServerCallContext context)
        {
            var height = request.Height;

            await foreach (var blocks in this.blockChainService.GetBlockChain(height))
            {
                foreach (var block in blocks)
                {
                    var blockModel = ConvertBlockToBlockModel(block);
                    await responseStream.WriteAsync(blockModel);
                }
            }
        }

        public override async Task<TransactionResponse> AddTransactionToPool(SendRequest request, ServerCallContext context)
        {
            var transaction = new Transaction()
            {
                Hash = request.TransactionId,
                TimeStamp = request.TransactionInput.TimeStamp,
                Sender = request.TransactionInput.SenderAddress,
                Recipient = request.TransactionOutput.RecipientAddress,
                Amount = request.TransactionOutput.Amount,
                Fee = request.TransactionOutput.Fee,
            };

            var transactionHash = BlockChainHashing.GenerateTransactionHash(transaction);

            // if some intermediately changes the data in transaction this check will fail.
            if (!transactionHash.Equals(transaction.Hash))
            {
                return await Task.FromResult(InvalidTransactionResponse());
            }

            var isValid = BlockChainHashing
                .VerifySignature(request.PublicKey, request.TransactionId, request.TransactionInput.Signature);

            // if transaction is made from someone not possessing the private key this check fill fail.
            if (!isValid)
            {
                return await Task.FromResult(InvalidTransactionResponse());
            }

            this.transactionPool.Add(transaction);
            var response = new TransactionResponse() { Result = TransactionStatus.Pending.ToString() };

            return await Task.FromResult(response);
        }

        private static TransactionResponse InvalidTransactionResponse()
        {
            var response = new TransactionResponse()
            {
                Result = TransactionStatus.Canceled.ToString(),
            };

            return response;
        }

        private static BlockModel ConvertBlockToBlockModel(Block block)
        {
            var blockHeader = new BlockHeaderModel()
            {
                Version = block.BlockHeader.Version,
                PreviousHash = block.BlockHeader.PreviousHash ?? string.Empty,
                MerkleRoot = block.BlockHeader.MerkleRoot,
                TimeStamp = block.BlockHeader.TimeStamp,
                Difficulty = block.BlockHeader.Difficulty,
                Validator = block.BlockHeader.Validator,
            };

            var blockModel = new BlockModel()
            {
                Hash = block.Hash,
                Height = block.Height,
                BlockHeader = blockHeader,
            };

            block.Transactions
                .Select(t => new TransactionModel()
                {
                    Hash = t.Hash,
                    TimeStamp = t.TimeStamp,
                    Sender = t.Sender,
                    Recipient = t.Recipient,
                    Amount = t.Amount,
                    Fee = t.Fee,
                })
                .ToList()
                .ForEach(t => blockModel.Transactions.Add(t));

            return blockModel;
        }
    }
}
