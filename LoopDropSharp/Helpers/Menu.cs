using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Helpers
{
    public class Menu
    {
        public static void BannerForLoopDropSharp()
        {
            // Initial Information and Questions
            Console.Title = "cobmin.io's LoopDropSharp";
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
            Font.SetTextToBlue("         |           |  Version 1.0|  ");
            //Font.SetTextToDarkBlue("Query and send your Nfts");
            Font.SetTextToBlue("[Query·Nft·Holders·and·Airdrop·Nfts/crypto]");
            Console.WriteLine();
            Font.SetTextToDarkGray("If you have any questions, start at https://cobmin.io/posts/Airdrop-Nfts-on-Loopring");
            Font.SetREADMEFontColorPurple("Find information on the setup files in the ", "README", " at https://github.com/cobmin/LoopDropSharp/blob/master/README.md");
            Font.SetTextToDarkGreen("Ready to start?");
        }
        public static Dictionary<string, string> MenuForLoopDropSharp()
        {
            var allUtilities = new Dictionary<string, string>()
            {
                {"utilityZero", "General tips and FAQs"},
                {"utilityOne", "Find Nft Data for a single Nft"},
                {"utilityTwo", "Find Nft Datas from Nft Ids"},
                {"utilityThree", "Find all Nft Data from a Collection"},
                {"utilityFour", "Find Nft Holders from Nft Data"},
                {"utilityFive", "Find Nft Holders from an address"},
                {"utilitySix", "Airdrop the same NFT to any users"},
                {"utilitySeven", "Airdrop unique NFTs to any users"},
                {"utilityEight", "Airdrop LRC/ETH to any users"}
            };
            // Menu of the Utilities. need to be sure to change numbers here and in the CheckUtilityNumber
            Font.SetTextToBlue("This application can currently perform the following:");
            Font.SetTextToDarkPurple("     Lookups:");
            Font.SetTextToWhite($"\t 1. {allUtilities.ElementAt(1).Value}.");
            Font.SetTextToWhite($"\t 2. {allUtilities.ElementAt(2).Value}.");
            Font.SetTextToWhite($"\t 3. {allUtilities.ElementAt(3).Value}.");
            Font.SetTextToWhite($"\t 4. {allUtilities.ElementAt(4).Value}.");
            Font.SetTextToWhite($"\t 5. {allUtilities.ElementAt(5).Value}.");
            Font.SetTextToDarkPurple("     Airdrops:");
            Font.SetTextToWhite($"\t 6. {allUtilities.ElementAt(6).Value}.");
            Font.SetTextToWhite($"\t 7. {allUtilities.ElementAt(7).Value}.");
            Font.SetTextToWhite($"\t 8. {allUtilities.ElementAt(8).Value}.");
            Font.SetTextToDarkPurple("     Tips/FAQs:");
            Font.SetTextToWhite($"\t 0. {allUtilities.ElementAt(0).Value}.");
            Font.SetTextToBlue("Which would you like to do?");
            return allUtilities;

        }

        public static void FooterForLoopDropSharp()
        {
            Console.WriteLine();
            Font.SetTextToPurple("Thanks for using Cobmin's LoopDropSharp.");
            Font.SetTextToDarkGreen("Any feedback? You can find his contact information here, https://cobmin.io/.");
            Font.SetTextToPurple("Check out his Nft collection at https://loopexchange.art/collection/flowers.");
            Font.SetTextToGray("Need help with your drop? Let me know.");
            Console.WriteLine(); 
            Font.SetTextToDarkGray("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
            Font.SetTextToDarkGray("|P|o|w|e|r| |t|o| |t|h|e| |C|r|e|a|t|o|r|s|");
            Font.SetTextToDarkGray("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
        }
    }
}
