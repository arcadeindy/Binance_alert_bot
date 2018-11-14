using Binance.Net;
using Binance.Net.Objects;
using Binance_alert_bot.Binance.Objects;
using Binance_alert_bot.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Binance_alert_bot.Binance
{
    public class BinanceBotClient
    {
        #region Delegates
        public event BinanceLogsStateHandler BinanceLogs;
        public delegate void BinanceLogsStateHandler(string message);

        public event BinanceSymbolsStateHandler BinanceSymbols;
        public delegate void BinanceSymbolsStateHandler(List<string> Symbols);

        public event BinanceMarketStateHandler BinanceMarkets;
        public delegate void BinanceMarketStateHandler(Market market);
        #endregion

        #region Fields
        public List<string> AllSymbolsBTC = new List<string>();
        public volatile List<Market> BinanceMarket = new List<Market>();
        #endregion

        #region Properties
        private BinanceClient client;
        private BinanceSocketClient ws;
        private BinanceSymbol[] exchangeInfo;
        private Config cfg;
        #endregion

        #region Constructor/Destructor
        public BinanceBotClient()
        {

        }
        public void BinanceSetCredentials()
        {
            BinanceLogs?.Invoke("Подключение...");
            client = new BinanceClient();

            ws = new BinanceSocketClient();

            GetExchangeInfo();
            BinanceLogs?.Invoke("Получил всю инфо о бриже");

            Task.Run(() => SubscribeToNewSymbolAsync(AllSymbolsBTC)).Wait();

            BinanceLogs?.Invoke("Подключил сокеты всех монет");

            Task.Run(() => GetKlines());

            Task.Run(() => DeleteTicks());
            Task.Run(() => Notification());

        }
        public void UpdateNotifications(Config cfg)
        {
            this.cfg = cfg;
        }
        #endregion

        #region Methods
        private void Notification()
        {
            while (true)
            {
                foreach (var market in BinanceMarket.ToList())
                {
                    if (market.Ticks1min.Count < 60 * 24 * 2 - 1
                     || market.Ticks3min.Count < 2
                     || market.Ticks5min.Count < 2
                     || market.Ticks15min.Count < 2
                     || market.Ticks30min.Count < 2
                     || market.Ticks1h.Count < 2
                     || market.Ticks2h.Count < 2
                     || market.Ticks4h.Count < 2
                     || market.Ticks6h.Count < 2
                     || market.Ticks12h.Count < 2
                     || market.Ticks24h.Count < 2)
                        continue;


                    market.Ticks1min = market.Ticks1min.OrderBy(x => x.Time).ToList();
                    market.Ticks3min = market.Ticks3min.OrderBy(x => x.Time).ToList();
                    market.Ticks5min = market.Ticks5min.OrderBy(x => x.Time).ToList();
                    market.Ticks15min = market.Ticks15min.OrderBy(x => x.Time).ToList();
                    market.Ticks30min = market.Ticks30min.OrderBy(x => x.Time).ToList();
                    market.Ticks1h = market.Ticks1h.OrderBy(x => x.Time).ToList();
                    market.Ticks2h = market.Ticks2h.OrderBy(x => x.Time).ToList();
                    market.Ticks4h = market.Ticks4h.OrderBy(x => x.Time).ToList();
                    market.Ticks6h = market.Ticks6h.OrderBy(x => x.Time).ToList();
                    market.Ticks12h = market.Ticks12h.OrderBy(x => x.Time).ToList();
                    market.Ticks24h = market.Ticks24h.OrderBy(x => x.Time).ToList();
                    market.Update();

                    BinanceMarkets?.Invoke(market);

                    foreach (var notify in cfg.notifications.Guid)
                    {
                        string text = "";
                        string emoje = "";
                        foreach (var n in notify.Value)
                        {
                            if (n.Type == "Ask" || n.Type == "Bid")
                                n.Timeframe = "";

                            if (!n.Symbol.Contains(market.Symbol))
                                continue;

                            if ((DateTime.Now - n.Time).TotalSeconds < cfg.Delay)
                                continue;

                            var formula = n.Change.Split(' ');

                            string operand = formula[0].ToString();

                            decimal change = Convert.ToDecimal(formula[1].ToString());

                            string endSymbol = formula[2].ToString();

                            if (market.MI[n.Type + n.Timeframe].ChangeValue < change && operand == "<")
                            {
                                emoje += $"↑{market.MI[n.Type + n.Timeframe].Emoji} ";
                                text += $"{market.MI[n.Type + n.Timeframe].Text}\n";

                            }
                            if (market.MI[n.Type + n.Timeframe].ChangeValue > change && operand == ">")
                            {
                                emoje += $"↓{market.MI[n.Type + n.Timeframe].Emoji} ";
                                text += $"{market.MI[n.Type + n.Timeframe].Text}\n";
                            }
                        }

                        if (text != "")
                        {
                            text = $"{emoje}\n{text}\n";

                            TelegramBotClient bot = new TelegramBotClient(cfg.TelegramApiKey);
                            bot.SendTextMessageAsync(notify.Value.First().TelegramChatId, text, parseMode: ParseMode.Markdown);
                        }
                    }
                    
                }
            }
        }

        private void DeleteTicks()
        {
            while (true)
            {
                foreach (var symbol in BinanceMarket.ToArray())
                {
                    symbol.Ticks1min.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 60 * 24 * 2 - 10));
                    symbol.Ticks3min.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 3 * 2 - 1));
                    symbol.Ticks5min.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 5 * 2 - 1));
                    symbol.Ticks15min.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 15 * 2 - 1));
                    symbol.Ticks30min.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 30 * 2 - 1));
                    symbol.Ticks1h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 60 * 2 - 1));
                    symbol.Ticks2h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 2 * 60 * 2 - 1));
                    symbol.Ticks4h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 4 * 60 * 2 - 1));
                    symbol.Ticks6h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 6 * 60 * 2 - 1));
                    symbol.Ticks12h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 12 * 60 * 2 - 1));
                    symbol.Ticks24h.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 24 * 60 * 2 - 1));
                }
                Thread.Sleep(1000);
            }
        }
        private void SubscribeToNewSymbolAsync(List<string> symbol, int SubscribeDeleyHour = 0)
        {
            if (symbol.Count > 1)
                //сокеты для всех символов
                ws.SubscribeToAllSymbolTicker((data) =>
                {
                    try
                    {
                        foreach (var coin in data)
                        {
                            if (!coin.Symbol.Contains("BTC") || coin.Symbol == "BTCUSDT")
                                continue;

                            var findCoin = BinanceMarket.Find(x => x.Symbol == coin.Symbol);

                            if (findCoin == null)
                            {
                                BinanceMarket.Add(
                                    new Market
                                    {
                                        Symbol = coin.Symbol,
                                        Bid = Math.Round(coin.BestBidPrice, GetPriceFilter(coin.Symbol)),
                                        Ask = Math.Round(coin.BestAskPrice, GetPriceFilter(coin.Symbol))
                                    });
                            }
                            else
                            {
                                findCoin.Bid = Math.Round(coin.BestBidPrice, GetPriceFilter(coin.Symbol));
                                findCoin.Ask = Math.Round(coin.BestAskPrice, GetPriceFilter(coin.Symbol));
                            }
                        }
                    }
                    catch { }
                });
            else
            {
                ws.SubscribeToSymbolTicker(symbol[0], data =>
                {
                    try
                    {
                        if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT")
                            return;

                        var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);

                        if (findCoin == null)
                        {
                            BinanceMarket.Add(
                                new Market
                                {
                                    Symbol = data.Symbol,
                                    Bid = Math.Round(data.BestBidPrice, GetPriceFilter(data.Symbol)),
                                    Ask = Math.Round(data.BestAskPrice, GetPriceFilter(data.Symbol))
                                });
                        }
                        else
                        {
                            findCoin.Bid = Math.Round(data.BestBidPrice, GetPriceFilter(data.Symbol));
                            findCoin.Ask = Math.Round(data.BestAskPrice, GetPriceFilter(data.Symbol));
                        }
                    }
                    catch { }
                });
                BinanceLogs?.Invoke($"Подписался на новую монету {symbol[0]}");
            }

            //сокеты свечей на 1 минуту
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.OneMinute, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks1min.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks1min.Last().Time = data.EventTime;
                            findCoin.Ticks1min.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1min.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1min.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1min.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1min.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks1min.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 3 минуты
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.ThreeMinutes, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks3min.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks3min.Last().Time = data.EventTime;
                            findCoin.Ticks3min.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks3min.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks3min.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks3min.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks3min.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks3min.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 5 минут
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.FiveMinutes, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks5min.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks5min.Last().Time = data.EventTime;
                            findCoin.Ticks5min.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks5min.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks5min.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks5min.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks5min.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks5min.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 15 минут
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.FiveteenMinutes, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks15min.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks15min.Last().Time = data.EventTime;
                            findCoin.Ticks15min.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks15min.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks15min.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks15min.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks15min.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks15min.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 30 минут
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.ThirtyMinutes, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks30min.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks30min.Last().Time = data.EventTime;
                            findCoin.Ticks30min.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks30min.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks30min.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks30min.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks30min.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks30min.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 1 час
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.OneHour, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks1h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks1h.Last().Time = data.EventTime;
                            findCoin.Ticks1h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks1h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks1h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 2 h
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.TwoHour, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks2h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks2h.Last().Time = data.EventTime;
                            findCoin.Ticks2h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks2h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks2h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks2h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks2h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks2h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 4 h
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.FourHour, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks4h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks4h.Last().Time = data.EventTime;
                            findCoin.Ticks4h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks4h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks4h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks4h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks4h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks4h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 6 h
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.SixHour, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {
                        if (data.Data.Final)
                        {
                            findCoin.Ticks6h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks6h.Last().Time = data.EventTime;
                            findCoin.Ticks6h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks6h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks6h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks6h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks6h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks6h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });

            //сокеты свечей на 12 h
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.TwelfHour, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks12h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks12h.Last().Time = data.EventTime;
                            findCoin.Ticks12h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks12h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks12h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks12h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks12h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks12h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });
            //сокеты свечей на 24 h
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.OneDay, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC") || data.Symbol == "BTCUSDT") return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {

                        if (data.Data.Final)
                        {
                            findCoin.Ticks24h.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol)),
                                    Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol)),
                                    Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol)),
                                    High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol)),
                                    VolumeQuote = data.Data.QuoteAssetVolume,
                                    VolumeBase = data.Data.Volume
                                });
                        }
                        else
                        {
                            findCoin.Ticks24h.Last().Time = data.EventTime;
                            findCoin.Ticks24h.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks24h.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks24h.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks24h.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks24h.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks24h.Last().VolumeBase = data.Data.Volume;
                        }
                    }
                }
                catch { }
            });
        }
        private void GetKlines()
        {
            foreach (var symbol in AllSymbolsBTC)
            {
                int requests = -2 * 24 * 60 - 10;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.OneMinute, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks1min.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 3 - 10;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.ThreeMinutes, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks3min.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 5 - 10;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.FiveMinutes, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks5min.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 15 - 10;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.FiveteenMinutes, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks15min.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 30 - 10;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.ThirtyMinutes, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks30min.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.OneHour, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks1h.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 2 *60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.TwoHour, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks2h.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 4 * 60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.FourHour, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks4h.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 6 * 60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.SixHour, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks6h.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 12 * 60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.TwelfHour, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume
                            });
                    }

                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks12h.AddRange(MarketTicks);
                    }
                }

                requests = -2 * 24 * 60 - 1;
                while (requests < 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.OneDay, startTime: DateTime.UtcNow.AddMinutes(requests), limit: 1000);

                    requests += 1000;


                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = Math.Round(kline.Close, GetPriceFilter(symbol)),
                                Open = Math.Round(kline.Open, GetPriceFilter(symbol)),
                                High = Math.Round(kline.High, GetPriceFilter(symbol)),
                                Low = Math.Round(kline.Low, GetPriceFilter(symbol)),
                                VolumeQuote = kline.QuoteAssetVolume,
                                VolumeBase = kline.Volume,
                            });
                    }
                    start:
                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);
                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol
                            });
                        goto start;
                    }
                    else
                    {
                        findCoin.Ticks24h.AddRange(MarketTicks);
                    }
                }

                BinanceLogs?.Invoke($"История свечей по {symbol.Replace("BTC", "/BTC")}");
            }
            BinanceLogs?.Invoke("Получил историю свечей");
        }
        private void GetExchangeInfo(string Symbol = "")
        {
            start:
            var info = client.GetExchangeInfo();
            if (info.Success)
            {
                exchangeInfo = info.Data.Symbols;

                if (Symbol != String.Empty && Symbol.Contains("BTC") && Symbol != "BTCUSDT")
                        Task.Run(() => SubscribeToNewSymbolAsync(new List<string>() { Symbol }));


                foreach (var symbol in exchangeInfo.ToArray())
                {
                    if (symbol.Name.Contains("BTC") && symbol.Name != "BTCUSDT")
                    {
                        AllSymbolsBTC.Add(symbol.Name);
                    }
                }
                BinanceSymbols?.Invoke(AllSymbolsBTC);
            }
            else
            {
                ErrorHandling(info.Error);
                goto start;
            }
        }
        private int GetPriceFilter(string Symbol)
        {
            start:
            var findCoin = exchangeInfo.FirstOrDefault(x => x.Name == Symbol);
            if (findCoin == null)
            {
                GetExchangeInfo(Symbol);
                goto start;
                //throw new Exception($"{Symbol} не найдена в списке всех монет. Обновил");
            }
            else
            {
                BinanceSymbolPriceFilter filter = (BinanceSymbolPriceFilter)findCoin.Filters[0];
                return filter.TickSize.ToString().Replace(",", "").IndexOf("1");
            }
        }
        private void ErrorHandling(Error error)
        {
            BinanceLogs?.Invoke($"{error.Code}: {error.Message}");
        }
        #endregion
    }
}
