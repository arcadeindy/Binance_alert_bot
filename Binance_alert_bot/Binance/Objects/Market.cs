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
        public List<MarketTicks> Ticks1min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks3min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks5min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks15min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks30min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks1h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks2h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks4h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks6h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks12h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks24h { get; set; } = new List<MarketTicks>();
    }
}
