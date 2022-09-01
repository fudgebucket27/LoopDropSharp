using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public class NftHolder
    {
        public int accountId { get; set; }
        public int tokenId { get; set; }
        public string amount { get; set; }
    }

    public class NftHoldersAndTotal
    {
        public int totalNum { get; set; }
        public List<NftHolder> nftHolders { get; set; }
    }

}
