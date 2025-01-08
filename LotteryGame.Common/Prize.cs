using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Common
{
    public struct Prize
    {
        public PrizeType PrizeType { get; set; }
        public decimal PrizeValue { get; set; }
    }
}
