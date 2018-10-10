using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Objects
{
    public class Notifications
    {
        public string Symbol { get; set; }
        public string Type { get; set; }
        public string Timeframe { get; set; }
        public string Change { get; set; }
        public string GUID { get; set; }
        public DateTime NotifyTime { get; set; }
    }
}
