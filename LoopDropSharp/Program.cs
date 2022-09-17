using LoopDropSharp;
using LoopDropSharp.Helpers;
using LoopDropSharp.Models;
using Microsoft.Extensions.Configuration;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Type = LoopDropSharp.Type;

// load services
ILoopringService loopringService = new LoopringService();
IEthereumService ethereumService = new EthereumService();
INftMetadataService nftMetadataService = new NftMetadataService("https://loopring.mypinata.cloud/ipfs/");

//load global variables 
List<MintsAndTotal> userMintsAndTotalList;
List<NftHoldersAndTotal> nftHoldersAndTotalList;
List<string> invalidAddress = new List<string>();
List<string> validAddress = new List<string>();
List<string> banishAddress = new List<string>();
NftBalance userNftToken;
string nftData;
string nftAmount;
int nftTokenId;
string userResponseOnWalletAddressDisplay;
string userResponseReadyToMoveOn;
string nftMetadataLink = "";
string toAddressInitial;
NftMetadata nftMetadata = new NftMetadata();
//Console.TreatControlCAsInput = true;

//Settings loaded from the appsettings.json fileq
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("./appsettings.json")
    .AddEnvironmentVariables()
    .Build();
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

string loopringApiKey = settings.LoopringApiKey;//loopring api key KEEP PRIVATE
string loopringPrivateKey = settings.LoopringPrivateKey; //loopring private key KEEP PRIVATE
var MMorGMEPrivateKey = settings.MMorGMEPrivateKey; //metamask or gamestop private key KEEP PRIVATE
var fromAddress = settings.LoopringAddress; //your loopring address
var fromAccountId = settings.LoopringAccountId; //your loopring account id
var validUntil = settings.ValidUntil; //the examples seem to use this number
var maxFeeTokenId = settings.MaxFeeTokenId; //0 should be for ETH, 1 is for LRC
var exchange = settings.Exchange; //loopring exchange address, shouldn't need to change this,
int toAccountId = 0; //leave this as 0 DO NOT CHANGE
var fileName = "Input.txt";
bool contains = false;
Menu.BannerForLoopDropSharp();
userResponseReadyToMoveOn = Utils.CheckYes();
while (userResponseReadyToMoveOn == "yes")
{
    var minterAndCollection = new MinterAndCollection();

    var allUtilities = Menu.MenuForLoopDropSharp();
    var userResponseOnUtility = Utils.CheckUtilityNumber(10);

    switch (userResponseOnUtility)
    {
        #region case 0
        case "0":
            Font.SetTextToBlue(allUtilities.ElementAt(0).Value);
            Font.SetTextToPurple("     Is Nft Data and same as Nft Id?");
            Font.SetTextToWhite("\t Nft Data is not the same as an Nft Id, but Nft Data comes from an Nft Id.");
            Font.SetTextToWhite("\tYou can find the Nft Id on any Loopring Explorer and typically any marketplace.");
            Console.WriteLine();
            Font.SetTextToPurple("     What is Nft Data used for?");
            Font.SetTextToWhite("\t Nft Data is used to find your unique Nft on the blockchain. ");
            Font.SetTextToWhite("\t Once you have this you are able to do lookups associated to the Nft and transfer the Nft.");
            Console.WriteLine();
            Font.SetTextToPurple("     What wallet can I use to Airdrop?");
            Font.SetTextToWhite("\t You can use a MetaMask wallet or a GameStop Wallet.");
            Font.SetTextToWhite("\t For those wondering, unfortunately, you cannot use a Loopring wallet to perform Airdrops.");
            break;
        #endregion
        #region case 1
        case "1":
            NftData nftdataRequestForNftData;
            Font.SetTextToBlue(allUtilities.ElementAt(1).Value);
            Font.SetTextToBlue("Here you will get Nft Data from an Nft.");
            Font.SetTextToDarkGray("Find your Nft on lexplorer.io or explorer.loopring.io and find the below.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                Font.SetTextToBlue("Enter in the Nft Id");
                string nftIdForNftData = Utils.ReadLineWarningNoNulls("Enter in the Nft Id");
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();

                minterAndCollection.minter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);

                nftdataRequestForNftData = await loopringService.GetNftData(settings.LoopringApiKey, nftIdForNftData, minterAndCollection.minter, minterAndCollection.TokenId);  //the nft tokenId, not the nftId
            } while (nftdataRequestForNftData == null);

            Font.SetTextToBlueInline($"This Nft's Nft Data is: "); 
            Font.SetTextToGreen($"{nftdataRequestForNftData.nftData}");
            break;
        #endregion case 1
        #region case 2
        case "2":
            Font.SetTextToBlue(allUtilities.ElementAt(2).Value);
            Font.SetTextToBlue("Here you will provide the Nft Id and get the associated Nft Data." );
            Font.SetTextToYellow("All Nft Ids have to be from the same collection.");
            Font.SetTextToWhite("Let's get started.");
            var howManyWallets = Utils.CheckInputDotTxt();
            string userResponseOnNftData;

            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                string nftIdFromText;
                nftdataRequestForNftData = null;
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                var counter = 0;
                while ((nftIdFromText = sr.ReadLine()) != null)
                {
                    do {
                        var nftIdForNftData = nftIdFromText;
                        if (minterAndCollection.minter.Contains(".eth"))
                        {
                            var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, minterAndCollection.minter);
                            if (!String.IsNullOrEmpty(varHexAddress.data))
                            {
                                minterAndCollection.minter = varHexAddress.data;
                            }
                            else
                            {
                                Console.WriteLine($"{minterAndCollection.minter} is an invalid address");
                                continue;
                            }
                        }

                        nftdataRequestForNftData = await loopringService.GetNftData(settings.LoopringApiKey, nftIdForNftData, minterAndCollection.minter, minterAndCollection.TokenId);
                    } while (nftdataRequestForNftData == null);
                    if (counter == 0)
                    {
                        Font.SetTextToBlue("Nft Data:");
                    }
                    counter++;
                    Font.SetTextToGreen(nftdataRequestForNftData.nftData);
                }
            }
            break;
        #endregion case 2
        #region case 3
        case "3":
            userResponseOnWalletAddressDisplay = "";
            nftdataRequestForNftData = null;
            string userResponseOnMinter;
            string responseOnMinter;
            AccountInformation responseOnAccountId = new AccountInformation();
            Font.SetTextToBlue(allUtilities.ElementAt(3).Value);
            Font.SetTextToBlue("Here you will get all the Nft Data from the Nfts in a Collection.");
            Font.SetTextToWhite("Let's get started.");
            do {
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                responseOnMinter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);
                responseOnAccountId = await loopringService.GetUserAccountInformationFromOwner(responseOnMinter);
            }
            while (responseOnAccountId == null);
            var nftDataList = await loopringService.GetUserMintedNftsWithCollection(settings.LoopringApiKey, responseOnAccountId.accountId, minterAndCollection.TokenId);
            Font.SetTextToGreen($"{minterAndCollection.minter} has {nftDataList.Count()} mints in this Collection.");

            foreach (var nftDataSingle in nftDataList)
            {
                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, responseOnAccountId.accountId, nftDataSingle.nftData);
                nftTokenId = userNftToken.data[0].tokenId;
                nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);
                Font.SetTextToGreen($"{nftMetadata.name}, {nftDataSingle.nftData}");
            }
            break;
        #endregion case 3
        #region case 4
        case "4":
            string userResponseOnFileSetup = "";
            Font.SetTextToBlue(allUtilities.ElementAt(4).Value);
            Font.SetTextToBlue("Here you will get all the wallet addresses that hold the given Nft Data.");
            Font.SetTextToWhite("Let's get started.");
            Font.SetTextToGreen("Is this for one or many Nfts?");
            var userResponseOnMany = Utils.CheckOneOrMany();

            if (userResponseOnMany == "one")
            {
                Font.SetTextToGreen("What is the Nft Data?");
                nftData = Utils.ReadLineWarningNoNulls("what is the Nft Data?");
                nftHoldersAndTotalList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, nftData);  //the nft tokenId, not the 
                foreach (var nftHoldersAndTotalSingle in nftHoldersAndTotalList)
                {
                    if (nftHoldersAndTotalSingle == nftHoldersAndTotalList.FirstOrDefault())
                    {
                        Font.SetTextToGreen($"NftData {nftData} has {nftHoldersAndTotalSingle.totalNum} total holders.");
                        Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                    };
                    foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                    {
                        var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());  //the nft tokenId, not the 
                        Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {item.amount}");
                    }
                }
            }
            else if (userResponseOnMany == "many")
            {
                howManyWallets = Utils.CheckInputDotTxt();

                Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");

                using (StreamReader sr = new StreamReader("./Input.txt"))
                {
                    Font.SetTextToGreen("Would you like to view just the wallet addresses?");
                    userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo();
                    while ((nftData = sr.ReadLine()) != null)
                    {
                        nftHoldersAndTotalList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, nftData);  //the nft tokenId, not the 
                        foreach (var nftHoldersAndTotalSingle in nftHoldersAndTotalList)
                        {
                            if (userResponseOnWalletAddressDisplay == "yes")
                            {
                                foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                                {
                                    var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());  //the nft tokenId, not the 
                                    Console.WriteLine(userAccountInformation.owner);
                                }
                            }
                            else if (userResponseOnWalletAddressDisplay == "no")
                            {
                                if (nftHoldersAndTotalSingle == nftHoldersAndTotalList.FirstOrDefault())
                                {
                                    var minterFromNftDatas = await loopringService.GetNftInformationFromNftData(settings.LoopringApiKey, nftData);
                                    var accountIdFromMinter = await loopringService.GetUserAccountInformationFromOwner(minterFromNftDatas[0].minter);
                                    userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, accountIdFromMinter.accountId, nftData);
                                    var nftmetaDataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                                    nftMetadata = await nftMetadataService.GetMetadata(nftmetaDataLink);

                                    Font.SetTextToGreen($"{nftMetadata.name}, {nftMetadata.description}, {nftData} has {nftHoldersAndTotalSingle.totalNum} total holders.");
                                    Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                                };
                                foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                                {
                                    var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());  //the nft tokenId, not the 
                                    Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {item.amount}");
                                }
                            }
                        }
                    }
                }
            }
            break;
        #endregion case 4
        #region case 5
        case "5":
            Font.SetTextToBlue(allUtilities.ElementAt(5).Value);
            Font.SetTextToBlue("Here you will get all the wallet addresses that hold an account's Nfts.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                Font.SetTextToGreen("Who is the minter?");
                userResponseOnMinter = Utils.ReadLineWarningNoNulls("Who is the minter?");
                responseOnMinter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, userResponseOnMinter);
                responseOnAccountId = await loopringService.GetUserAccountInformationFromOwner(responseOnMinter);
            }
            while (responseOnAccountId == null);
            Font.SetTextToGreen("Would you like to view just the wallet addresses?");
            userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo();
            userMintsAndTotalList = await loopringService.GetUserMintedNfts(settings.LoopringApiKey, responseOnAccountId.accountId);
            Font.SetTextToGreen($"{userResponseOnMinter} has {userMintsAndTotalList.First().totalNum} mints");
            if (userResponseOnWalletAddressDisplay == "yes")
            {
                foreach (var userMintsAndTotalSingle in userMintsAndTotalList)
                {
                    var mints = userMintsAndTotalSingle.mints;
                    foreach (var mint in mints)
                    {
                        nftData = mint.nftData;
                        var nftHoldersAndTotal = await loopringService.GetNftHolders(settings.LoopringApiKey, nftData);

                        foreach (var item in nftHoldersAndTotal.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            Font.SetTextToGreen($"{userAccountInformation.owner}");
                        }
                    }
                }
            }
            else if (userResponseOnWalletAddressDisplay == "no")
            {
                foreach (var userMintsAndTotalSingle in userMintsAndTotalList)
                {
                    var mints = userMintsAndTotalSingle.mints;
                    foreach (var mint in mints)
                    {
                        nftData = mint.nftData;
                        var nftHoldersAndTotal = await loopringService.GetNftHolders(settings.LoopringApiKey, nftData);

                        userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, mint.accountId, nftData);
                        nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                        nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

                        Font.SetTextToBlue($"{nftMetadata.name}");
                        Font.SetTextToBlue($"{nftMetadata.description}");
                        Font.SetTextToBlueInline($"NftData: ");
                        Font.SetTextToGreen(nftData);
                        Font.SetTextToBlueInline($"Total Holders: ");
                        Font.SetTextToGreen(nftHoldersAndTotal.totalNum.ToString());
                        Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                        foreach (var item in nftHoldersAndTotal.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {item.amount}");
                        }
                        Font.SetTextToWhite("------------------------------------------");
                    }
                }
            }
            break;
        #endregion case 5
        #region case 6
        case "6":
            Font.SetTextToBlue(allUtilities.ElementAt(6).Value);
            Font.SetTextToBlue("Here you will provide the Nft Data and get the Nft Holders who own all Nfts provided.");
            Font.SetTextToWhite("Let's get started.");
            var howManyNfts = Utils.CheckInputDotTxt();
            Font.SetTextToPurple($"You provided the following {howManyNfts} Nfts.");

            var allNftData = new List<string>();
            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((nftData = sr.ReadLine()) != null)
                {
                    allNftData.Add(nftData);
                }
            }
            var originalWalletHolderList = new List<string>();
            var placeHolderWalletHolderList = new List<string>();
            var filteredWallettHolderList = new List<string>(placeHolderWalletHolderList);
            foreach (var data in allNftData)
            {
                if (allNftData.FirstOrDefault() == data)
                {
                    nftHoldersAndTotalList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, data);  //the nft tokenId, not the 
                    foreach (var nftHoldersAndTotalSingle in nftHoldersAndTotalList)
                    {
                        if (nftHoldersAndTotalSingle == nftHoldersAndTotalList.FirstOrDefault())
                        {
                            var minterFromNftDatas = await loopringService.GetNftInformationFromNftData(settings.LoopringApiKey, data);
                            var accountIdFromMinter = await loopringService.GetUserAccountInformationFromOwner(minterFromNftDatas[0].minter);
                            userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, accountIdFromMinter.accountId, data);
                            var nftmetaDataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                            nftMetadata = await nftMetadataService.GetMetadata(nftmetaDataLink);

                            Font.SetTextToBlue($"{nftMetadata.name}");
                        };
                        foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            originalWalletHolderList.Add(userAccountInformation.owner);
                        }
                    }
                }
                else
                {
                    nftHoldersAndTotalList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, data);
                    foreach (var nftHoldersAndTotalSingle in nftHoldersAndTotalList)
                    {
                        if (nftHoldersAndTotalSingle == nftHoldersAndTotalList.FirstOrDefault())
                        {
                            var minterFromNftDatas = await loopringService.GetNftInformationFromNftData(settings.LoopringApiKey, data);
                            var accountIdFromMinter = await loopringService.GetUserAccountInformationFromOwner(minterFromNftDatas[0].minter);
                            userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, accountIdFromMinter.accountId, data);
                            var nftmetaDataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                            nftMetadata = await nftMetadataService.GetMetadata(nftmetaDataLink);

                            Font.SetTextToBlue($"{nftMetadata.name}");
                        };
                        foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());

                            if (originalWalletHolderList.Contains(userAccountInformation.owner) && filteredWallettHolderList.Count() == 0)
                            {
                                placeHolderWalletHolderList.Add(userAccountInformation.owner);
                            }
                            else if (filteredWallettHolderList.Contains(userAccountInformation.owner))
                            {
                                placeHolderWalletHolderList.Add(userAccountInformation.owner);
                            }

                        }
                        var filteredWalletHolder = placeHolderWalletHolderList.ConvertAll<string>(x => x.ToString());
                        filteredWallettHolderList = filteredWalletHolder;
                        placeHolderWalletHolderList.Clear();
                        //right now all looks like it will work once the placeholderwalletholderlist stops being a reference type. need to make them all a value type. 
                    }
                }
            }
            Font.SetTextToPurple($"The following {filteredWallettHolderList.Count()} wallets have all above Nfts. ");
            foreach (var wallet in filteredWallettHolderList)
            {
                Console.WriteLine($"{wallet}");
            }

            break;
        #endregion case 6
        #region case 7
        case "7":
            var nftdataRequest = new NftData();
            Font.SetTextToBlue(allUtilities.ElementAt(7).Value);
            Font.SetTextToBlue("Here you will drop one Nft to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt();
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlueInline("Do you know your Nft's Nft Data?");
            Font.SetTextToYellow(" This is not the same as the Nft Id.");
            userResponseOnNftData = Utils.CheckYesOrNo();
            if (userResponseOnNftData == "yes")
            {
                Font.SetTextToBlue("Enter the NftData");
                nftData = Console.ReadLine();
                try
                {
                    userNftToken = await loopringService.GetTokenIdWithCheck(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                    nftTokenId = userNftToken.data[0].tokenId;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                Font.SetTextToBlue("Find your Nft on lexplorer.io or explorer.loopring.io.");
                Console.WriteLine("You should see the Nft Id, Minter, and Token/Collection Address");
            do {
                    Font.SetTextToBlue("Enter in the Nft Id");
                    string nftId = Utils.ReadLineWarningNoNulls("Enter in the Nft Id");
                    minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                    minterAndCollection.minter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);
                    nftdataRequest = await loopringService.GetNftData(settings.LoopringApiKey, nftId, minterAndCollection.minter, minterAndCollection.TokenId);
            } while (nftdataRequest == null) ;
            nftData = nftdataRequest.nftData;

                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                nftTokenId = userNftToken.data[0].tokenId;

            }
            nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
            nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

            Font.SetTextToBlue($"How many of '{nftMetadata.name}' do you want to transfer to each address?");
            nftAmount = Utils.CheckNftSendAmount(howManyWallets, userNftToken.data[0].total);

            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((toAddressInitial = sr.ReadLine()) != null)
                {
                    //remove whitespace after wallet address if it exists.
                    var toAddress = toAddressInitial.ToLower().Trim();

                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

                    //check for ens and convert to long wallet address if so
                    if (toAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, toAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            toAddress = varHexAddress.data.ToLower().Trim();
                        }
                        else
                        {
                            invalidAddress.Add(toAddressInitial);
                            Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, toAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                    Utils.ParseHexUnsigned(exchange),
                                    (BigInteger) fromAccountId,
                                    (BigInteger) toAccountId,
                                    (BigInteger) nftTokenId,
                                    BigInteger.Parse(nftAmount),
                                    (BigInteger) maxFeeTokenId,
                                    BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                    Utils.ParseHexUnsigned(toAddress),
                                    (BigInteger) 0,
                                    (BigInteger) 0,
                                    (BigInteger) validUntil,
                                    (BigInteger) storageId.offchainId
                    };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeName] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedData.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = toAddress},
                                    new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                    new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                    new MemberValue {TypeName = "uint32", Value = validUntil},
                                    new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: nftData
                        );

                    Console.WriteLine(nftTransferResponse);
                    validAddress.Add(toAddressInitial);

                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAudit(validAddress, invalidAddress, banishAddress, nftMetadata.name);
            }
            break;
        #endregion case 7
        #region case 8
        case "8":
            Font.SetTextToBlue(allUtilities.ElementAt(8).Value);
            Font.SetTextToBlue($"Here you will drop many Nfts to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxtTwoInputs();
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("How many of each Nft do you want to transfer to each address?");
            nftAmount = Utils.ReadLineWarningNoNullsForceInt("How many of each Nft do you want to transfer to each address?");

            using (StreamReader sr = new StreamReader("./Input.txt"))
            {

            string walletAddressLine;


            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    string[] walletAddressLineArray = walletAddressLine.Split(',');
                    toAddressInitial = walletAddressLineArray[0].Trim();
                    var toAddress = toAddressInitial.ToLower().Trim();
                    nftData = walletAddressLineArray[1].ToLower().Trim();

                    userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);  //the nft tokenId, not the nftId
                    if (userNftToken.totalNum == 0)
                    {
                        invalidAddress.Add(walletAddressLine);
                        Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                        continue;
                    }
                    nftTokenId = userNftToken.data[0].tokenId;

                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

                    //check for ens and convert to long wallet address if so
                    if (toAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, toAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            toAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(toAddress);
                            Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }
                    
                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, toAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                    Utils.ParseHexUnsigned(exchange),
                                    (BigInteger) fromAccountId,
                                    (BigInteger) toAccountId,
                                    (BigInteger) nftTokenId,
                                    BigInteger.Parse(nftAmount),
                                    (BigInteger) maxFeeTokenId,
                                    BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                    Utils.ParseHexUnsigned(toAddress),
                                    (BigInteger) 0,
                                    (BigInteger) 0,
                                    (BigInteger) validUntil,
                                    (BigInteger) storageId.offchainId
                    };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeName] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedData.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = toAddress},
                                    new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                    new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                    new MemberValue {TypeName = "uint32", Value = validUntil},
                                    new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: nftData
                        );

                    Console.WriteLine(nftTransferResponse);
                    validAddress.Add(toAddressInitial);

                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditAmbiguous(validAddress, invalidAddress, banishAddress);
            }
            break;
        #endregion case 8
        #region case 9
        case "9":
            //Token id of 1 for LRC, token id of 0 for ETH
            decimal amountToTransfer = 0m;
            string transferMemo = "";
            int transferTokenId = 3;
            string transferTokenSymbol = "";
            Font.SetTextToBlue(allUtilities.ElementAt(9).Value);
            Font.SetTextToPurple($"Here you will airdrop LRC/ETH to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt();
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            Font.SetTextToBlue("Amount to transfer per address?");
            amountToTransfer = Utils.ReadLineWarningNoNullsForceDecimal("Amount to transfer per address?");
            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.ToLower().Trim();

            if (transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if (transferTokenId == 0)
            {
                transferTokenSymbol = "ETH";
            }

            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    toAddressInitial = walletAddressLine;
                    var transferToAddress = walletAddressLine.ToLower().Trim();

                    //check for ens and convert to long wallet address if so
                    if (transferToAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, transferToAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            transferToAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(transferToAddress);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, transferToAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }


                    var amount = (amountToTransfer * 1000000000000000000m).ToString("0");
                    var transferFeeAmountResult = await loopringService.GetOffChainTransferFee(loopringApiKey, fromAccountId, 3, transferTokenSymbol, amount); //3 is the request type for crypto transfers
                    var feeAmount = transferFeeAmountResult.fees.Where(w => w.token == transferTokenSymbol).First().fee;
                    var transferStorageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, transferTokenId);

                    TransferRequest req = new TransferRequest()
                    {
                        exchange = exchange,
                        maxFee = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = feeAmount
                        },
                        token = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = amount
                        },
                        payeeAddr = transferToAddress,
                        payerAddr = fromAddress,
                        payeeId = 0,
                        payerId = fromAccountId,
                        storageId = transferStorageId.offchainId,
                        validUntil = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                        tokenName = transferTokenSymbol,
                        tokenFeeName = transferTokenSymbol
                    };

                    BigInteger[] eddsaSignatureinputs = {
                Utils.ParseHexUnsigned(req.exchange),
                (BigInteger)req.payerId,
                (BigInteger)req.payeeId,
                (BigInteger)req.token.tokenId,
                BigInteger.Parse(req.token.volume),
                (BigInteger)req.maxFee.tokenId,
                BigInteger.Parse(req.maxFee.volume),
                Utils.ParseHexUnsigned(req.payeeAddr),
                0,
                0,
                (BigInteger)req.validUntil,
                (BigInteger)req.storageId
            };

                    Poseidon poseidonTransfer = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonTransferHash = poseidonTransfer.CalculatePoseidonHash(eddsaSignatureinputs);
                    Eddsa eddsaTransfer = new Eddsa(poseidonTransferHash, loopringPrivateKey);
                    string transferEddsaSignature = eddsaTransfer.Sign();

                    //Calculate ecdsa
                    string primaryTypeNameTransfer = "Transfer";
                    TypedData eip712TypedDataTransfer = new TypedData();
                    eip712TypedDataTransfer.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedDataTransfer.PrimaryType = primaryTypeNameTransfer;
                    eip712TypedDataTransfer.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeNameTransfer] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedDataTransfer.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = transferToAddress},
                                    new MemberValue {TypeName = "uint16", Value = req.token.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.token.volume)},
                                    new MemberValue {TypeName = "uint16", Value = req.maxFee.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.maxFee.volume)},
                                    new MemberValue {TypeName = "uint32", Value = req.validUntil},
                                    new MemberValue {TypeName = "uint32", Value = req.storageId},
                                };

                    TransferTypedData typedDataTransfer = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = transferToAddress,
                            tokenID = req.token.tokenId,
                            amount = req.token.volume,
                            feeTokenID = req.maxFee.tokenId,
                            maxFee = req.maxFee.volume,
                            validUntil = (int)req.validUntil,
                            storageID = req.storageId
                        },
                        primaryType = primaryTypeNameTransfer,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signerTransfer = new Eip712TypedDataSigner();
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedDataTransfer = signerTransfer.EncodeTypedData(eip712TypedDataTransfer);
                    var ECDRSASignatureTransfer = ethECKeyTransfer.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedDataTransfer));
                    var serializedECDRSASignatureTransfer = EthECDSASignature.CreateStringSignature(ECDRSASignatureTransfer);
                    var transferEcdsaSignature = serializedECDRSASignatureTransfer + "0" + (int)2;

                    var tokenTransferResult = await loopringService.SubmitTokenTransfer(
                        loopringApiKey,
                        exchange,
                        fromAccountId,
                        fromAddress,
                        0,
                        transferToAddress,
                        req.token.tokenId,
                        req.token.volume,
                        req.maxFee.tokenId,
                        req.maxFee.volume,
                        req.storageId,
                        req.validUntil,
                        transferEddsaSignature,
                        transferEcdsaSignature,
                        transferMemo);
                    Console.WriteLine(tokenTransferResult);
                    validAddress.Add(transferToAddress);
                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditCrypto(validAddress, invalidAddress, banishAddress, transferTokenSymbol);
            }
            break;
        #endregion case 9
        #region case 10
        case "10":
            //Token id of 1 for LRC, token id of 0 for ETH
            amountToTransfer = 0m;
            transferMemo = "";
            transferTokenId = 3;
            transferTokenSymbol = "";
            Font.SetTextToBlue(allUtilities.ElementAt(10).Value);
            Font.SetTextToBlue($"Here you will airdrop LRC/ETH to many users with different amounts.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt();
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            //Font.SetTextToBlue("Amount to transfer per address?");
            //amountToTransfer = Utils.ReadLineWarningNoNullsForceDecimal("Amount to transfer per address?");
            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.ToLower().Trim();

            if (transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if (transferTokenId == 0)
            {
                transferTokenSymbol = "ETH";
            }

            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    string[] walletAddressLineArray = walletAddressLine.Split(',');
                    var transferToAddress = walletAddressLineArray[0].ToLower().Trim();
                    toAddressInitial = walletAddressLineArray[0].Trim();
                    amountToTransfer = decimal.Parse(walletAddressLineArray[1].ToLower().Trim());

                    //check for ens and convert to long wallet address if so
                    if (transferToAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, transferToAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            transferToAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(transferToAddress);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, transferToAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    var amount = (amountToTransfer * 1000000000000000000m).ToString("0");
                    var transferFeeAmountResult = await loopringService.GetOffChainTransferFee(loopringApiKey, fromAccountId, 3, transferTokenSymbol, amount); //3 is the request type for crypto transfers
                    var feeAmount = transferFeeAmountResult.fees.Where(w => w.token == transferTokenSymbol).First().fee;
                    var transferStorageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, transferTokenId);

                    TransferRequest req = new TransferRequest()
                    {
                        exchange = exchange,
                        maxFee = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = feeAmount
                        },
                        token = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = amount
                        },
                        payeeAddr = transferToAddress,
                        payerAddr = fromAddress,
                        payeeId = 0,
                        payerId = fromAccountId,
                        storageId = transferStorageId.offchainId,
                        validUntil = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                        tokenName = transferTokenSymbol,
                        tokenFeeName = transferTokenSymbol
                    };

                    BigInteger[] eddsaSignatureinputs = {
                Utils.ParseHexUnsigned(req.exchange),
                (BigInteger)req.payerId,
                (BigInteger)req.payeeId,
                (BigInteger)req.token.tokenId,
                BigInteger.Parse(req.token.volume),
                (BigInteger)req.maxFee.tokenId,
                BigInteger.Parse(req.maxFee.volume),
                Utils.ParseHexUnsigned(req.payeeAddr),
                0,
                0,
                (BigInteger)req.validUntil,
                (BigInteger)req.storageId
            };

                    Poseidon poseidonTransfer = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonTransferHash = poseidonTransfer.CalculatePoseidonHash(eddsaSignatureinputs);
                    Eddsa eddsaTransfer = new Eddsa(poseidonTransferHash, loopringPrivateKey);
                    string transferEddsaSignature = eddsaTransfer.Sign();

                    //Calculate ecdsa
                    string primaryTypeNameTransfer = "Transfer";
                    TypedData eip712TypedDataTransfer = new TypedData();
                    eip712TypedDataTransfer.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedDataTransfer.PrimaryType = primaryTypeNameTransfer;
                    eip712TypedDataTransfer.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeNameTransfer] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedDataTransfer.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = transferToAddress},
                                    new MemberValue {TypeName = "uint16", Value = req.token.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.token.volume)},
                                    new MemberValue {TypeName = "uint16", Value = req.maxFee.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.maxFee.volume)},
                                    new MemberValue {TypeName = "uint32", Value = req.validUntil},
                                    new MemberValue {TypeName = "uint32", Value = req.storageId},
                                };

                    TransferTypedData typedDataTransfer = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = transferToAddress,
                            tokenID = req.token.tokenId,
                            amount = req.token.volume,
                            feeTokenID = req.maxFee.tokenId,
                            maxFee = req.maxFee.volume,
                            validUntil = (int)req.validUntil,
                            storageID = req.storageId
                        },
                        primaryType = primaryTypeNameTransfer,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signerTransfer = new Eip712TypedDataSigner();
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedDataTransfer = signerTransfer.EncodeTypedData(eip712TypedDataTransfer);
                    var ECDRSASignatureTransfer = ethECKeyTransfer.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedDataTransfer));
                    var serializedECDRSASignatureTransfer = EthECDSASignature.CreateStringSignature(ECDRSASignatureTransfer);
                    var transferEcdsaSignature = serializedECDRSASignatureTransfer + "0" + (int)2;

                    var tokenTransferResult = await loopringService.SubmitTokenTransfer(
                        loopringApiKey,
                        exchange,
                        fromAccountId,
                        fromAddress,
                        0,
                        transferToAddress,
                        req.token.tokenId,
                        req.token.volume,
                        req.maxFee.tokenId,
                        req.maxFee.volume,
                        req.storageId,
                        req.validUntil,
                        transferEddsaSignature,
                        transferEcdsaSignature,
                        transferMemo);
                    Console.WriteLine(tokenTransferResult);
                    validAddress.Add(transferToAddress);
                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditCrypto(validAddress, invalidAddress, banishAddress, transferTokenSymbol);
            }
            break;
        #endregion case 10
    }
    validAddress.Clear();
    invalidAddress.Clear();
    banishAddress.Clear();
    Font.SetTextToGreen("Start a new functionality?");
    userResponseReadyToMoveOn = Utils.CheckYesOrNo();
}
Menu.FooterForLoopDropSharp();
Console.ReadLine();