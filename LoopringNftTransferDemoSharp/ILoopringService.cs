using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopNftTransferDemoSharp
{
    public interface ILoopringService
    {
        Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId);
        Task<OffchainFee> GetOffChainFee(string apiKey, int accountId, int requestType, string amount);
        Task<string> SubmitNftTransfer(
            string apiKey,
            string exchange,
            int fromAccountId,
            string fromAddress,
                 int toAccountId,
                 string toAddress,
                 int nftTokenId,
                 string nftAmount,
                 int maxFeeTokenId,
                 string maxFeeAmount,
                 int storageId,
                 long validUntil,
                 string eddsaSignature,
                 string ecdsaSignature,
                 string nftData
                 );
    }
}
