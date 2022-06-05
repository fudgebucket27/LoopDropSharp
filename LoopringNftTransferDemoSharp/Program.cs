using LoopNftTransferDemoSharp;
using Microsoft.Extensions.Configuration;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Numerics;
using Type = LoopNftTransferDemoSharp.Type;

//Change these
string nftAmount = "1"; //the amount of the nft to transfer
int nftTokenId = 34008; //the nft tokenId, not the nftId
string nftData = "0x18c28cdd97789a7a82603b9a4618dd711660e7231cd6e14087baa858de483e32"; //the nftData, not nftId
string toAddress = "0x99fdddfdc9277404db0379009274cc98d3688f8b"; //the Address to send it to

//Settings loaded from the appsettings.json file
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

string loopringApiKey = settings.LoopringApiKey;//loopring api key KEEP PRIVATE
string loopringPrivateKey = settings.LoopringPrivateKey; //loopring private key KEEP PRIVATE
var metamaskPrivateKey = settings.MetamaskPrivateKey; //metamask private key KEEP PRIVATE
var fromAddress = settings.LoopringAddress; //your loopring address
var fromAccountId = settings.LoopringAccountId; //your loopring account id
var validUntil = settings.ValidUntil; //the examples seem to use this number
var maxFeeTokenId = settings.MaxFeeTokenId; //0 should be for ETH, 1 is for LRC?
var exchange = settings.Exchange; //loopring exchange address, shouldn't need to change this,

int toAccountId = 0; //leave this as 0 DO NOT CHANGE

//Initialize loopring service
ILoopringService loopringService = new LoopringService();

//Storage id
var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

//Getting the offchain fee
var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

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
    ) ;

Console.WriteLine(nftTransferResponse);