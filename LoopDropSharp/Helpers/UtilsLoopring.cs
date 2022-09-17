using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Helpers
{
    public class UtilsLoopring
    {
        public static MinterAndCollection GetMinterAndCollection()
        {
            var result = new MinterAndCollection();
            Font.SetTextToBlue("Enter in the Minter");
            string minter = Utils.ReadLineWarningNoNulls("Enter in the Minter");
            Font.SetTextToBlue("Enter in the TokenId/Collection Address");
            string TokenId = Utils.ReadLineWarningNoNulls("Enter in the TokenId/Collection Address");
            result.minter = minter;
            result.TokenId = TokenId;
            return result;

        }
    }

    public class MinterAndCollection
    {
        public string minter { get; set; }
        public string TokenId { get; set; }
    }
}
