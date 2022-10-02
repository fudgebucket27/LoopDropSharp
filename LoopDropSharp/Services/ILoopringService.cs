using LoopDropSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public interface ILoopringService
    {
        Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId);
        Task<OffchainFee> GetOffChainFee(string apiKey, int accountId, int requestType, string amount);

        Task<TransferFeeOffchainFee> GetOffChainTransferFee(string apiKey, int accountId, int requestType, string feeToken, string amount);
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
                 string nftData, 
                 string transferMemo
                 );
        Task<string> SubmitTokenTransfer(
          string apiKey,
              string exchange,
              int fromAccountId,
              string fromAddress,
                   int toAccountId,
                   string toAddress,
                   int tokenId,
                   string tokenAmount,
                   int maxFeeTokenId,
                   string maxFeeAmount,
                   int storageId,
                   long validUntil,
                   string eddsaSignature,
                   string ecdsaSignature,
                   string memo
                 );
        Task<EnsResult> GetHexAddress(string apiKey, string ens);
        Task<string> CheckForEthAddress(string apiKey, string address);
        Task<NftBalance> GetTokenId(string apiKey, int accountId, string nftData);
        Task<NftBalance> GetTokenIdWithCheck(string apiKey, int accountId, string nftData);
        Task<NftData> GetNftData(string apiKey, string nftId, string minter, string tokenAddress);
        Task<List<NftHolder>> GetNftHoldersMultiple(string apiKey, string nftData);
        Task<List<NftHoldersAndTotal>> GetNftHoldersMultipleOld(string apiKey, string nftData);
        Task<NftHoldersAndTotal> GetNftHolders(string apiKey, string nftData);
        Task<AccountInformation> GetUserAccountInformation(string accountId);
        Task<AccountInformation> GetUserAccountInformationFromOwner(string owner);
        Task<List<MintsAndTotal>> GetUserMintedNfts(string apiKey, int accountId);
        Task<List<NftData>> GetUserMintedNftsWithCollection(string apiKey, int accountId, string collectionId);
        Task<List<NftData>> GetNftInformationFromNftData(string apiKey, string nftData);
        Task<bool> CheckBanishTextFile(string toAddressInitial, string toAddress, string loopringApiKey);
    }
}
