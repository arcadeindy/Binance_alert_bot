using Binance_alert_bot.Binance;
using Binance_alert_bot.Binance.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binance_alert_bot
{
    public partial class MainMenu : Form
    {
        BinanceBotClient client;
        public MainMenu()
        {
            InitializeComponent();
        }

        #region Form
        private void MainMenu_Shown(object sender, EventArgs e)
        {
            client = new BinanceBotClient();
            client.BinanceLog += Logs;
            Task.Run(() => client.BinanceSetCredentials());
            Task.Run(() => LoadBinance());
        }
        private void cbAsk_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Ask", cbAsk.Checked);
        }

        private void cbBid_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Bid", cbBid.Checked);
        }

        private void cbChange1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change1min", cbChange1min.Checked);
        }

        private void cbChange3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change3min", cbChange3min.Checked);
        }

        private void cbChange5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change5min", cbChange5min.Checked);
        }

        private void cbChange15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change15min", cbChange15min.Checked);
        }

        private void cbChange30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change30min", cbChange30min.Checked);
        }

        private void cbChange1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change1h", cbChange1h.Checked);
        }

        private void cbChange2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change2h", cbChange2h.Checked);
        }

        private void cbChange4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change4h", cbChange4h.Checked);
        }

        private void cbChange6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change6h", cbChange6h.Checked);
        }

        private void cbChange12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change12h", cbChange12h.Checked);
        }

        private void cbChange24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Change24h", cbChange24h.Checked);
        }

        private void cbHigh1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High1min", cbHigh1min.Checked);
        }

        private void cbHigh3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High3min", cbHigh3min.Checked);
        }

        private void cbHigh5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High5min", cbHigh5min.Checked);
        }

        private void cbHigh15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High15min", cbHigh15min.Checked);
        }

        private void cbHigh30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High30min", cbHigh30min.Checked);
        }

        private void cbHigh1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High1h", cbHigh1h.Checked);
        }

        private void cbHigh2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High2h", cbHigh2h.Checked);
        }

        private void cbHigh4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High4h", cbHigh4h.Checked);
        }

        private void cbHigh6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High6h", cbHigh6h.Checked);
        }

        private void cbHigh12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High12h", cbHigh12h.Checked);
        }

        private void cbHigh24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("High24h", cbHigh24h.Checked);
        }

        private void cbLow1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low1min", cbLow1min.Checked);
        }

        private void cbLow3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low3min", cbLow3min.Checked);
        }

        private void cbLow5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low5min", cbLow5min.Checked);
        }

        private void cbLow15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low15min", cbLow15min.Checked);
        }

        private void cbLow30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low30min", cbLow30min.Checked);
        }

        private void cbLow1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low1h", cbLow1h.Checked);
        }

        private void cbLow2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low2h", cbLow2h.Checked);
        }

        private void cbLow4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low4h", cbLow4h.Checked);
        }

        private void cbLow6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low6h", cbLow6h.Checked);
        }

        private void cbLow12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low12h", cbLow12h.Checked);
        }

        private void cbLow24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Low24h", cbLow24h.Checked);
        }

        private void cbVolume1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume1min", cbVolume1min.Checked);
        }

        private void cbVolume3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume3min", cbVolume3min.Checked);
        }

        private void cbVolume5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume5min", cbVolume5min.Checked);
        }

        private void cbVolume15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume15min", cbVolume15min.Checked);
        }

        private void cbVolume30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume30min", cbVolume30min.Checked);
        }

        private void cbVolume1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume1h", cbVolume1h.Checked);
        }

        private void cbVolume2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume2h", cbVolume2h.Checked);
        }

        private void cbVolume4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume4h", cbVolume4h.Checked);
        }

        private void cbVolume6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume6h", cbVolume6h.Checked);
        }

        private void cbVolume12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume12h", cbVolume12h.Checked);
        }

        private void cbVolume24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Volume24h", cbVolume24h.Checked);
        }

        private void cbVolumeChange1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange1min", cbVolumeChange1min.Checked);
        }

        private void cbVolumeChange3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange3min", cbVolumeChange3min.Checked);
        }

        private void cbVolumeChange5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange5min", cbVolumeChange5min.Checked);
        }

        private void cbVolumeChange15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange15min", cbVolumeChange15min.Checked);
        }

        private void cbVolumeChange30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange30min", cbVolumeChange30min.Checked);
        }

        private void cbVolumeChange1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange1h", cbVolumeChange1h.Checked);
        }

        private void cbVolumeChange2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange2h", cbVolumeChange2h.Checked);
        }

        private void cbVolumeChange4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange4h", cbVolumeChange4h.Checked);
        }

        private void cbVolumeChange6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange6h", cbVolumeChange6h.Checked);
        }

        private void cbVolumeChange12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange12h", cbVolumeChange12h.Checked);
        }

        private void cbVolumeChange24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeChange24h", cbVolumeChange24h.Checked);
        }
        #endregion

        #region Methods
        private void InThread(MethodInvoker mth)
        {
            //start:
            try
            {
                if (IsDisposed)
                    return;
                if (InvokeRequired)
                    Invoke(mth);
                else
                    mth();
            }
            catch
            {
                //goto start;
            }
        }
        private void Logs(string msg)
        {
            InThread(() => this.tbLogs.AppendText($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} {msg}\n"));
        }
        private void ColumnAction(string ColumnName, bool Checked)
        {
            InThread(() => this.dgBinanceTable.Columns[ColumnName].Visible = Checked);
        }
        private void LoadBinance()
        {
            while (true)
            {
                try
                {

                    if (client == null) { Thread.Sleep(1000); continue; }
                    var sw = Stopwatch.StartNew();
                    //List<Market> BinanceMarket = client.BinanceMarket;
                    if (client.BinanceMarket.Count > 0)

                        foreach (var market in client.BinanceMarket.ToArray())
                        {
                            if (market.Ticks.Count < 60 * 24 * 2 - 1)
                                continue;

                            start:
                            bool find = false;
                            foreach (DataGridViewRow dr in this.dgBinanceTable.Rows)
                            {
                                if (dr.Cells["Symbol"].Value.ToString() == market.Symbol)
                                {
                                    find = true;
                                    InThread(() => LoadBinanceCell(dr.Cells["Ask"], market.Ask));
                                    InThread(() => LoadBinanceCell(dr.Cells["Bid"], market.Bid));

                                    InThread(() => LoadBinanceCell(dr.Cells["Change1min"], GetProfit(market.Ticks.Last().Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change3min"], GetProfit(market.Ticks[market.Ticks.Count - 3].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change5min"], GetProfit(market.Ticks[market.Ticks.Count - 5].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change15min"], GetProfit(market.Ticks[market.Ticks.Count - 15].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change30min"], GetProfit(market.Ticks[market.Ticks.Count - 30].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change1h"], GetProfit(market.Ticks[market.Ticks.Count - 60].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change2h"], GetProfit(market.Ticks[market.Ticks.Count - 60 * 2].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change4h"], GetProfit(market.Ticks[market.Ticks.Count - 60 * 4].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change6h"], GetProfit(market.Ticks[market.Ticks.Count - 60 * 6].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change12h"], GetProfit(market.Ticks[market.Ticks.Count - 60 * 12].Open, market.Ticks.Last().Close)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Change24h"], GetProfit(market.Ticks[market.Ticks.Count - 60 * 24].Open, market.Ticks.Last().Close)));

                                    InThread(() => LoadBinanceCell(dr.Cells["High1min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High3min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High5min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High15min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High30min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High1h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High2h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High4h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High6h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High12h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Max(h => h.High)));
                                    InThread(() => LoadBinanceCell(dr.Cells["High24h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Max(h => h.High)));

                                    InThread(() => LoadBinanceCell(dr.Cells["Low1min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low3min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low5min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low15min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low30min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low1h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low2h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low4h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low6h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low12h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Max(h => h.Low)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Low24h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Max(h => h.Low)));

                                    InThread(() => LoadBinanceCell(dr.Cells["Volume1min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume3min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume5min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume15min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume30min"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume1h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume2h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume4h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume6h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume12h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Sum(h => h.Volume)));
                                    InThread(() => LoadBinanceCell(dr.Cells["Volume24h"], market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Sum(h => h.Volume)));

                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange1min"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 1) && t.Time < DateTime.UtcNow.AddMinutes(- 1)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange3min"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 3) && t.Time < DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange5min"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 5) && t.Time < DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange15min"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 15) && t.Time < DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange30min"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 30) && t.Time < DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange1h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60) && t.Time < DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange2h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60*2) && t.Time < DateTime.UtcNow.AddMinutes(-60*2)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange4h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60*4) && t.Time < DateTime.UtcNow.AddMinutes(-60*4)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange6h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60*6) && t.Time < DateTime.UtcNow.AddMinutes(-60*6)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange12h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60*12) && t.Time < DateTime.UtcNow.AddMinutes(-60*12)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    InThread(() => LoadBinanceCell(dr.Cells["VolumeChange24h"], GetProfit(market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24) - 60*24) && t.Time < DateTime.UtcNow.AddMinutes(-60*24)).Sum(h => h.Volume), market.Ticks.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes((-1 * 60 * 24))).Sum(h => h.Volume))));
                                    break;
                                }
                            }

                            if (!find)
                            {
                                InThread(() => this.dgBinanceTable.Rows.Add(
                                    market.Symbol,
                                    0,
                                    0,

                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,

                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,

                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,

                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,

                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0
                                    ));

                                goto start;
                            }   
                        }
                    sw.Stop();
                    if (sw.ElapsedMilliseconds / 10 > 1000)
                        continue;
                    else
                        Thread.Sleep(1000 - (int)sw.ElapsedMilliseconds / 10);

                }
                catch { }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void LoadBinanceCell(DataGridViewCell cell, decimal value)
        {
            if (Convert.ToDecimal(cell.Value) > value)
                cell.Style.ForeColor = Color.Red;
            else if (Convert.ToDecimal(cell.Value) < value)
                cell.Style.ForeColor = Color.Green;
            else
                cell.Style.ForeColor = Color.Black;

            cell.Value = value;
        }
        public decimal GetProfit(decimal first, decimal last)
        {
            return Math.Round(last * 100 / first - 100, 2);
        }


        #endregion


    }
}
