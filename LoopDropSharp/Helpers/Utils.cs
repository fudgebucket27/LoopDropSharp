using LoopDropSharp;
using Nethereum.Signer.EIP712;
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
        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        public static string CheckYesOrNo(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "yes") && (userResponse != "no"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes or no.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckOneOrMany(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "one") && (userResponse != "many"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer one or many.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckYes(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while (userResponse != "yes")
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes when you are ready.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }
        public static string CheckUtilityNumber(string userResponse)
        {
            userResponse = Console.ReadLine();
            while ((userResponse != "1") && (userResponse != "2") && (userResponse != "3") && (userResponse != "4") && (userResponse != "5") && (userResponse != "6") && (userResponse != "7"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please type a number between 1 and 7.");
                userResponse = Console.ReadLine();
            }
            return userResponse;
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

        public static int CheckWalletAddresstxt()
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
                    Font.SetTextToBlue("Did you setup your walletAddress.txt?");
                    userResponseOnWalletSetup = CheckYes(userResponseOnWalletSetup.ToLower());
                    sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt");
                    counter++;
                }
                else
                {
                    Font.SetTextToYellow("It doesn't look like you did. Please refer to the README and respond yes when you are ready.");
                    userResponseOnWalletSetup = CheckYes(userResponseOnWalletSetup.ToLower());
                    sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt");
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

        public static int GetWalletAddresstxtLines()
        {
            StreamReader sr;
            string walletAddresses;
            int howManyWalletAddresses;
            var userResponseOnWalletSetup = "";
            do
            {

                sr = new StreamReader(".\\..\\..\\..\\walletAddresses.txt");
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
            var nftAmount = Console.ReadLine(); // need to make a check here no negative/null/letters/symbols
            while ((howManyWallets * int.Parse(nftAmount)) > int.Parse(userNftTokentotalNum))
            {
                Font.SetTextToYellow($"Math Error. You have {userNftTokentotalNum} of this Nft in your wallet and want to " +
                    $"send to {nftAmount} of them to {howManyWallets} wallets.");
                Font.SetTextToBlue("How many of your Nft do you want to transfer to each address?");
                nftAmount = Console.ReadLine(); // need to make a check here no negative/null/letters/symbols
                howManyWallets = GetWalletAddresstxtLines();
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

        public static int GetUnixTimestamp() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;



    }
}
