# LoopDropSharp
Batch airdrop NFTs and tokens on Loopring Layer 2.
Nft Lookups and Tips as well

This is a Console App on .NET 6. To build and compile this yourself you need something like Visual Studio 2022.

I suggest downloading one of the compiled releases.

## Setup

The following instructions in this README are for version 1 and above. For version 1 and above you can follow the [Video Tutorial](url)

For versions before 1 [see this](https://youtu.be/Bkl6BwfA6jE)

Download one of the compiled releases in the [Releases](https://github.com/cobmin/LoopDropSharp/releases/tag/v1.0.0) section and unzip it into a location of your choice. You will need to edit the included appsettings.json file with your own Loopring details. 

This application uses a ***Metamask/GameStop*** private key to sign the transfers so you will need to export that out from your Metamask/GameStop account. You can export the Loopring related account details from the "Security" tab while logged into https://loopring.io. Make sure these details are from the same account. Remember to keep these values private and do not share with anyone!

macOS users: You also need to run the following command in the unzipped folder of LoopDropSharp to turn it into an executable in order to run it. You may also need to add it as a trusted application if it get's blocked from running.

```
chmod +x LoopDropSharp
If compiling yourself please read the section about it below.
```

## Compiling yourself

If compiling yourself. You need to generate an appsettings.json file in the project directory with the "Copy to Output directory" set to "Copy Always". The appsettings.json file should look like the following, remember to keep these values private and do not share with anyone!

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
### Utility 2: Find Nft Datas from Nft Ids
In the Input.txt located in the project directory add your nft Ids. You will have one Id each line. Each Id will be one lookup.

### Utility 4: Find Nft Holders from Nft Data
In the Input.txt located in the project directory add your nft Data. You will have one nft Data on each line. Each nft Data will be one lookup.

### Utility 6: Airdrop the same NFT to any users
In the Input.txt located in the project directory add your wallet addresses that you want to transfer to. You will have one wallet address on each line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### Utility 7: Airdrop unique NFTs to any users
In the Input.txt located in the project directory add your wallet address a comma and then the nft data (example: 0x4a71e0267207cec67c78df8857d81c508d43b00d,0x103cb20d3b310873f711d25758d57f18ba77a6b7842ae605d662e0ddd908ed5a). You will have one wallet address and one nft data on each line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### Utility 8: Airdrop LRC/ETH to any users
In the Input.txt located in the project directory add your wallet addresses. You will have one wallet address on one line. Each wallet address will be one transfer of LRC/ETH. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS. 

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
## Video Tutorial for Version 1 and above
Here is a video going over the setup: 

## Video Tutorial for Versions below 1
Here is a video going over the setup: https://youtu.be/Bkl6BwfA6jE

