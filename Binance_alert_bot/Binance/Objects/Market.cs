using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Binance.Objects
{
    public class Market
    {
        public string Symbol { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public List<MarketTicks> Ticks { get; set; } = new List<MarketTicks>();
    }
}
