﻿using LoopDropSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Helpers
{
    public class MenuAndUtility
    {
        public Dictionary<string, string> allUtilities { get; set; }
        public string userResponseOnUtility { get; set; }


    }
    public class Menu
    {
        public static string BannerForLoopDropSharp()
        {
            // Initial Information and Questions
            Console.Title = "LoopDropSharp: Airdrop Nfts and Crypto";
            Font.SetTextToDarkPurple("Welcome to:");
            //Font.SetTextToDarkBlue(" _                      ____                 ____  _ ");
            //Font.SetTextToDarkBlue("| |    ___   ___  _ __ |  _ \\ _ __ ___  _ __/ ___|| |__   __ _ _ __ _ __  ");
            //Font.SetTextToDarkBlue("| |   / _ \\ / _ \\| '_ \\| | | | '__/ _ \\| '_ \\___ \\| '_ \\ / _` | '__| '_ \\ ");
            //Font.SetTextToDarkBlue("| |__| (_) | (_) | |_) | |_| | | | (_) | |_) |__) | | | | (_| | |  | |_) |");
            //Font.SetTextToDarkBlue("|_____\\___/ \\___/| .__/|____/|_|  \\___/| .__/____/|_| |_|\\__,_|_|  | .__/ ");
            //Font.SetTextToDarkBlue("                 |_|                   |_|              Version 1.0|_|    ");
            Font.SetTextToBlue("             __         __            ");
            Font.SetTextToBlue("|   _  _  _ |  \\ _ _  _(__ |_  _  _ _ ");
            Font.SetTextToBlue("|__(_)(_)[_)|__/[ (_)[_)__)[ )(_][ [_)");
            Font.SetTextToBlue("         |           |Version 1.2.0|  ");
            //Font.SetTextToDarkBlue("Query and send your Nfts");
            Font.SetTextToBlue("[Airdrop·Nfts·Crypto]");
            Console.WriteLine();
            Font.SetTextToDarkGray("If you have any questions, start at https://cobmin.io/posts/Airdrop-Nfts-on-Loopring");
            Font.SetREADMEFontColorDarkGray("Find information on the setup files in the ", "README", " at https://github.com/cobmin/LoopDropSharp/blob/master/README.md");
            Font.SetTextToGreen("Ready to start?");
            var userResponseReadyToMoveOn = Utils.CheckYes();
            return userResponseReadyToMoveOn;
        }
        public static MenuAndUtility MenuForLoopDropSharp()
        {
            var allUtilities = new Dictionary<string, string>()
            {
                {"utilityZero", "General tips and FAQs"},
                {"utilityOne", "Find Nft Data for a single Nft"},
                {"utilityTwo", "Find Nft Datas from Nft Ids"},
                {"utilityThree", "Find all Nft Data from a Collection"},
                {"utilityFour", "Find Nft Holders from Nft Data"},
                {"utilityFive", "Find Nft Holders from an Ens/Wallet"},
                {"utilitySix", "Find Nft Holders who own all given Nft Data"},
                {"utilitySeven", "Airdrop the same NFT to any users"},
                {"utilityEight", "Airdrop the same NFT to any users with different amounts"},
                {"utilityNine", "Airdrop unique NFTs to any users"},
                {"utilityTen", "Airdrop LRC/ETH to any users"},
                {"utilityEleven", "Airdrop LRC/ETH to any users with different amounts"}
            };
            // Menu of the Utilities. need to be sure to change numbers here and in the CheckUtilityNumber
            Font.SetTextToBlue("This application can currently perform the following:");
            Font.SetTextToPurple("     Lookups:");
            Font.SetTextToDarkPurple("        Nft Data");
            Font.SetTextToWhite($"\t 1. {allUtilities.ElementAt(1).Value}.");
            Font.SetTextToWhite($"\t 2. {allUtilities.ElementAt(2).Value}.");
            Font.SetTextToWhite($"\t 3. {allUtilities.ElementAt(3).Value}.");
            Font.SetTextToDarkPurple("        Nft Holders");
            Font.SetTextToWhite($"\t 4. {allUtilities.ElementAt(4).Value}.");
            Font.SetTextToWhite($"\t 5. {allUtilities.ElementAt(5).Value}.");
            Font.SetTextToWhite($"\t 6. {allUtilities.ElementAt(6).Value}.");
            Font.SetTextToPurple("     Airdrops:");
            Font.SetTextToDarkPurple("        Nfts");
            Font.SetTextToWhite($"\t 7. {allUtilities.ElementAt(7).Value}.");
            Font.SetTextToWhite($"\t 8. {allUtilities.ElementAt(8).Value}.");
            Font.SetTextToWhite($"\t 9. {allUtilities.ElementAt(9).Value}.");
            Font.SetTextToDarkPurple("        Crypto");
            Font.SetTextToWhite($"\t 10. {allUtilities.ElementAt(10).Value}.");
            Font.SetTextToWhite($"\t 11. {allUtilities.ElementAt(11).Value}.");
            Font.SetTextToPurple("     Tips/FAQs:");
            Font.SetTextToWhite($"\t 0. {allUtilities.ElementAt(0).Value}.");
            Font.SetTextToGreen("Which would you like to do?");
            var userResponseOnUtility = Utils.CheckUtilityNumber(allUtilities.Count() - 1);
            var menuAndUtility = new MenuAndUtility() { allUtilities = allUtilities , userResponseOnUtility = userResponseOnUtility };
            return menuAndUtility;

        }

        public static void FooterForLoopDropSharp()
        {
            Console.WriteLine();
            Font.SetTextToPurple("Thanks for using Cobmin's LoopDropSharp.");
            Font.SetTextToDarkGreen("Any feedback? You can find his contact information here, https://cobmin.io/.");
            Font.SetTextToPurple("Check out his Nft collection at https://loopexchange.art/collection/flowers.");
            Font.SetTextToGray("Need help with your drop? Let him know.");
            Console.WriteLine(); 
            Font.SetTextToDarkGray("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
            Font.SetTextToDarkGray("|P|o|w|e|r| |t|o| |t|h|e| |C|r|e|a|t|o|r|s|");
            Font.SetTextToDarkGray("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
        }

        public static string EndOfLoopDropSharpFunctionality(List<string> validAddress, List<string> invalidAddress, 
            List<string> banishAddress, List<MintsAndTotal> userMintsAndTotalList, List<NftHoldersAndTotal> nftHoldersAndTotalList
            )
        {
            validAddress.Clear();
            invalidAddress.Clear();
            banishAddress.Clear();
            userMintsAndTotalList.Clear();
            Font.SetTextToGreen("Start a new functionality?");
            var userResponseReadyToMoveOn = Utils.CheckYesOrNo();
            return userResponseReadyToMoveOn;
        }
    }
}
