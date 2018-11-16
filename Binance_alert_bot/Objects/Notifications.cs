using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Objects
{
    public class Notifications
    {
        public Dictionary<string, List<Notification>> Guid { get; set; } = new Dictionary<string, List<Notification>>();
        
    }
    public class Notification
    {
        public List<string> Symbol { get; set; }
        public string Type { get; set; }
        public string Timeframe { get; set; }
        public string Change { get; set; }
        public DateTime Time { get; set; } = DateTime.MinValue;
        public long TelegramChatId { get; set; }
        public string Guid { get; set; }
    }
}
