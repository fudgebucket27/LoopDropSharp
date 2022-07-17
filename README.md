# LoopringBatchNftTransferDemoSharp
This is Fudgey's demo repo modified to Batch transfer a specific NFT to multiple accounts on Loopring in C#.

It uses a ***Metamask*** private key to sign the transfers so you will need to export that out from your Metamask account. You can export the Loopring related account details from the "Security" tab while logged into https://loopring.io. Make sure these details are from the same account!

DO NOT SHARE ANY API OR PRIVATE KEYS with anyone else!!!!!!!

This was written in .NET 6 so you need an IDE that can compile it. 

You will need to setup an "appsettings.json" file in the project directory like below with the property "Copy to Output Directory" set to "Copy Always".

```json
{
  "Settings": {
    "LoopringApiKey": "ksdBlahblah", //Your loopring api key.  DO NOT SHARE THIS AT ALL.
    "LoopringPrivateKey": "0xblahblah", //Your loopring private key.  DO NOT SHARE THIS AT ALL.
    "MetamaskPrivateKey": "asadasdBLahBlah", //Private key from metamask. DO NOT SHARE THIS AT ALL.
    "LoopringAddress": "0xblahabla", //Your loopring address
    "LoopringAccountId": 40940, //Your loopring account id
    "ValidUntil": 1700000000, //How long this transfer should be valid for. Shouldn't have to change this value
    "MaxFeeTokenId": 1, //The token id for the fee. 0 for ETH, 1 for LRC
    "Exchange": "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4" //Loopring Exchange address
  }
}
```

Lines 12-15 in the Program.cs file are what need to be changed with the nftAmount, nftTokenId, and nftData.

```csharp
string nftAmount = "1"; //the amount of the nft to transfer
int nftTokenId = 34008; //the nft tokenId, not the nftId
string nftData = "0x18c28cdd97789a7a82603b9a4618dd711660e7231cd6e14087baa858de483e32"; //the nftData, not nftId
```
In the walletAddresses.txt located in the project directory add your wallet address. You will have one wallet address on one line. Each wallet address will be one transfer. Be sure to have enough LRC for each transfer.

A successful NFT transfer will return the following JSON response:

```json
{
  "hash": "0x1aecf4f692076ec8efe7ee4856568ce255029ee9ebbfc99138da560de353e4ac",
  "status": "processing",
  "isIdempotent": false,
  "accountId": 40940,
  "tokenId": 0,
  "storageId": 169
}
```

## Credits
I forked Fudgey's Repo and added a few lines of code. all credit goes to him.

Fudgy thanked the below so I do as well: 

Thanks to ItsMonty.eth for the original NFT Transfer code in the [LooPyMinty](https://github.com/Montspy/LooPyMinty) repo which I converted to C#.

Also thanks to Taranasus for his [LoopringSharp](https://github.com/taranasus/LoopringSharp) repo for the ECDSA signing which I also used.

I also used my own PoseidonSharp library for generating the Poseidon hashes and EDDSA signing.
