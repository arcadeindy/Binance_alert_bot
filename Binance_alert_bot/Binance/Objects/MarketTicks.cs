﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Binance.Objects
{
    public class MarketTicks
    {
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal VolumeQuote { get; set; }
        public decimal VolumeBase { get; set; }
    }
}
