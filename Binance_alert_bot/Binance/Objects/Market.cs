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
        public Dictionary<string, List<MarketTicks>> Ticks { get; set; } = new Dictionary<string, List<MarketTicks>>()
        {
            { "1min", new List<MarketTicks>() },
            { "3min", new List<MarketTicks>() },
            { "5min", new List<MarketTicks>() },
            { "15min", new List<MarketTicks>() },
            { "30min", new List<MarketTicks>() },
            { "1h", new List<MarketTicks>() },
            { "2h", new List<MarketTicks>() },
            { "4h", new List<MarketTicks>() },
            { "6h", new List<MarketTicks>() },
            { "12h", new List<MarketTicks>() },
            { "24h", new List<MarketTicks>() }

        };
        /*public List<MarketTicks> Ticks1min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks3min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks5min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks15min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks30min { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks1h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks2h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks4h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks6h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks12h { get; set; } = new List<MarketTicks>();
        public List<MarketTicks> Ticks24h { get; set; } = new List<MarketTicks>();*/
    }
}
