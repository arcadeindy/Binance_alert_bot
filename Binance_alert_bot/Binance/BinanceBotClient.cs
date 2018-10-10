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

            Logs?.Invoke("Подключено");

            while (true)
            {
                foreach(var symbol in BinanceMarket.ToArray())
                    symbol.Ticks.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 60 * 24 * 2 - 10));
                Thread.Sleep(1000 * 60);
            }
        }

        #endregion

        #region Methods
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
                                        Ask = Math.Round(coin.BestAskPrice, GetPriceFilter(coin.Symbol)),
                                        Ticks = new List<MarketTicks>()
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
                                    Ask = Math.Round(data.BestAskPrice, GetPriceFilter(data.Symbol)),
                                    Ticks = new List<MarketTicks>()
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
                            findCoin.Ticks.Add(
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
                            findCoin.Ticks.Last().Time = data.EventTime;
                            findCoin.Ticks.Last().Close = Math.Round(data.Data.Close, GetPriceFilter(data.Symbol));
                            findCoin.Ticks.Last().Open = Math.Round(data.Data.Open, GetPriceFilter(data.Symbol));
                            findCoin.Ticks.Last().Low = Math.Round(data.Data.Low, GetPriceFilter(data.Symbol));
                            findCoin.Ticks.Last().High = Math.Round(data.Data.High, GetPriceFilter(data.Symbol));
                            findCoin.Ticks.Last().VolumeQuote = data.Data.QuoteAssetVolume;
                            findCoin.Ticks.Last().VolumeBase = data.Data.Volume;
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

                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);

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

                    if (findCoin == null)
                    {
                        BinanceMarket.Add(
                            new Market
                            {
                                Symbol = symbol,
                                Ticks = MarketTicks
                            });
                    }
                    else
                    {
                        findCoin.Ticks.AddRange(MarketTicks);
                    }
                }
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
