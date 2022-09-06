using LoopDropSharp;
using Microsoft.Extensions.Configuration;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Numerics;
using Type = LoopDropSharp.Type;

string userResponseReadyToMoveOn = "";
string userResponseOnUtility = "";
string userResponseOnNftData = "";
string nftData = "";
string nftAmount;
string userResponseOnMany = "";
string userResponseOnWalletAddressDisplay = "";
string userResponseOnWalletSetup = "";
string tokenAddress = "";
int userResponseOnAccountId = 0;
List<MintsAndTotal> userMintsAndTotalList;
NftBalance userNftToken;
List<NftHoldersAndTotal> nftHoldersAndTotalList;
int nftTokenId;
ILoopringService loopringService = new LoopringService();
IEthereumService ethereumService = new EthereumService();
INftMetadataService nftMetadataService = new NftMetadataService("https://loopring.mypinata.cloud/ipfs/");
List<string> invalidAddress = new List<string>();
List<string> validAddress = new List<string>();

//Settings loaded from the appsettings.json file
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile(".\\appsettings.json")
    .AddEnvironmentVariables()
    .Build();
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

string loopringApiKey = settings.LoopringApiKey;//loopring api key KEEP PRIVATE
string loopringPrivateKey = settings.LoopringPrivateKey; //loopring private key KEEP PRIVATE
var metamaskPrivateKey = settings.MetamaskPrivateKey; //metamask private key KEEP PRIVATE
var fromAddress = settings.LoopringAddress; //your loopring address
var fromAccountId = settings.LoopringAccountId; //your loopring account id
var validUntil = settings.ValidUntil; //the examples seem to use this number
var maxFeeTokenId = settings.MaxFeeTokenId; //0 should be for ETH, 1 is for LRC
var exchange = settings.Exchange; //loopring exchange address, shouldn't need to change this,
int toAccountId = 0; //leave this as 0 DO NOT CHANGE

// Initial Information and Questions
Font.SetTextToDarkBlue("Welcome to LoopDropSharp.");
Font.SetTextToBlue("If you have any questions, start at https://cobmin.io/posts/Airdrop-Nfts-on-Loopring");
Font.SetTextToYellow("Before you airdrop, be sure to setup your appsetting.json and necessary text files.");
Console.WriteLine("Find information on the files in the README at https://github.com/fudgebucket27/LoopDropSharp/blob/master/README.md");
Console.WriteLine("After they are setup, answer the following questions to get started.");
Font.SetTextToBlue("Ready to start?");
userResponseReadyToMoveOn = Utils.CheckYes(userResponseReadyToMoveOn);
while (userResponseReadyToMoveOn == "yes")
{
    // Menu of the Utilities. need to be sure to change numbers here and in the CheckUtilityNumber
    Font.SetTextToBlue("This airdrop application can currently perform the following:");
    Console.WriteLine("\t 1. Airdrop the same NFT to any users.");
    Console.WriteLine("\t 2. Airdrop unique NFTs to any users.");
    Console.WriteLine("\t 3. Find Nft Data for a single Nft.");
    Console.WriteLine("\t 4. Find Nft Holders from Nft Data.");
    Console.WriteLine("\t 5. Find an accounts Nft wallet holders.");
    Console.WriteLine("\t 6. Find all Nft Data from a Collection.");
    Console.WriteLine("\t 7. Airdrop LRC/ETH to any users.");
    Font.SetTextToBlue("Which would you like to do?");
    userResponseOnUtility = Utils.CheckUtilityNumber(userResponseOnUtility);
    switch (userResponseOnUtility)
    {
        #region case 1
        case "1":
            Font.SetTextToBlue("Airdrop the same NFT to any users.");
            Console.WriteLine("Here you will drop one Nft to many users. Check the README for walletAddress.txt setup.");
            Console.WriteLine("Let's get started.");
            var howManyWallets = Utils.CheckWalletAddresstxt();
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("Do you know your Nft's NftData (not the NftId)?");
            userResponseOnNftData = Utils.CheckYesOrNo(userResponseOnNftData.ToLower());
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
                Font.SetTextToBlue("Enter in the Nft Id");
                string nftId = Utils.ReadLineWarningNoNulls("Enter in the Nft Id"); 
                Font.SetTextToBlue("Enter in the Minter");
                string minter = Utils.ReadLineWarningNoNulls("Enter in the Minter"); 
                Font.SetTextToBlue("Enter in the Token/Collection Address");
                tokenAddress = Utils.ReadLineWarningNoNulls("Enter in the Token/Collection Address"); 

                var nftdataRequest = await loopringService.GetNftData(nftId, minter, tokenAddress); 
                nftData = nftdataRequest.nftData;

                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);  
                nftTokenId = userNftToken.data[0].tokenId;

            }
            var nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
            var nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

            Font.SetTextToBlue($"How many of '{nftMetadata.name}' do you want to transfer to each address?");
            nftAmount = Utils.CheckNftSendAmount(howManyWallets, userNftToken.data[0].total);

            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt"))
            {
                string toAddressInitial;
                while ((toAddressInitial = sr.ReadLine()) != null)
                {
                    //remove whitespace after wallet address if it exists.
                    var toAddress = toAddressInitial.ToLower().Trim();

                    //Initialize loopring service
                    //ILoopringService loopringService = new LoopringService();

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
                            invalidAddress.Add(toAddressInitial);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
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
                    var ethECKey = new Nethereum.Signer.EthECKey(metamaskPrivateKey.Replace("0x", ""));
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
                if (validAddress.Count > 0)
                {
                    Font.SetTextToGreen($"The following were valid addresses that did receive '{nftMetadata.name}'.");
                    foreach (var address in validAddress)
                    {
                        Console.WriteLine(address);
                    }
                }
                if(invalidAddress.Count > 0)
                {
                    Font.SetTextToRed($"The following were invalid addresses that did not receive '{nftMetadata.name}'.");
                    foreach (var address in invalidAddress)
                    {
                        Console.WriteLine(address);
                    }
                }
            }
            break;
        #endregion case 1
        #region case 2
        case "2":
            Font.SetTextToBlue("Airdrop unique NFTs to any users.");
            Console.WriteLine("Here you will drop many Nfts to many users. Check the README for walletAddress.txt setup.");
            Console.WriteLine("Let's get started.");
            Font.SetTextToBlue("Did you setup your walletAddress.txt?");
            userResponseOnNftData = Utils.CheckYes(userResponseOnNftData.ToLower());
            Font.SetTextToBlue("How many of each Nft do you want to transfer to each address?");
            nftAmount = Console.ReadLine();

            //be sure to add a text file with all the wallet addresses, NftData. one on each line.
            using (StreamReader sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt"))
            {

                string walletAddressLine;
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    string[] walletAddressLineArray = walletAddressLine.Split(',');
                    var toAddress = walletAddressLineArray[0].ToLower().Trim();
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
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
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
                    var ethECKey = new Nethereum.Signer.EthECKey(metamaskPrivateKey.Replace("0x", ""));
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
                    validAddress.Add(walletAddressLine);

                }
                Font.SetTextToGreen($"The following were valid addresses and/or nftData that did receive an Nft:");
                foreach (var address in validAddress)
                {
                    Console.WriteLine(address);
                }
                Font.SetTextToRed($"The following were invalid addresses and/or nftData that did not receive an Nft:");
                foreach (var address in invalidAddress)
                {
                    Console.WriteLine(address);
                }
            }
            break;
        #endregion case 2
        #region case 3
        case "3":
            Font.SetTextToBlue("Find Nft Data for a single Nft.");
            Console.WriteLine("Here you will get all the Nft Data from an Nft.");
            Console.WriteLine("Find your Nft on lexplorer.io or explorer.loopring.io.");
            Console.WriteLine("You should see the Nft Id, Minter, and Token/Collection Address");
            Font.SetTextToBlue("Enter in the Nft Id");
            string nftIdForNftData = Console.ReadLine(); //the amount of the nft to transfer
            Font.SetTextToBlue("Enter in the Minter");
            string minterForNftData = Console.ReadLine(); //the amount of the nft to transfer
            Font.SetTextToBlue("Enter in the Token/Collection Address");
            string tokenAddressForNftData = Console.ReadLine(); //the amount of the nft to transfer

            if (minterForNftData.Contains(".eth"))
            {
                var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, minterForNftData);
                if (!String.IsNullOrEmpty(varHexAddress.data))
                {
                    minterForNftData = varHexAddress.data;
                }
                else
                {
                    Console.WriteLine($"{minterForNftData} is an invalid address");
                    continue;
                }
            }

            var nftdataRequestForNftData = await loopringService.GetNftData(nftIdForNftData, minterForNftData, tokenAddressForNftData);  //the nft tokenId, not the nftId
            Font.SetTextToGreen($"This Nft's Nft Data is {nftdataRequestForNftData.nftData}");
            break;
        #endregion case 3
        #region case 4
        case "4":
            string userResponseOnFileSetup = "";
            Font.SetTextToBlue("Find Nft Holders from Nft Data.");
            Console.WriteLine("Here you will get all the wallet addresses that hold the given Nft Data.");
            Console.WriteLine("Let's get started.");
            Font.SetTextToBlue("Is this for one or many Nfts?");
            userResponseOnMany = Utils.CheckOneOrMany(userResponseOnMany.ToLower());
            
            if (userResponseOnMany == "one")
            {
                Font.SetTextToBlue("What is the Nft Data?");
                nftData = Console.ReadLine();
                nftHoldersAndTotalList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, nftData);  //the nft tokenId, not the 
                foreach (var nftHoldersAndTotalSingle in nftHoldersAndTotalList)
                {
                    if (nftHoldersAndTotalSingle == nftHoldersAndTotalList.FirstOrDefault())
                    {
                        Font.SetTextToBlue($"NftData {nftData} has {nftHoldersAndTotalSingle.totalNum} total holders.");
                        Font.SetTextToGreen($"Wallet Address \t\t\t\t\t\t Total");
                    };
                    foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                    {
                        var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());  //the nft tokenId, not the 
                        Console.WriteLine($"{userAccountInformation.owner}\t\t {item.amount}");
                    }
                }
            }
            else if (userResponseOnMany == "many") 
            {
                Console.WriteLine("Check the README for nftData.txt setup.");
                Font.SetTextToBlue("Did you setup your nftData.txt?");
                userResponseOnFileSetup = Utils.CheckYes(userResponseOnFileSetup.ToLower());
                Font.SetTextToBlue("Would you like to view just the wallet addresses?");
                userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo(userResponseOnWalletAddressDisplay.ToLower());

                using (StreamReader sr = new StreamReader(".\\..\\..\\..\\nftData.txt"))
                {
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
                                    nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                                    nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

                                    Font.SetTextToBlue($"{nftMetadata.name}, {nftMetadata.description}, {nftData} has {nftHoldersAndTotalSingle.totalNum} total holders.");
                                    Font.SetTextToGreen($"Wallet Address \t\t\t\t\t\t Total");
                                };
                                foreach (var item in nftHoldersAndTotalSingle.nftHolders)
                                {
                                    var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());  //the nft tokenId, not the 
                                    Console.WriteLine($"{userAccountInformation.owner}\t\t {item.amount}");
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
            Font.SetTextToBlue("Find an account's Nft wallet holders.");
            Console.WriteLine("Here you will get all the wallet addresses that hold an account's Nfts.");
            Console.WriteLine("Let's get started.");
            Font.SetTextToBlue("What is the user account Id?");
            userResponseOnAccountId = int.Parse(Console.ReadLine());
            Font.SetTextToBlue("Would you like to view just the wallet addresses?");
            userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo(userResponseOnWalletAddressDisplay.ToLower());
            userMintsAndTotalList = await loopringService.GetUserMintedNfts(settings.LoopringApiKey, userResponseOnAccountId);
            Font.SetTextToGreen($"{userResponseOnAccountId} has {userMintsAndTotalList.First().totalNum} mints");
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
                            Console.WriteLine($"{userAccountInformation.owner}");
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

                        Font.SetTextToBlue($"{nftMetadata.name}, {nftMetadata.description}, {nftData} has {nftHoldersAndTotal.totalNum} total holders.");
                        Font.SetTextToGreen($"Wallet Address \t\t\t\t\t\t Total");
                        foreach (var item in nftHoldersAndTotal.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            Console.WriteLine($"{userAccountInformation.owner}\t\t {item.amount}");
                        }
                    }
                }
            }
            break;
        #endregion case 5
        #region case 6
        case "6":
            userResponseOnWalletAddressDisplay = "";
            Font.SetTextToBlue("Find all Nft Data from a Collection.");
            Console.WriteLine("Here you will get all the Nft Data from the Nfts in a Collection.");
            Console.WriteLine("Let's get started.");
            Font.SetTextToBlue("What is the user account Id?");
            userResponseOnAccountId = int.Parse(Console.ReadLine());
            userMintsAndTotalList = await loopringService.GetUserMintedNfts(settings.LoopringApiKey, userResponseOnAccountId);
            Font.SetTextToGreen($"{userResponseOnAccountId} has {userMintsAndTotalList.First().totalNum} mints");
            
                foreach (var userMintsAndTotalSingle in userMintsAndTotalList)
                {
                    var mints = userMintsAndTotalSingle.mints;
                    foreach (var mint in mints)
                    {
                    Console.WriteLine(mint.nftData);
                    }
                }
            break;
        #endregion case 6
        #region case 7
        case "7":
            //Token id of 1 for LRC, token id of 0 for ETH
            decimal amountToTransfer = 0m;
            string transferMemo = "";
            int transferTokenId = 3;
            string transferTokenSymbol = "";
            Font.SetTextToBlue("7. Airdrop LRC/ETH to any users.");
            Console.WriteLine("Here you will airdrop LRC/ETH to many users. Check the README for walletAddress.txt setup.");
            Console.WriteLine("Let's get started.");
            Font.SetTextToBlue("Did you setup your walletAddress.txt?");
            userResponseOnNftData = Utils.CheckYes(userResponseOnNftData.ToLower());
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            Font.SetTextToBlue("Amount to transfer per address?");
            amountToTransfer = decimal.Parse(Console.ReadLine()?.ToLower().Trim());
            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.ToLower().Trim();

            if(transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if(transferTokenId == 0)
            {
               transferTokenSymbol = "ETH";
            }

            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt"))
            {

                string walletAddressLine;
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
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
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(metamaskPrivateKey.Replace("0x", ""));
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
                if(validAddress.Count > 0)
                {
                    Font.SetTextToGreen($"The following were valid addresses that did receive {transferTokenSymbol}");
                    foreach (var address in validAddress)
                    {
                        Console.WriteLine(address);
                    }
                }
                if(invalidAddress.Count > 0)
                {
                    Font.SetTextToRed($"The following were invalid addresses that did not receive {transferTokenSymbol}");
                    foreach (var address in invalidAddress)
                    {
                        Console.WriteLine(address);
                    }
                }
            }
            break;
            #endregion case 7

    }
    validAddress.Clear();
    invalidAddress.Clear();
    Font.SetTextToBlue("Start a new functionality?");
    userResponseReadyToMoveOn = Utils.CheckYesOrNo(userResponseReadyToMoveOn);
}
Font.SetTextToGreen("Thanks for using Cobmin's LoopDropSharp.");
Font.SetTextToPurple("Check out his Nft collection at https://loopexchange.art/collection/flowers.");