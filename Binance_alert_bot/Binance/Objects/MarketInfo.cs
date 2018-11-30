using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_plus.Binance.Objects
{
    public class MarketInfo
    {
        public Color BackColorColumn { get; set; } = Color.White;
        public string Emoji { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
        public decimal ChangeValue => NewValue - OldValue;
        public decimal ChangeValueProcentage => GetProfit(OldValue, NewValue);
        public string Text { get; set; } = "";
        public string TimeFrame { get; set; }

        private decimal GetProfit(decimal first, decimal last)
        {
            if (first == 0)
                first = 1;
            if (last == 0)
                last = 1;
            return Math.Round(last * 100 / first - 100, 2);
        }
    }
}
