using LoopDropSharp;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public class LoopringService : ILoopringService, IDisposable
    {
        const string _baseUrl = "https://api3.loopring.io";

        readonly RestClient _client;

        public LoopringService()
        {
            _client = new RestClient(_baseUrl);
        }


        public async Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId)
        {
            var request = new RestRequest("api/v3/storageId");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("sellTokenId", sellTokenId);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<StorageId>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting storage id: {httpException.Message}");
                return null;
            }
        }

        public async Task<OffchainFee> GetOffChainFee(string apiKey, int accountId, int requestType, string amount)
        {
            var request = new RestRequest("api/v3/user/nft/offchainFee");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("requestType", requestType);
            request.AddParameter("amount", amount);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<OffchainFee>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting off chain fee: {httpException.Message}");
                return null;
            }
        }


        public async Task<string> SubmitNftTransfer(
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
            )
        {
            var request = new RestRequest("api/v3/nft/transfer");
            request.AddHeader("x-api-key", apiKey);
            request.AddHeader("x-api-sig", ecdsaSignature);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("exchange", exchange);
            request.AddParameter("fromAccountId", fromAccountId);
            request.AddParameter("fromAddress", fromAddress);
            request.AddParameter("toAccountId", toAccountId);
            request.AddParameter("toAddress", toAddress);
            request.AddParameter("token.tokenId", nftTokenId);
            request.AddParameter("token.amount", nftAmount);
            request.AddParameter("token.nftData", nftData);
            request.AddParameter("maxFee.tokenId", maxFeeTokenId);
            request.AddParameter("maxFee.amount", maxFeeAmount);
            request.AddParameter("storageId", storageId);
            request.AddParameter("validUntil", validUntil);
            request.AddParameter("eddsaSignature", eddsaSignature);
            request.AddParameter("ecdsaSignature", ecdsaSignature);
            try
            {
                var response = await _client.ExecutePostAsync(request);
                var data = response.Content;
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error submitting nft transfer: {httpException.Message}");
                return null;
            }
        }

        public async Task<string> SubmitTokenTransfer(
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
          )
        {
            var request = new RestRequest("api/v3/transfer");
            request.AddHeader("x-api-key", apiKey);
            request.AddHeader("x-api-sig", ecdsaSignature);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("exchange", exchange);
            request.AddParameter("payerId", fromAccountId);
            request.AddParameter("payerAddr", fromAddress);
            request.AddParameter("payeeId", toAccountId);
            request.AddParameter("payeeAddr", toAddress);
            request.AddParameter("token.tokenId", tokenId);
            request.AddParameter("token.volume", tokenAmount);
            request.AddParameter("maxFee.tokenId", maxFeeTokenId);
            request.AddParameter("maxFee.volume", maxFeeAmount);
            request.AddParameter("storageId", storageId);
            request.AddParameter("validUntil", validUntil);
            request.AddParameter("eddsaSignature", eddsaSignature);
            request.AddParameter("ecdsaSignature", ecdsaSignature);
            request.AddParameter("memo", memo);
            try
            {
                var response = await _client.ExecutePostAsync(request);
                var data = response.Content;
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error submitting token transfer: {httpException.Message}");
                return null;
            }
        }

        public async Task<EnsResult> GetHexAddress(string apiKey, string ens)
        {
            var request = new RestRequest("api/wallet/v3/resolveEns");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("fullName", ens);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<EnsResult>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting ens: {httpException.Message}");
                return null;
            }
        }
        public async Task<string> CheckForEthAddress(string apiKey, string address)
        {
            string minterForNftData = address.Trim().ToLower();
            if (address.Contains(".eth"))
            {
                var varHexAddress = await GetHexAddress(apiKey, minterForNftData);
                if (!String.IsNullOrEmpty(varHexAddress.data))
                {
                    minterForNftData = varHexAddress.data;
                    return minterForNftData;
                }
                else
                {
                    Font.SetTextToYellow($"{minterForNftData} is an invalid address");
                    return null;
                }
            }
            return address;
        }

        public async Task<NftBalance> GetTokenId(string apiKey, int accountId, string nftData)
        {
            var data = new NftBalance();
            var counter = 0;
            var request = new RestRequest("/api/v3/user/nft/balances");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("nftDatas", nftData);
            try
            {
                    var response = await _client.GetAsync(request);
                    data = JsonConvert.DeserializeObject<NftBalance>(response.Content!);
                    counter++;
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<NftBalance> GetTokenIdWithCheck(string apiKey, int accountId, string nftData)
        {
            var data = new NftBalance();
            var counter = 0;
            var request = new RestRequest("/api/v3/user/nft/balances");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("nftDatas", nftData);
            try
            {
                do
                {
                    if (counter != 0)
                    {
                        Font.SetTextToYellow("This is not an NftData or this Nft isn't in your wallet. Please enter in a correct one.");
                        nftData = Console.ReadLine();
                        request.AddOrUpdateParameter("nftDatas", nftData);
                    }
                    var response = await _client.GetAsync(request);
                    data = JsonConvert.DeserializeObject<NftBalance>(response.Content!);
                    counter++;
                } while (data.data.Count == 0);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<NftData> GetNftData(string apiKey, string nftId, string minter, string tokenAddress)
        {
            var data = new NftData();
            var request = new RestRequest("/api/v3/nft/info/nftData");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("nftId", nftId);
            request.AddParameter("minter", minter);
            request.AddParameter("tokenAddress", tokenAddress);
            try
            {
                    var response = await _client.GetAsync(request);
                    data = JsonConvert.DeserializeObject<NftData>(response.Content!);
                Thread.Sleep(100);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                if (httpException.Message == "Request failed with status code BadRequest")
                {
                    Font.SetTextToRed("The above information did not find an Nft Data. Please, try again.");
                    return null;
                }
                Font.SetTextToWhite($"Error getting NftData: {httpException.Message}");
                return null;
            }
        }

        public async Task<List<NftHolder>> GetNftHoldersMultiple(string apiKey, string nftData)
        {
            var allData = new List<NftHolder>();
            var request = new RestRequest("/api/v3/nft/info/nftHolders");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("nftData", nftData);
            request.AddParameter("limit", 100);
            try
            {
                var offset = 100;
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<NftHoldersAndTotal>(response.Content!);
                var total = data.totalNum;

                allData.AddRange(data.nftHolders);
                while (total > 100)
                {
                    total = total - 100;
                    request.AddOrUpdateParameter("offset", offset);
                    response = await _client.GetAsync(request);
                    var moreData = JsonConvert.DeserializeObject<NftHoldersAndTotal>(response.Content!);
                    allData.AddRange(moreData.nftHolders);
                    offset = offset + 100;
                }
                return allData;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }
        public async Task<List<NftHoldersAndTotal>> GetNftHoldersMultipleOld(string apiKey, string nftData)
        {
            var allData = new List<NftHoldersAndTotal>();
            var request = new RestRequest("/api/v3/nft/info/nftHolders");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("nftData", nftData);
            request.AddParameter("limit", 100);
            try
            {
                var offset = 100;
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<NftHoldersAndTotal>(response.Content!);
                var total = data.totalNum;
                allData.Add(data);
                while (total > 100)
                {
                    total = total - 100;
                    request.AddOrUpdateParameter("offset", offset);
                    response = await _client.GetAsync(request);
                    var moreData = JsonConvert.DeserializeObject<NftHoldersAndTotal>(response.Content!);
                    allData.Add(moreData);
                    offset = offset + 100;
                }
                return allData;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }
        public async Task<NftHoldersAndTotal> GetNftHolders(string apiKey, string nftData)
        {
            var request = new RestRequest("/api/v3/nft/info/nftHolders");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("nftData", nftData);
            request.AddParameter("limit", 100);
            try
            {
                var offset = 100;
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<NftHoldersAndTotal>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<AccountInformation> GetUserAccountInformation(string accountId)
        {
            var request = new RestRequest("/api/v3/account");
            request.AddParameter("accountId", accountId);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<AccountInformation>(response.Content!);
                Thread.Sleep(100);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<AccountInformation> GetUserAccountInformationFromOwner(string owner)
        {
            var request = new RestRequest("/api/v3/account");
            request.AddParameter("owner", owner);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<AccountInformation>(response.Content!);
                Thread.Sleep(100);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                if (httpException.Message == "Request failed with status code BadRequest")
                {
                    Font.SetTextToRed("The above information did not find an account. Please, try again.");
                    return null;
                }
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<List<MintsAndTotal>> GetUserMintedNfts(string apiKey, int accountId)
        {
            var allDataMintsAndTotal = new List<MintsAndTotal>();
            var request = new RestRequest("/api/v3/user/nft/mints");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("limit", 50);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<MintsAndTotal>(response.Content!);
                var total = data.totalNum;
                allDataMintsAndTotal.Add(data);
                while (total > 50)
                {
                    total = total - 50;
                    var createdAt = allDataMintsAndTotal.LastOrDefault().mints.LastOrDefault().createdAt;
                    request.AddOrUpdateParameter("end", createdAt);
                    response = await _client.GetAsync(request);
                    var moreData = JsonConvert.DeserializeObject<MintsAndTotal>(response.Content!);
                    allDataMintsAndTotal.Add(moreData);
                }
                return allDataMintsAndTotal;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<List<NftData>> GetUserMintedNftsWithCollection(string apiKey, int accountId, string collectionId)
        {
            Font.SetTextToBlue("Starting...");
            Font.SetTextToYellow("This may take a while depending on the minter and collection.");
            var allDataMintsAndTotal = new List<MintsAndTotal>();
            var allDataMintsAndTotalInCollection = new List<NftData>();
            var request = new RestRequest("/api/v3/user/nft/mints");
            request.AddHeader("X-API-KEY", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("limit", 50);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<MintsAndTotal>(response.Content!);
                var total = data.totalNum;
                allDataMintsAndTotal.Add(data);
                while (total >= 50)
                {
                    total = total - 50;
                    var createdAt = allDataMintsAndTotal.LastOrDefault().mints.LastOrDefault().createdAt;
                    request.AddOrUpdateParameter("end", createdAt);
                    response = await _client.GetAsync(request);
                    var moreData = JsonConvert.DeserializeObject<MintsAndTotal>(response.Content!);
                    allDataMintsAndTotal.Add(moreData);
                }
                foreach (var item in allDataMintsAndTotal)
                {
                    var mints = item.mints;
                    foreach (var mint in mints)
                    {
                        var nftDataInformationList = await GetNftInformationFromNftData(apiKey, mint.nftData);
                        foreach (var nftDataInformation in nftDataInformationList)
                        {
                            if (nftDataInformation.tokenAddress.ToLower() == collectionId)
                            {
                                allDataMintsAndTotalInCollection.Add(nftDataInformation);
                            }
                            else
                            {
                                Console.WriteLine(nftDataInformation.nftId);
                                Console.WriteLine(nftDataInformation.minter);
                                Console.WriteLine(nftDataInformation.tokenAddress);
                            }
                        }
                    }
                }
                return allDataMintsAndTotalInCollection;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting TokenId: {httpException.Message}");
                return null;
            }
        }

        public async Task<TransferFeeOffchainFee> GetOffChainTransferFee(string apiKey, int accountId, int requestType, string feeToken, string amount)
        {
            var request = new RestRequest("api/v3/user/offchainFee");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("requestType", requestType);
            request.AddParameter("tokenSymbol", feeToken);
            request.AddParameter("amount", amount);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<TransferFeeOffchainFee>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting transfer off chain fee: {httpException.Message}");
                return null;
            }
        }

        public async Task<List<NftData>> GetNftInformationFromNftData(string apiKey, string nftData)
        {
            var request = new RestRequest("/api/v3/nft/info/nfts");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("nftDatas", nftData);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<List<NftData>>(response.Content!);
                Thread.Sleep(100);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting transfer off chain fee: {httpException.Message}");
                return null;
            }
        }

        public async Task<bool> CheckBanishTextFile(string toAddressInitial, string toAddress, string loopringApiKey)
        {
            List<string> banishAddresses = new List<string>();
            bool banned;
            string banishAddress;
            using (StreamReader sr = new StreamReader("./Banish.txt"))
            {
                while ((banishAddress = sr.ReadLine()) != null)
                {
                    if (banishAddress.Contains(".eth"))
                    {
                        Thread.Sleep(100);
                        var varHexAddress = await GetHexAddress(loopringApiKey, banishAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            banishAddress = varHexAddress.data.ToLower().Trim();
                        }
                        else
                        {
                            //invalidAddress.Add(toAddressInitial);
                            Console.WriteLine($"Invalid address: {banishAddress}. Could not find an associated wallet.");
                            //continue;
                        }
                    }
                    banishAddresses.Add(banishAddress.ToLower().Trim());
                }
            }
            if (banishAddresses.Contains(toAddress) || banishAddresses.Contains(toAddressInitial.Trim().ToLower()))
            {
                banned = true;
            }
            else
            {
                banned = false;
            }
            return banned;
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}
