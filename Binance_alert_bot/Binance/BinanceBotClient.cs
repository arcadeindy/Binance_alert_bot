using Binance.Net;
using Binance.Net.Objects;
using Binance_alert_bot.Binance.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binance_alert_bot.Binance
{
    public class BinanceBotClient
    {
        #region Delegates
        public event LogsStateHandler Logs;
        public delegate void LogsStateHandler(string message);

        public event SymbolsStateHandler SymbolsEvent;
        public delegate void SymbolsStateHandler(List<string> symbols);
        #endregion

        #region Fields
        public List<string> AllSymbolsBTC = new List<string>();
        public volatile List<Market> BinanceMarket = new List<Market>();
        #endregion

        #region Properties
        private BinanceClient client;
        private BinanceSocketClient ws;
        private BinanceSymbol[] exchangeInfo;
        #endregion

        #region Constructor/Destructor
        public BinanceBotClient()
        {

        }
        public void BinanceSetCredentials()
        {
            Logs?.Invoke("Подключение...");
            client = new BinanceClient();

            ws = new BinanceSocketClient();

            GetExchangeInfo();
            Logs?.Invoke("Получил всю инфо о бриже");

            Task.Run(() => SubscribeToNewSymbolAsync(AllSymbolsBTC)).Wait();

            Logs?.Invoke("Подключил сокеты всех монет");

            Task.Run(() => GetKlines());

            Task.Run(() => DeleteTicks());
            Task.Run(() => Notification());

        }

        #endregion

        #region Methods
        private void Notification()
        {
            while (true)
            {
                foreach (var market in BinanceMarket.ToArray())
                {
                    if (market.Ticks["1min"].Count < 60 * 24 * 2 - 1
                     || market.Ticks["3min"].Count < 2
                     || market.Ticks["5min"].Count < 2
                     || market.Ticks["15min"].Count < 2
                     || market.Ticks["30min"].Count < 2
                     || market.Ticks["1h"].Count < 2
                     || market.Ticks["2h"].Count < 2
                     || market.Ticks["4h"].Count < 2
                     || market.Ticks["6h"].Count < 2
                     || market.Ticks["12h"].Count < 2
                     || market.Ticks["24h"].Count < 2)
                        continue;


                    market.Ticks["1min"] = market.Ticks["1min"].OrderBy(x => x.Time).ToList();
                    market.Ticks["3min"] = market.Ticks["3min"].OrderBy(x => x.Time).ToList();
                    market.Ticks["5min"] = market.Ticks["5min"].OrderBy(x => x.Time).ToList();
                    market.Ticks["15min"] = market.Ticks["15min"].OrderBy(x => x.Time).ToList();
                    market.Ticks["30min"] = market.Ticks["30min"].OrderBy(x => x.Time).ToList();
                    market.Ticks["1h"] = market.Ticks["1h"].OrderBy(x => x.Time).ToList();
                    market.Ticks["2h"] = market.Ticks["2h"].OrderBy(x => x.Time).ToList();
                    market.Ticks["4h"] = market.Ticks["4h"].OrderBy(x => x.Time).ToList();
                    market.Ticks["6h"] = market.Ticks["6h"].OrderBy(x => x.Time).ToList();
                    market.Ticks["12h"] = market.Ticks["12h"].OrderBy(x => x.Time).ToList();
                    market.Ticks["24h"] = market.Ticks["24h"].OrderBy(x => x.Time).ToList();

                    var dt = LoadBinanceTable()

                }
            }
        }

        private void DeleteTicks()
        {
            while (true)
            {
                foreach (var symbol in BinanceMarket.ToArray())
                {
                    symbol.Ticks["1min"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 60 * 24 * 2 - 10));
                    symbol.Ticks["3min"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 3 * 2 - 1));
                    symbol.Ticks["5min"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 5 * 2 - 1));
                    symbol.Ticks["15min"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 15 * 2 - 1));
                    symbol.Ticks["30min"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 30 * 2 - 1));
                    symbol.Ticks["1h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 1 * 60 * 2 - 1));
                    symbol.Ticks["2h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 2 * 60 * 2 - 1));
                    symbol.Ticks["4h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 4 * 60 * 2 - 1));
                    symbol.Ticks["6h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 6 * 60 * 2 - 1));
                    symbol.Ticks["12h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 12 * 60 * 2 - 1));
                    symbol.Ticks["24h"].RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 24 * 60 * 2 - 1));
                }
                Thread.Sleep(1000 * 60);
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
                Logs?.Invoke($"Подписался на новую монету {symbol[0]}");
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
                            findCoin.Ticks["1min"].Add(
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
                            findCoin.Ticks["1min"].Last().Time = data.EventTime;
                            findCoin.Ticks["1min"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1min"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1min"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1min"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1min"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["1min"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["3min"].Add(
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
                            findCoin.Ticks["3min"].Last().Time = data.EventTime;
                            findCoin.Ticks["3min"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["3min"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["3min"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["3min"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["3min"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["3min"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["5min"].Add(
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
                            findCoin.Ticks["5min"].Last().Time = data.EventTime;
                            findCoin.Ticks["5min"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["5min"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["5min"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["5min"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["5min"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["5min"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["15min"].Add(
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
                            findCoin.Ticks["15min"].Last().Time = data.EventTime;
                            findCoin.Ticks["15min"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["15min"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["15min"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["15min"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["15min"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["15min"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["30min"].Add(
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
                            findCoin.Ticks["30min"].Last().Time = data.EventTime;
                            findCoin.Ticks["30min"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["30min"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["30min"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["30min"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["30min"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["30min"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["1h"].Add(
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
                            findCoin.Ticks["1h"].Last().Time = data.EventTime;
                            findCoin.Ticks["1h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["1h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["1h"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["2h"].Add(
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
                            findCoin.Ticks["2h"].Last().Time = data.EventTime;
                            findCoin.Ticks["2h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["2h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["2h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["2h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["2h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["2h"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["4h"].Add(
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
                            findCoin.Ticks["4h"].Last().Time = data.EventTime;
                            findCoin.Ticks["4h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["4h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["4h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["4h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["4h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["4h"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["6h"].Add(
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
                            findCoin.Ticks["6h"].Last().Time = data.EventTime;
                            findCoin.Ticks["6h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["6h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["6h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["6h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["6h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["6h"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["12h"].Add(
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
                            findCoin.Ticks["12h"].Last().Time = data.EventTime;
                            findCoin.Ticks["12h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["12h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["12h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["12h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["12h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["12h"].Last().VolumeBase = data.Data.Volume;
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
                            findCoin.Ticks["24h"].Add(
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
                            findCoin.Ticks["24h"].Last().Time = data.EventTime;
                            findCoin.Ticks["24h"].Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["24h"].Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["24h"].Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["24h"].Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks["24h"].Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks["24h"].Last().VolumeBase = data.Data.Volume;
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
                        findCoin.Ticks["1min"].AddRange(MarketTicks);
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
                        findCoin.Ticks["3min"].AddRange(MarketTicks);
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
                        findCoin.Ticks["5min"].AddRange(MarketTicks);
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
                        findCoin.Ticks["15min"].AddRange(MarketTicks);
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
                        findCoin.Ticks["30min"].AddRange(MarketTicks);
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
                        findCoin.Ticks["1h"].AddRange(MarketTicks);
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
                        findCoin.Ticks["2h"].AddRange(MarketTicks);
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
                        findCoin.Ticks["4h"].AddRange(MarketTicks);
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
                        findCoin.Ticks["6h"].AddRange(MarketTicks);
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
                        findCoin.Ticks["12h"].AddRange(MarketTicks);
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
                        findCoin.Ticks["24h"].AddRange(MarketTicks);
                    }
                }

                Logs?.Invoke($"История свечей по {symbol.Replace("BTC", "/BTC")}");
            }
            Logs?.Invoke("Получил историю свечей");
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
                SymbolsEvent?.Invoke(AllSymbolsBTC);
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
            Logs?.Invoke($"{error.Code}: {error.Message}");
        }
        #endregion
    }
}
