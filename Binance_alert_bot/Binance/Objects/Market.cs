using Binance_plus.Binance.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_alert_bot.Binance.Objects
{
    public class Market
    {
        public bool AllTimeFrame = true;
        public string Symbol { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        /*public Dictionary<string, List<MarketTicks>> Ticks { get; set; } = new Dictionary<string, List<MarketTicks>>()
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

        };*/
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




        public MarketInfo PriceChange1min => GetPriceChange(Ticks1min, 1);
        public MarketInfo PriceChange3min => GetPriceChange(Ticks3min, 3);
        public MarketInfo PriceChange5min => GetPriceChange(Ticks5min, 5);
        public MarketInfo PriceChange15min => GetPriceChange(Ticks15min, 15);
        public MarketInfo PriceChange30min => GetPriceChange(Ticks30min, 30);
        public MarketInfo PriceChange1h => GetPriceChange(Ticks1h, 60);
        public MarketInfo PriceChange2h => GetPriceChange(Ticks2h, 60 * 2);
        public MarketInfo PriceChange4h => GetPriceChange(Ticks4h, 60 * 4);
        public MarketInfo PriceChange6h => GetPriceChange(Ticks6h, 60 * 6);
        public MarketInfo PriceChange12h => GetPriceChange(Ticks12h, 60 * 12);
        public MarketInfo PriceChange24h => GetPriceChange(Ticks24h, 60 * 24);

        public MarketInfo GetPriceChange(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            if (AllTimeFrame)
            {
                marketInfo.OldValue = Ticks[Ticks.Count - 2].Close;
                marketInfo.NewValue = Ticks.Last().Close;
            }
            else
            {
                marketInfo.OldValue = Ticks1min.Find(t => t.Time == Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).Close;
                marketInfo.NewValue = Ticks1min.Last().Close;
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎↓";
            }
            marketInfo.Text = $"{marketInfo.Emoji} Price = {((marketInfo.ChangeValueProcentage > 0) ? "+" : "")}{marketInfo.ChangeValueProcentage}% ({marketInfo.OldValue} → {marketInfo.NewValue})";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }


        public decimal High1min => GetHigh(Ticks1min, 1);
        public decimal High3min => GetHigh(Ticks3min, 3);
        public decimal High5min => GetHigh(Ticks5min, 5);
        public decimal High15min => GetHigh(Ticks15min, 15);
        public decimal High30min => GetHigh(Ticks30min, 30);
        public decimal High1h => GetHigh(Ticks1h, 60);
        public decimal High2h => GetHigh(Ticks2h, 60 * 2);
        public decimal High4h => GetHigh(Ticks4h, 60 * 4);
        public decimal High6h => GetHigh(Ticks6h, 60 * 6);
        public decimal High12h => GetHigh(Ticks12h, 60 * 12);
        public decimal High24h => GetHigh(Ticks24h, 60 * 24);

        public decimal GetHigh(List<MarketTicks> Ticks, int minutes)
        {
            if (AllTimeFrame)
            {
                return Ticks.Last().High;
            }
            else
            {
                return Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Max(h => h.High);
            }
        }


        public decimal Low1min => GetLow(Ticks1min, 1);
        public decimal Low3min => GetLow(Ticks3min, 3);
        public decimal Low5min => GetLow(Ticks5min, 5);
        public decimal Low15min => GetLow(Ticks15min, 15);
        public decimal Low30min => GetLow(Ticks30min, 30);
        public decimal Low1h => GetLow(Ticks1h, 60);
        public decimal Low2h => GetLow(Ticks2h, 60 * 2);
        public decimal Low4h => GetLow(Ticks4h, 60 * 4);
        public decimal Low6h => GetLow(Ticks6h, 60 * 6);
        public decimal Low12h => GetLow(Ticks12h, 60 * 12);
        public decimal Low24h => GetLow(Ticks24h, 60 * 24);

        public decimal GetLow(List<MarketTicks> Ticks, int minutes)
        {
            if (AllTimeFrame)
            {
                return Ticks.Last().High;
            }
            else
            {
                return Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Min(h => h.Low);
            }
        }



        public MarketInfo Amplitude1min => GetAmplitude(Ticks1min, 1);
        public MarketInfo Amplitude3min => GetAmplitude(Ticks3min, 3);
        public MarketInfo Amplitude5min => GetAmplitude(Ticks5min, 5);
        public MarketInfo Amplitude15min => GetAmplitude(Ticks15min, 15);
        public MarketInfo Amplitude30min => GetAmplitude(Ticks30min, 30);
        public MarketInfo Amplitude1h => GetAmplitude(Ticks1h, 60);
        public MarketInfo Amplitude2h => GetAmplitude(Ticks2h, 60 * 2);
        public MarketInfo Amplitude4h => GetAmplitude(Ticks4h, 60 * 4);
        public MarketInfo Amplitude6h => GetAmplitude(Ticks6h, 60 * 6);
        public MarketInfo Amplitude12h => GetAmplitude(Ticks12h, 60 * 12);
        public MarketInfo Amplitude24h => GetAmplitude(Ticks24h, 60 * 24);

        public MarketInfo GetAmplitude(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            if (AllTimeFrame)
            {
                marketInfo.OldValue = Ticks.Last().Low;
                marketInfo.NewValue = Ticks.Last().High;
            }
            else
            {
                marketInfo.OldValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Min(m => m.Low);
                marketInfo.NewValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Max(m => m.High);
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.Emoji = "🕯↑";
            }
            else
            {
                marketInfo.Emoji = "🕯↓";
            }
            marketInfo.Text = $"{marketInfo.Emoji} Amp = {marketInfo.ChangeValueProcentage}%";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }



        public decimal VolumeQuote1min => GetVolumeQuote(Ticks1min, 1);
        public decimal VolumeQuote3min => GetVolumeQuote(Ticks3min, 3);
        public decimal VolumeQuote5min => GetVolumeQuote(Ticks5min, 5);
        public decimal VolumeQuote15min => GetVolumeQuote(Ticks15min, 15);
        public decimal VolumeQuote30min => GetVolumeQuote(Ticks30min, 30);
        public decimal VolumeQuote1h => GetVolumeQuote(Ticks1h, 60);
        public decimal VolumeQuote2h => GetVolumeQuote(Ticks2h, 60 * 2);
        public decimal VolumeQuote4h => GetVolumeQuote(Ticks4h, 60 * 4);
        public decimal VolumeQuote6h => GetVolumeQuote(Ticks6h, 60 * 6);
        public decimal VolumeQuote12h => GetVolumeQuote(Ticks12h, 60 * 12);
        public decimal VolumeQuote24h => GetVolumeQuote(Ticks24h, 60 * 24);

        public decimal GetVolumeQuote(List<MarketTicks> Ticks, int minutes)
        {
            if (AllTimeFrame)
            {
                return Ticks.Last().VolumeQuote;
            }
            else
            {
                return Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Sum(h => h.VolumeQuote);
            }
        }



        public decimal VolumeBase1min => GetVolumeBase(Ticks1min, 1);
        public decimal VolumeBase3min => GetVolumeBase(Ticks3min, 3);
        public decimal VolumeBase5min => GetVolumeBase(Ticks5min, 5);
        public decimal VolumeBase15min => GetVolumeBase(Ticks15min, 15);
        public decimal VolumeBase30min => GetVolumeBase(Ticks30min, 30);
        public decimal VolumeBase1h => GetVolumeBase(Ticks1h, 60);
        public decimal VolumeBase2h => GetVolumeBase(Ticks2h, 60 * 2);
        public decimal VolumeBase4h => GetVolumeBase(Ticks4h, 60 * 4);
        public decimal VolumeBase6h => GetVolumeBase(Ticks6h, 60 * 6);
        public decimal VolumeBase12h => GetVolumeBase(Ticks12h, 60 * 12);
        public decimal VolumeBase24h => GetVolumeBase(Ticks24h, 60 * 24);

        public decimal GetVolumeBase(List<MarketTicks> Ticks, int minutes)
        {
            if (AllTimeFrame)
            {
                return Ticks.Last().VolumeBase;
            }
            else
            {
                return Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Sum(h => h.VolumeBase);
            }
        }

        public MarketInfo VolumeChange1min => GetVolumeChange(Ticks1min, 1);
        public MarketInfo VolumeChange3min => GetVolumeChange(Ticks3min, 3);
        public MarketInfo VolumeChange5min => GetVolumeChange(Ticks5min, 5);
        public MarketInfo VolumeChange15min => GetVolumeChange(Ticks15min, 15);
        public MarketInfo VolumeChange30min => GetVolumeChange(Ticks30min, 30);
        public MarketInfo VolumeChange1h => GetVolumeChange(Ticks1h, 60);
        public MarketInfo VolumeChange2h => GetVolumeChange(Ticks2h, 60 * 2);
        public MarketInfo VolumeChange4h => GetVolumeChange(Ticks4h, 60 * 4);
        public MarketInfo VolumeChange6h => GetVolumeChange(Ticks6h, 60 * 6);
        public MarketInfo VolumeChange12h => GetVolumeChange(Ticks12h, 60 * 12);
        public MarketInfo VolumeChange24h => GetVolumeChange(Ticks24h, 60 * 24);

        public MarketInfo GetVolumeChange(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            if (AllTimeFrame)
            {
                marketInfo.OldValue = Ticks[Ticks.Count - 2].VolumeQuote;
                marketInfo.NewValue = Ticks.Last().VolumeQuote;
            }
            else
            {
                marketInfo.OldValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-1 * minutes)).Sum(h => h.VolumeQuote);
                marketInfo.NewValue = Ticks1min.FindAll(t => t.Time >= DateTime.UtcNow.AddMinutes(-1 * minutes - 1)).Sum(h => h.VolumeQuote);
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.Emoji = "🍖↑";
            }
            else
            {
                marketInfo.Emoji = "🍗↓";
            }
            marketInfo.Text = $"{marketInfo.Emoji} Vol = {((marketInfo.ChangeValueProcentage > 0) ? "+" : "")}{marketInfo.ChangeValueProcentage}% ({marketInfo.OldValue} → {marketInfo.NewValue})";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }

        private string GetTimeframe(int minutes)
        {
            switch (minutes)
            {
                case 1:
                    return "1min";
                case 3:
                    return "3min";
                case 5:
                    return "5min";
                case 15:
                    return "15min";
                case 30:
                    return "30min";
                case 60:
                    return "1h";
                case 60*2:
                    return "2h";
                case 60*4:
                    return "4h";
                case 60*6:
                    return "6h";
                case 60 * 12:
                    return "12h";
                case 60 * 24:
                    return "24h";
                default:
                    return ""; 
            }
        }
    }
}
