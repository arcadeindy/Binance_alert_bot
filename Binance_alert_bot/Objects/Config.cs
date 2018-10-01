using Binance_alert_bot.Binance.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Objects
{
    public class Config
    {
        #region CheckBox
        public bool ask { get; set; } = true;
        public bool bid { get; set; } = true;
        public bool PriceChange1min { get; set; } = false;
        public bool PriceChange3min { get; set; } = false;
        public bool PriceChange5min { get; set; } = false;
        public bool PriceChange15min { get; set; } = false;
        public bool PriceChange30min { get; set; } = false;
        public bool PriceChange1h { get; set; } = false;
        public bool PriceChange2h { get; set; } = false;
        public bool PriceChange4h { get; set; } = false;
        public bool PriceChange6h { get; set; } = false;
        public bool PriceChange12h { get; set; } = false;
        public bool PriceChange24h { get; set; } = true;


        public bool Low1min { get; set; } = false;
        public bool Low3min { get; set; } = false;
        public bool Low5min { get; set; } = false;
        public bool Low15min { get; set; } = false;
        public bool Low30min { get; set; } = false;
        public bool Low1h { get; set; } = false;
        public bool Low2h { get; set; } = false;
        public bool Low4h { get; set; } = false;
        public bool Low6h { get; set; } = false;
        public bool Low12h { get; set; } = false;
        public bool Low24h { get; set; } = true;

        public bool High1min { get; set; } = false;
        public bool High3min { get; set; } = false;
        public bool High5min { get; set; } = false;
        public bool High15min { get; set; } = false;
        public bool High30min { get; set; } = false;
        public bool High1h { get; set; } = false;
        public bool High2h { get; set; } = false;
        public bool High4h { get; set; } = false;
        public bool High6h { get; set; } = false;
        public bool High12h { get; set; } = false;
        public bool High24h { get; set; } = true;

        public bool Volume1min { get; set; } = false;
        public bool Volume3min { get; set; } = false;
        public bool Volume5min { get; set; } = false;
        public bool Volume15min { get; set; } = false;
        public bool Volume30min { get; set; } = false;
        public bool Volume1h { get; set; } = false;
        public bool Volume2h { get; set; } = false;
        public bool Volume4h { get; set; } = false;
        public bool Volume6h { get; set; } = false;
        public bool Volume12h { get; set; } = false;
        public bool Volume24h { get; set; } = true;

        public bool VolumeChange1min { get; set; } = false;
        public bool VolumeChange3min { get; set; } = false;
        public bool VolumeChange5min { get; set; } = false;
        public bool VolumeChange15min { get; set; } = false;
        public bool VolumeChange30min { get; set; } = false;
        public bool VolumeChange1h { get; set; } = false;
        public bool VolumeChange2h { get; set; } = false;
        public bool VolumeChange4h { get; set; } = false;
        public bool VolumeChange6h { get; set; } = false;
        public bool VolumeChange12h { get; set; } = false;
        public bool VolumeChange24h { get; set; } = false;
        #endregion

        public List<Notifications> notifications { get; set; } = new List<Notifications>();
        public bool Favorite { get; set; } = false;
        public List<string> FavoriveSymbols { get; set; } = new List<string>();

        public string TelegramApiKey { get; set; }
        public string TelegramChatID { get; set; }

        public static Config Reload()
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        }

        public static void Save(Config cfg)
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(cfg));
        }
    }
}
