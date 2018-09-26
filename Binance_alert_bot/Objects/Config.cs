using Binance_alert_bot.Binance.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Objects
{
    public class Config
    {
        public dgMarketSettings dgMarketSettings { get; set; }
        public dgNotifications dgNotifications { get; set; }
        public Market market { get; set; }
    }
}
