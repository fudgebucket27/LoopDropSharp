﻿using LoopDropSharp;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public static class Utils
    {

        //public static void CheckForAppsettingsDotJson()
        //{
        //    string fileName = @".\..\..\..\appsettings.json";

        //    try
        //    {
        //        if (!File.Exists(fileName))
        //        {
        //            // Create a new file     
        //            using (StreamWriter sw = File.CreateText(fileName))
        //            {
        //                {
        //                    sw.Write("{");
        //                    sw.Write("  \"Settings\": {\r\n");
        //                    sw.Write("    \"LoopringApiKey\": \"ksdBlahblah\", //Your loopring api key.  DO NOT SHARE THIS AT ALL.\r\n");
        //                    sw.Write("    \"LoopringPrivateKey\": \"0xblahblah\", //Your loopring private key.  DO NOT SHARE THIS AT ALL.\r\n");
        //                    sw.Write("    \"MetamaskPrivateKey\": \"asadasdBLahBlah\", //Private key from metamask. DO NOT SHARE THIS AT ALL.\r\n");
        //                    sw.Write("    \"LoopringAddress\": \"0xblahabla\", //Your loopring address\r\n");
        //                    sw.Write("    \"LoopringAccountId\": 40940, //Your loopring account id\r\n");
        //                    sw.Write("    \"ValidUntil\": 1700000000, //How long this transfer should be valid for. Shouldn't have to change this value\r\n");
        //                    sw.Write("    \"MaxFeeTokenId\": 1, //The token id for the fee. 0 for ETH, 1 for LRC\r\n");
        //                    sw.Write("    \"Exchange\": \"0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4\" //Loopring Exchange address\r\n");
        //                    sw.Write("  }\r\n");
        //                    sw.Write("}");
        //                }
        //            }
        //            Font.SetTextToRed("The Appsettings.json file is not setup. Please set it up before proceeding.");
        //            Font.SetTextToYellow("Watch this video for more information, https://www.youtube.com/watch?v=Bkl6BwfA6jE&t=18s.");
        //            Font.SetTextToYellow("The file's properties > Copy to Output Directory might need to be set to 'Copy Always'.");
        //            Font.SetTextToYellow("Application may need to be restarted after changes are made.");
        //            Font.SetTextToBlue("Are you ready?");
        //            var userResponseReadyToMoveOn = Utils.CheckYes();
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        Font.SetTextToWhite(Ex.ToString());
        //    }
        //}
        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        public static string CheckYesOrNo()
        {
            var userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "yes") && (userResponse != "no"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes or no.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckOneOrMany()
        {
            var userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "one") && (userResponse != "many"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer one or many.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckYes()
        {
            var userResponse = Console.ReadLine().ToLower();
            while (userResponse != "yes")
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes when you are ready.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }
        public static string CheckUtilityNumber(int maxUtilityNumber)
        {
            int userResponse;
            bool checkNumber;
            do
            {
                checkNumber = int.TryParse(Console.ReadLine(), out userResponse);
                if (checkNumber == false)
                {
                    Font.SetTextToYellow($"Please type a number between 0 and {maxUtilityNumber}.");

                }
            } while (checkNumber == false);
            while (userResponse < 0 && userResponse > maxUtilityNumber)
            {
                Font.SetTextToYellow($"Please type a number between 0 and {maxUtilityNumber}.");
                do
                {
                    checkNumber = int.TryParse(Console.ReadLine(), out userResponse);
                    if (checkNumber == false)
                    {
                        Font.SetTextToYellow($"Please type a number between 0 and {maxUtilityNumber}.");

                    }
                } while (checkNumber == false);
            }
            return userResponse.ToString();
        }
        public static int Check1Or2(int userResponse)
        {
            bool validOrNot = false;
            var counter = 0;
            do
            {
                if (counter == 0)
                {
                    validOrNot = int.TryParse(Console.ReadLine()?.ToLower().Trim(), out userResponse);
                    counter++;
                }
                else
                {
                    Font.SetTextToYellow("Please answer 1 or 2.");
                    validOrNot = int.TryParse(Console.ReadLine()?.ToLower().Trim(), out userResponse);
                }
            } while (((userResponse != 1) && (userResponse != 2)) || validOrNot == false);

            return userResponse;
        }

        public static int CheckInputDotTxt()
        {
            StreamReader sr;
            string walletAddresses;
            int howManyWalletAddresses;
            var counter = 0;
            var userResponseOnWalletSetup = "";
            do
            {
                if (counter == 0)
                {
                    Font.SetTextToGreenInline("Did you setup your Input.txt? ");
                    Font.SetREADMEFontColorDarkGray("Check the ", "README", $" for Input.txt setup.");
                    userResponseOnWalletSetup = CheckYes();
                    sr = new StreamReader("./Input.txt");
                    counter++;
                }
                else
                {
                    Font.SetREADMEFontColorYellow("It doesn't look like you did. Please refer to the ","README", " and respond yes when you are ready.");
                    userResponseOnWalletSetup = CheckYes();
                    sr = new StreamReader("./Input.txt");
                }
                walletAddresses = sr.ReadToEnd().Replace("\r\n", "\r");
                howManyWalletAddresses = walletAddresses.Split('\r').Length;
                if (walletAddresses.EndsWith('\r'))
                {
                    do
                    {
                        walletAddresses = walletAddresses.Remove(walletAddresses.Length - 1).Remove(walletAddresses.Length - 1);
                        howManyWalletAddresses--;
                    } while (walletAddresses.EndsWith('\r'));
                }
                sr.Dispose();
            } while (walletAddresses == "");
            return howManyWalletAddresses;
        }
        public static int CheckInputDotTxtTwoInputs()
        {
            StreamReader sr;
            string walletAddresses;
            int howManyWalletAddresses;
            var counter = 0;
            var noDoubleWarning = false;
            do
            {
                if (counter == 0)
                {
                    Font.SetTextToGreenInline("Did you setup your Input.txt? ");
                    Font.SetREADMEFontColorDarkGray("Check the ", "README", $" for Input.txt setup.");
                    CheckYes();
                    sr = new StreamReader("./Input.txt");
                    counter++;
                }
                else
                {
                    if (noDoubleWarning == false)
                    {
                        Font.SetREADMEFontColorYellow("It doesn't look like you did. Please refer to the ", "README", " and respond yes when you are ready.");
                    }
                    CheckYes();
                    sr = new StreamReader("./Input.txt");
                }
                walletAddresses = sr.ReadToEnd().Replace("\r\n", "\r");
                howManyWalletAddresses = walletAddresses.Split('\r').Length;
                if (walletAddresses.EndsWith('\r'))
                {
                    do
                    {
                        walletAddresses = walletAddresses.Remove(walletAddresses.Length - 1).Remove(walletAddresses.Length - 1);
                        howManyWalletAddresses--;
                    } while (walletAddresses.EndsWith('\r'));
                }
                sr.Dispose();
                try
                {
                    sr = new StreamReader("./Input.txt");
                    string[] walletAddressLineArray = sr.ReadLine().Split(',');
                    var toAddress = walletAddressLineArray[0].ToLower().Trim();
                }
                catch (Exception)
                {
                    Font.SetTextToYellow("It looks like your Input.txt needs the walletAddress,NftData.");
                    Font.SetTextToBlue("respond yes when you are ready.");
                    noDoubleWarning = true;
                }
            } while (walletAddresses == "");
            return howManyWalletAddresses;
        }

        public static int GetInputDotTxtLines()
        {
            StreamReader sr;
            string walletAddresses;
            int howManyWalletAddresses;
            do
            {

                sr = new StreamReader("./Input.txt");
                walletAddresses = sr.ReadToEnd().Replace("\r\n", "\r");
                howManyWalletAddresses = walletAddresses.Split('\r').Length;
                if (walletAddresses.EndsWith('\r'))
                {
                    do
                    {
                        walletAddresses = walletAddresses.Remove(walletAddresses.Length - 1).Remove(walletAddresses.Length - 1);
                        howManyWalletAddresses--;
                    } while (walletAddresses.EndsWith('\r'));
                }
                sr.Dispose();
            } while (walletAddresses == "");
            return howManyWalletAddresses;
        }

        public static string CheckNftSendAmount(int howManyWallets, string userNftTokentotalNum)
        {
            string nftAmount;
            do
            {
            nftAmount = ReadLineWarningNoNullsForceInt("How many Nfts do you want to transfer to each address?");
            } while (nftAmount == null);
            while ((howManyWallets * int.Parse(nftAmount)) > int.Parse(userNftTokentotalNum))
            {
                Font.SetTextToRed($"Math Error. You have {userNftTokentotalNum} of this Nft in your wallet and want to " +
                    $"send to {nftAmount} of them to {howManyWallets} wallets each.");
                Font.SetTextToBlue("How many of your Nft do you want to transfer to each address?");
                do
                {
                    nftAmount = ReadLineWarningNoNullsForceInt("How many Nfts do you want to transfer to each address?");
                } while (nftAmount == null);
                howManyWallets = GetInputDotTxtLines();
            }
            return nftAmount;
        }

        public static string ReadLineWarningNoNulls(string message)
        {
            var s = Console.ReadLine();
            while (string.IsNullOrEmpty(s))
            {
                Font.SetTextToYellow($"Please, {message}");
                s = Console.ReadLine();
            }
            return s;
        }
        public static string ReadLineWarningNoNullsForceInt(string message)
        
        {
            var s = Console.ReadLine();
            int i;
            bool number = int.TryParse(s, out i);
            while (string.IsNullOrEmpty(s) || number == false)
            {
                Font.SetTextToYellow($"Please, {message}");
                s = Console.ReadLine();
                number = int.TryParse(s, out i);
            }
            return i.ToString();
        }

        public static decimal ReadLineWarningNoNullsForceDecimal(string message)

        {
            var s = Console.ReadLine().Trim();
            decimal i;
            bool number = decimal.TryParse(s, out i);
            while (string.IsNullOrEmpty(s) || number == false)
            {
                Font.SetTextToYellow($"Please, {message}");
                s = Console.ReadLine();
                number = decimal.TryParse(s, out i);
            }
            return i;
        }

        public static void ShowAirdropAudit(List<string> validAddress, List<string> invalidAddress, List<string> banishAddress, string? nftMetadataName)
        {
            if (validAddress.Count > 0)
            {
                Font.SetTextToBlue($"The following were valid addresses that did receive '{nftMetadataName}'.");
                foreach (var address in validAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (invalidAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were invalid addresses that did not receive '{nftMetadataName}'.");
                foreach (var address in invalidAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (banishAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were banish addresses that did not receive '{nftMetadataName}'.");
                foreach (var address in banishAddress)
                {
                    Console.WriteLine(address);
                }
            }
        }
        public static void ShowAirdropAuditAmbiguous(List<string> validAddress, List<string> invalidAddress, List<string> banishAddress)
        {
            if (validAddress.Count > 0)
            {
                Font.SetTextToBlue($"The following were valid addresses that did receive their Nft.");
                foreach (var address in validAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (invalidAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were invalid addresses or the nftData was invalid and they not receive their Nft.");
                foreach (var address in invalidAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (banishAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were banish addresses that did not receive their Nft.");
                foreach (var address in banishAddress)
                {
                    Console.WriteLine(address);
                }
            }
        }
        public static void ShowAirdropAuditCrypto(List<string> validAddress, List<string> invalidAddress, List<string> banishAddress, string transferTokenSymbol)
        {
            if (validAddress.Count > 0)
            {
                Font.SetTextToBlue($"The following were valid addresses that did receive {transferTokenSymbol}.");
                foreach (var address in validAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (invalidAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were invalid addresses that did not receive {transferTokenSymbol}.");
                foreach (var address in invalidAddress)
                {
                    Console.WriteLine(address);
                }
            }
            if (banishAddress.Count > 0)
            {
                Font.SetTextToRed($"The following were banish addresses that did not receive {transferTokenSymbol}.");
                foreach (var address in banishAddress)
                {
                    Console.WriteLine(address);
                }
            }
        }

        public static int GetUnixTimestamp() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;



    }
}
