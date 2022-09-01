# LoopDropSharp
This application can airdrop NFTs and other tokens on Layer 2.

https://cobmin.io/posts/Airdrop-Nfts-on-Loopring

## Setup

Here is a video going over the setup: https://youtu.be/Bkl6BwfA6jE

This application uses a ***Metamask/GameStop*** private key to sign the transfers so you will need to export that out from your Metamask/GameStop account. You can export the Loopring related account details from the "Security" tab while logged into https://loopring.io. Make sure these details are from the same account.

DO NOT SHARE ANY API OR PRIVATE KEYS with anyone else!!!!!!!

This was written in .NET 6 so you need an IDE that can compile it. 

You will need to change the "appsettings.json" file in the project directory with the necessary information.

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
After setting up the appsettings.json, execute the solution and follow the prompts.

## Text File setup
Wallet Address setup for: 1. Airdrop the same NFT to any users.
In the walletAddresses.txt located in the project directory add your wallet addresses. You will have one wallet address on one line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS. 

Wallet Address setup for: 2. Airdrop unique NFTs to any users
In the walletAddresses.txt located in the project directory add your wallet address a comma and then the nft data (example: 0x4a71e0267207cec67c78df8857d81c508d43b00d,0x103cb20d3b310873f711d25758d57f18ba77a6b7842ae605d662e0ddd908ed5a). You will have one wallet address and nft data on each line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

Nft Data setup for: 4. Find Nft Holders from Nft Data.
In the nftData.txt located in the project directory add your Nft Data. You will have one Nft Data on one line. 

When transfering, a successful Nft transfer will return the following JSON response:

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
The original boiler plate was originally made by myself, then Cobmin did some major modifications on a fork which I accepted into master.

Also thanks to:

TItsMonty.eth for the original NFT Transfer code in the [LooPyMinty](https://github.com/Montspy/LooPyMinty) repo which I converted to C#.

Also thanks to Taranasus for his [LoopringSharp](https://github.com/taranasus/LoopringSharp) repo for the ECDSA signing which I also used.

I also used my own PoseidonSharp library for generating the Poseidon hashes and EDDSA signing.
