using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    class Font
    {
        public static void SetREADMEFontColor(string beginning, string readMe, string end)
        {
            Console.Write(beginning);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{readMe}", Console.ForegroundColor);
            Console.ResetColor();
            Console.WriteLine(end);
        }

        public static void SetREADMEFontColorPurple(string beginning, string readMe, string end)
        {
            SetTextToPurpleInline(beginning);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{readMe}", Console.ForegroundColor);
            Console.ResetColor();
            SetTextToPurple(end);
        }
        public static void SetREADMEFontColorYellow(string beginning, string readMe, string end)
        {
            SetTextToYellowInline(beginning);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{readMe}", Console.ForegroundColor);
            Console.ResetColor();
            SetTextToYellow(end);
        }
        public static void SetTextToBlue(string str) {  
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToBlueInline(string str)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToDarkBlue(string str)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToYellow(string str)
        {  
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToYellowInline(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToRed(string str)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToRedInline(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToGreen(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToDarkGreen(string str)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToPurple(string str)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToPurpleInline(string str)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToDarkPurple(string str)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }

        public static void SetTextToGray(string str)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void SetTextToDarkGray(string str)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
        }
        public static void PowerToTheCreators()
        {
            Font.SetTextToRedInline("+-+-+-+-+-+-+-+-+-+-+-");
            Console.WriteLine("+-+-+-+-+-+-+-+-+-+-+");
            Font.SetTextToRedInline("|P|o|w|e|r| |t|o| |t|h");
            Console.WriteLine("|e| |C|r|e|a|t|o|r|s|");
            Font.SetTextToRedInline("+-+-+-+-+-+-+-+-+-+-+-");
            Console.WriteLine("+-+-+-+-+-+-+-+-+-+-+");
        }
    }
}
