using Binance_plus.Binance.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binance_alert_bot.Binance.Objects
{
    public class Market
    {
        public bool used = false;
        public bool AllTimeFrame;
        public string Symbol { get; set; }
        public string BaseSymbol { get; set; }
        public string QuoteSymbol { get; set; }
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




        //public MarketInfo PriceChange1min { get; set; }
        //public MarketInfo PriceChange3min { get; set; }
        //public MarketInfo PriceChange5min { get; set; }
        //public MarketInfo PriceChange15min { get; set; }
        //public MarketInfo PriceChange30min { get; set; }
        //public MarketInfo PriceChange1h { get; set; }
        //public MarketInfo PriceChange2h { get; set; }
        //public MarketInfo PriceChange4h { get; set; }
        //public MarketInfo PriceChange6h { get; set; }
        //public MarketInfo PriceChange12h { get; set; }
        //public MarketInfo PriceChange24h { get; set; }

        public MarketInfo GetPrice(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                //marketInfo.OldValue = Ticks1min.Last().Close;
                marketInfo.NewValue = Bid;
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            /*if (marketInfo.ChangeValue >= 0)
            {
                //marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏";
            }
            else
            {
                //marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎";
            }*/
            marketInfo.Text = $"Price = {marketInfo.NewValue}";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }

        public MarketInfo GetPriceChange(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                if (AllTimeFrame)
                {
                    marketInfo.OldValue = Ticks[Ticks.Count - 2].Close;
                    marketInfo.NewValue = Ticks.Last().Close;
                }
                else
                {
                    marketInfo.OldValue = Ticks1min.FindAll(t => t.Time >= Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).First().Close;
                    marketInfo.NewValue = Ticks1min.Last().Close;
                }
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
            }

            marketInfo.Change = marketInfo.ChangeValueProcentage;
            marketInfo.Text = $"Price = {((marketInfo.ChangeValueProcentage > 0) ? "+" : "")}{marketInfo.ChangeValueProcentage}% ({marketInfo.OldValue} → {marketInfo.NewValue})";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }


        //public decimal High1min { get; set; }
        //public decimal High3min { get; set; }
        //public decimal High5min { get; set; }
        //public decimal High15min { get; set; }
        //public decimal High30min { get; set; }
        //public decimal High1h { get; set; }
        //public decimal High2h { get; set; }
        //public decimal High4h { get; set; }
        //public decimal High6h { get; set; }
        //public decimal High12h { get; set; }
        //public decimal High24h { get; set; }

        public MarketInfo GetHigh(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                if (AllTimeFrame)
                {
                    //marketInfo.OldValue = Ticks[Ticks.Count - 2].High;
                    marketInfo.NewValue = Ticks.Last().High;
                }
                else
                {
                    //marketInfo.OldValue = Ticks1min.FindAll(t => t.Time >= Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).First().Close;
                    marketInfo.NewValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Max(h => h.High); ;
                }
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            /*if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎↓";
            }*/
            marketInfo.Change = marketInfo.ChangeValue;
            marketInfo.Text = $"{marketInfo.Emoji} High = {marketInfo.NewValue}";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }


        //public decimal Low1min { get; set; }
        //public decimal Low3min { get; set; }
        //public decimal Low5min { get; set; }
        //public decimal Low15min { get; set; }
        //public decimal Low30min { get; set; }
        //public decimal Low1h { get; set; }
        //public decimal Low2h { get; set; }
        //public decimal Low4h { get; set; }
        //public decimal Low6h { get; set; }
        //public decimal Low12h { get; set; }
        //public decimal Low24h { get; set; }

        public MarketInfo GetLow(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                if (AllTimeFrame)
                {
                    //marketInfo.OldValue = Ticks[Ticks.Count - 2].Low;
                    marketInfo.NewValue = Ticks.Last().Low;
                }
                else
                {
                    //marketInfo.OldValue = Ticks1min.FindAll(t => t.Time >= Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).First().Close;
                    marketInfo.NewValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Min(h => h.Low);
                }
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            /*if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎↓";
            }*/
            marketInfo.Change = marketInfo.ChangeValue;
            marketInfo.Text = $"{marketInfo.Emoji} Low = {marketInfo.NewValue}";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }



        //public MarketInfo Amplitude1min { get; set; }
        //public MarketInfo Amplitude3min { get; set; }
        //public MarketInfo Amplitude5min { get; set; }
        //public MarketInfo Amplitude15min { get; set; }
        //public MarketInfo Amplitude30min { get; set; }
        //public MarketInfo Amplitude1h { get; set; }
        //public MarketInfo Amplitude2h { get; set; }
        //public MarketInfo Amplitude4h { get; set; }
        //public MarketInfo Amplitude6h { get; set; }
        //public MarketInfo Amplitude12h { get; set; }
        //public MarketInfo Amplitude24h { get; set; }

        public MarketInfo GetAmplitude(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
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
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;
                marketInfo.Emoji = "🕯↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🕯↓";
            }
            marketInfo.Change = marketInfo.ChangeValueProcentage;
            marketInfo.Text = $"{marketInfo.Emoji} Amp = {marketInfo.ChangeValueProcentage}%";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }



        public decimal VolumeQuote1min { get; set; }
        public decimal VolumeQuote3min { get; set; }
        public decimal VolumeQuote5min { get; set; }
        public decimal VolumeQuote15min { get; set; }
        public decimal VolumeQuote30min { get; set; }
        public decimal VolumeQuote1h { get; set; }
        public decimal VolumeQuote2h { get; set; }
        public decimal VolumeQuote4h { get; set; }
        public decimal VolumeQuote6h { get; set; }
        public decimal VolumeQuote12h { get; set; }
        public decimal VolumeQuote24h { get; set; }

        public MarketInfo GetVolumeQuote(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                if (AllTimeFrame)
                {
                    //marketInfo.OldValue = Ticks[Ticks.Count - 2].Low;
                    marketInfo.NewValue = Ticks.Last().VolumeQuote;
                }
                else
                {
                    //marketInfo.OldValue = Ticks1min.FindAll(t => t.Time >= Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).First().Close;
                    marketInfo.NewValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Sum(h => h.VolumeQuote);
                }
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            /*if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎↓";
            }*/
            marketInfo.Change = marketInfo.ChangeValue;
            marketInfo.Emoji = "🥒";
            marketInfo.Text = $"🥒 Volume BTC = {marketInfo.NewValue}";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }



        public decimal VolumeBase1min { get; set; }
        public decimal VolumeBase3min { get; set; }
        public decimal VolumeBase5min { get; set; }
        public decimal VolumeBase15min { get; set; }
        public decimal VolumeBase30min { get; set; }
        public decimal VolumeBase1h { get; set; }
        public decimal VolumeBase2h { get; set; }
        public decimal VolumeBase4h { get; set; }
        public decimal VolumeBase6h { get; set; }
        public decimal VolumeBase12h { get; set; }
        public decimal VolumeBase24h { get; set; }

        public MarketInfo GetVolumeBase(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
                if (AllTimeFrame)
                {
                    //marketInfo.OldValue = Ticks[Ticks.Count - 2].Low;
                    marketInfo.NewValue = Ticks.Last().VolumeBase;
                }
                else
                {
                    //marketInfo.OldValue = Ticks1min.FindAll(t => t.Time >= Ticks1min[Ticks1min.Count - 2].Time.AddMinutes(-1 * minutes + 1)).First().Close;
                    marketInfo.NewValue = Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * minutes)).Sum(h => h.VolumeBase);
                }
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            /*if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;

                marketInfo.Emoji = "🍏↑";
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
                marketInfo.Emoji = "🍎↓";
            }*/
            marketInfo.Change = marketInfo.ChangeValue;
            marketInfo.Text = $"Volume = {marketInfo.NewValue}";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }

        //public MarketInfo VolumeChange1min { get; set; }
        //public MarketInfo VolumeChange3min { get; set; }
        //public MarketInfo VolumeChange5min { get; set; }
        //public MarketInfo VolumeChange15min { get; set; }
        //public MarketInfo VolumeChange30min { get; set; }
        //public MarketInfo VolumeChange1h { get; set; }
        //public MarketInfo VolumeChange2h { get; set; }
        //public MarketInfo VolumeChange4h { get; set; }
        //public MarketInfo VolumeChange6h { get; set; }
        //public MarketInfo VolumeChange12h { get; set; }
        //public MarketInfo VolumeChange24h { get; set; }

        public MarketInfo GetVolumeChange(List<MarketTicks> Ticks, int minutes)
        {
            MarketInfo marketInfo = new MarketInfo();

            marketInfo.Price = Bid;

            try
            {
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
            }
            catch
            {
                marketInfo.OldValue = 0;
                marketInfo.NewValue = 0;
            }

            if (marketInfo.ChangeValueProcentage >= 0)
            {
                marketInfo.BackColorColumn = Color.Green;
            }
            else
            {
                marketInfo.BackColorColumn = Color.Red;
            }
            marketInfo.Change = marketInfo.ChangeValueProcentage;
            marketInfo.Text = $"Vol = {((marketInfo.ChangeValueProcentage > 0) ? "+" : "")}{marketInfo.ChangeValueProcentage}% ({marketInfo.OldValue} → {marketInfo.NewValue})";
            marketInfo.TimeFrame = GetTimeframe(minutes);

            return marketInfo;
        }

        public Dictionary<string, MarketInfo> MI;

        public void Update()
        {
            try
            {
                MI = new Dictionary<string, MarketInfo>();

                MI.Add("Price", GetPrice(null, 1));

                MI.Add("PriceChange1min", GetPriceChange(Ticks1min, 1));
                MI.Add("PriceChange3min", GetPriceChange(Ticks3min, 3));
                MI.Add("PriceChange5min", GetPriceChange(Ticks5min, 5));
                MI.Add("PriceChange15min", GetPriceChange(Ticks15min, 15));
                MI.Add("PriceChange30min", GetPriceChange(Ticks30min, 30));
                MI.Add("PriceChange1h", GetPriceChange(Ticks1h, 60));
                MI.Add("PriceChange2h", GetPriceChange(Ticks2h, 60 * 2));
                MI.Add("PriceChange4h", GetPriceChange(Ticks4h, 60 * 4));
                MI.Add("PriceChange6h", GetPriceChange(Ticks6h, 60 * 6));
                MI.Add("PriceChange12h", GetPriceChange(Ticks12h, 60 * 12));
                MI.Add("PriceChange24h", GetPriceChange(Ticks24h, 60 * 24));

                MI.Add("High1min", GetHigh(Ticks1min, 1));
                MI.Add("High3min", GetHigh(Ticks3min, 3));
                MI.Add("High5min", GetHigh(Ticks5min, 5));
                MI.Add("High15min", GetHigh(Ticks15min, 15));
                MI.Add("High30min", GetHigh(Ticks30min, 30));
                MI.Add("High1h", GetHigh(Ticks1h, 60));
                MI.Add("High2h", GetHigh(Ticks2h, 60 * 2));
                MI.Add("High4h", GetHigh(Ticks4h, 60 * 4));
                MI.Add("High6h", GetHigh(Ticks6h, 60 * 6));
                MI.Add("High12h", GetHigh(Ticks12h, 60 * 12));
                MI.Add("High24h", GetHigh(Ticks24h, 60 * 24));

                MI.Add("Low1min", GetLow(Ticks1min, 1));
                MI.Add("Low3min", GetLow(Ticks3min, 3));
                MI.Add("Low5min", GetLow(Ticks5min, 5));
                MI.Add("Low15min", GetLow(Ticks15min, 15));
                MI.Add("Low30min", GetLow(Ticks30min, 30));
                MI.Add("Low1h", GetLow(Ticks1h, 60));
                MI.Add("Low2h", GetLow(Ticks2h, 60 * 2));
                MI.Add("Low4h", GetLow(Ticks4h, 60 * 4));
                MI.Add("Low6h", GetLow(Ticks6h, 60 * 6));
                MI.Add("Low12h", GetLow(Ticks12h, 60 * 12));
                MI.Add("Low24h", GetLow(Ticks24h, 60 * 24));

                MI.Add("Amplitude1min", GetAmplitude(Ticks1min, 1));
                MI.Add("Amplitude3min", GetAmplitude(Ticks3min, 3));
                MI.Add("Amplitude5min", GetAmplitude(Ticks5min, 5));
                MI.Add("Amplitude15min", GetAmplitude(Ticks15min, 15));
                MI.Add("Amplitude30min", GetAmplitude(Ticks30min, 30));
                MI.Add("Amplitude1h", GetAmplitude(Ticks1h, 60));
                MI.Add("Amplitude2h", GetAmplitude(Ticks2h, 60 * 2));
                MI.Add("Amplitude4h", GetAmplitude(Ticks4h, 60 * 4));
                MI.Add("Amplitude6h", GetAmplitude(Ticks6h, 60 * 6));
                MI.Add("Amplitude12h", GetAmplitude(Ticks12h, 60 * 12));
                MI.Add("Amplitude24h", GetAmplitude(Ticks24h, 60 * 24));

                MI.Add("VolumeQuote1min", GetVolumeQuote(Ticks1min, 1));
                MI.Add("VolumeQuote3min", GetVolumeQuote(Ticks3min, 3));
                MI.Add("VolumeQuote5min", GetVolumeQuote(Ticks5min, 5));
                MI.Add("VolumeQuote15min", GetVolumeQuote(Ticks15min, 15));
                MI.Add("VolumeQuote30min", GetVolumeQuote(Ticks30min, 30));
                MI.Add("VolumeQuote1h", GetVolumeQuote(Ticks1h, 60));
                MI.Add("VolumeQuote2h", GetVolumeQuote(Ticks2h, 60 * 2));
                MI.Add("VolumeQuote4h", GetVolumeQuote(Ticks4h, 60 * 4));
                MI.Add("VolumeQuote6h", GetVolumeQuote(Ticks6h, 60 * 6));
                MI.Add("VolumeQuote12h", GetVolumeQuote(Ticks12h, 60 * 12));
                MI.Add("VolumeQuote24h", GetVolumeQuote(Ticks24h, 60 * 24));

                MI.Add("VolumeBase1min", GetVolumeBase(Ticks1min, 1));
                MI.Add("VolumeBase3min", GetVolumeBase(Ticks3min, 3));
                MI.Add("VolumeBase5min", GetVolumeBase(Ticks5min, 5));
                MI.Add("VolumeBase15min", GetVolumeBase(Ticks15min, 15));
                MI.Add("VolumeBase30min", GetVolumeBase(Ticks30min, 30));
                MI.Add("VolumeBase1h", GetVolumeBase(Ticks1h, 60));
                MI.Add("VolumeBase2h", GetVolumeBase(Ticks2h, 60 * 2));
                MI.Add("VolumeBase4h", GetVolumeBase(Ticks4h, 60 * 4));
                MI.Add("VolumeBase6h", GetVolumeBase(Ticks6h, 60 * 6));
                MI.Add("VolumeBase12h", GetVolumeBase(Ticks12h, 60 * 12));
                MI.Add("VolumeBase24h", GetVolumeBase(Ticks24h, 60 * 24));

                MI.Add("VolumeChange1min", GetVolumeChange(Ticks1min, 1));
                MI.Add("VolumeChange3min", GetVolumeChange(Ticks3min, 3));
                MI.Add("VolumeChange5min", GetVolumeChange(Ticks5min, 5));
                MI.Add("VolumeChange15min", GetVolumeChange(Ticks15min, 15));
                MI.Add("VolumeChange30min", GetVolumeChange(Ticks30min, 30));
                MI.Add("VolumeChange1h", GetVolumeChange(Ticks1h, 60));
                MI.Add("VolumeChange2h", GetVolumeChange(Ticks2h, 60 * 2));
                MI.Add("VolumeChange4h", GetVolumeChange(Ticks4h, 60 * 4));
                MI.Add("VolumeChange6h", GetVolumeChange(Ticks6h, 60 * 6));
                MI.Add("VolumeChange12h", GetVolumeChange(Ticks12h, 60 * 12));
                MI.Add("VolumeChange24h", GetVolumeChange(Ticks24h, 60 * 24));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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

        private string ConvertToReadebleInt(string number)
        {
            return number;
        }
    }
}
