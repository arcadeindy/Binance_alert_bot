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
        public event BinanceClientStateHandler BinanceLog;
        public delegate void BinanceClientStateHandler(string message);
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
            BinanceLog?.Invoke("Подключение...");
            client = new BinanceClient();

            ws = new BinanceSocketClient();

            GetExchangeInfo();
            BinanceLog?.Invoke("Получил всю инфо о бриже");

            Task.Run(() => GetKlines());

            Task.Run(() => SubscribeToNewSymbolAsync(AllSymbolsBTC)).Wait();

            BinanceLog?.Invoke("Подключил сокеты всех монет");

            BinanceLog?.Invoke("Подключено");

            while (true)
            {
                foreach(var symbol in BinanceMarket.ToArray())
                    symbol.Ticks.RemoveAll(d => d.Time < DateTime.Now.AddMinutes(-1 * 60 * 24 * 2));
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
                            if (!coin.Symbol.Contains("BTC"))
                                continue;

                            var findCoin = BinanceMarket.Find(x => x.Symbol == coin.Symbol);

                            if (findCoin == null)
                            {
                                BinanceMarket.Add(
                                    new Market
                                    {
                                        Symbol = coin.Symbol,
                                        Bid = coin.BestBidPrice,
                                        Ask = coin.BestAskPrice,
                                        Ticks = new List<MarketTicks>()
                                    });
                            }
                            else
                            {
                                findCoin.Bid = coin.BestBidPrice;
                                findCoin.Ask = coin.BestAskPrice;
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
                        if (!data.Symbol.Contains("BTC"))
                            return;

                        var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);

                        if (findCoin == null)
                        {
                            BinanceMarket.Add(
                                new Market
                                {
                                    Symbol = data.Symbol,
                                    Bid = data.BestBidPrice,
                                    Ask = data.BestAskPrice,
                                    Ticks = new List<MarketTicks>()
                                });
                        }
                        else
                        {
                            findCoin.Bid = data.BestBidPrice;
                            findCoin.Ask = data.BestAskPrice;
                        }
                    }
                    catch { }
                });
                BinanceLog?.Invoke($"Подписался на новую монету {symbol[0]}");
            }

            //сокеты свечей на 1 минуту
            ws.SubscribeToKlineStream(symbol.ToArray(), KlineInterval.OneMinute, data =>
            {
                try
                {
                    if (!data.Symbol.Contains("BTC")) return;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == data.Symbol);
                    if (findCoin != null)
                    {
                        if (data.Data.Final)
                        {
                            findCoin.Ticks.Add(
                                new MarketTicks
                                {
                                    Time = data.Data.CloseTime,
                                    Close = data.Data.Close,
                                    Open = data.Data.Open,
                                    Volume = data.Data.QuoteAssetVolume
                                });
                        }
                        else
                        {
                            findCoin.Ticks.Last().Time = data.EventTime;
                            findCoin.Ticks.Last().Close = data.Data.Close;
                            findCoin.Ticks.Last().Open = data.Data.Open;
                            findCoin.Ticks.Last().Volume = data.Data.QuoteAssetVolume;
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
                int requests = 60 * 48;
                while (requests > 0)
                {
                    CallResult<BinanceKline[]> klines = client.GetKlines(symbol, KlineInterval.OneMinute, limit: (requests - 1000 > 0) ? 1000 : requests);

                    requests -= 1000;

                    var findCoin = BinanceMarket.Find(x => x.Symbol == symbol);

                    var MarketTicks = new List<MarketTicks>();

                    foreach (var kline in klines.Data)
                    {
                        MarketTicks.Add(
                            new MarketTicks
                            {
                                Time = kline.CloseTime,
                                Close = kline.Close,
                                Open = kline.Open,
                                High = kline.High,
                                Low = kline.Low,
                                Volume = kline.QuoteAssetVolume
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
            BinanceLog?.Invoke("Получил историю свечей");
        }
        private void GetExchangeInfo(string Symbol = "")
        {
            start:
            var info = client.GetExchangeInfo();
            if (info.Success)
            {
                exchangeInfo = info.Data.Symbols;

                if (Symbol != String.Empty && Symbol.Contains("BTC"))
                        Task.Run(() => SubscribeToNewSymbolAsync(new List<string>() { Symbol }));


                foreach (var symbol in exchangeInfo.ToArray())
                {
                    if (symbol.Name.Contains("BTC"))
                    {
                        AllSymbolsBTC.Add(symbol.Name);
                    }
                }
            }
            else
            {
                ErrorHandling(info.Error);
                goto start;
            }
        }
        private void ErrorHandling(Error error)
        {
            BinanceLog?.Invoke($"{error.Code}: {error.Message}");
        }
        #endregion
    }
}
