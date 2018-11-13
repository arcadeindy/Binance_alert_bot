using Binance_alert_bot.Binance;
using Binance_alert_bot.Binance.Objects;
using Binance_alert_bot.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Binance_alert_bot
{
    public partial class MainMenu : Form
    {
        BinanceBotClient client;
        Config cfg;
        public MainMenu()
        {
            InitializeComponent();
        }

        #region Form
        private void MainMenu_Shown(object sender, EventArgs e)
        {
            client = new BinanceBotClient();
            client.BinanceLogs += Logs;

            Task.Run(() => client.BinanceSetCredentials());
            Task.Run(() => LoadBinance());
        }
        private void btnAddNotify_Click(object sender, EventArgs e)
        {
            if (this.cblNotifySymbols.CheckedItems.Count == 0 
                && this.ddlNotifyType.SelectedItem.ToString() == "" 
                && this.ddlNotifyTimeframe.SelectedItem.ToString() == "" 
                && this.tbNotifyChange.Text != ""
                && this.ddlGiud.SelectedItem.ToString() != ""
                && this.ddlChatId.ToString() != "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            InThread(() => this.dgNotification.Rows.Add(
                (cblNotifySymbols.CheckedItems.Count > 1) ? $"{cblNotifySymbols.CheckedItems.Count} Pairs" : cblNotifySymbols.CheckedItems[0].ToString(),
                this.ddlNotifyType.SelectedItem.ToString(),
                this.ddlNotifyTimeframe.SelectedItem.ToString(),
                GetMultiChange(this.tbNotifyChange.Text),
                this.ddlGiud.SelectedItem.ToString(),
                this.ddlChatId.SelectedText.ToString()));
        }

        private void btnNotifyDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgNotification.SelectedRows)
            {
                this.dgNotification.Rows.RemoveAt(row.Index);
            }
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
            ColumnAction("PriceChange1min", cbPriceChange1min.Checked);
        }

        private void cbChange3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange3min", cbPriceChange3min.Checked);
        }

        private void cbChange5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange5min", cbPriceChange5min.Checked);
        }

        private void cbChange15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange15min", cbPriceChange15min.Checked);
        }

        private void cbChange30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange30min", cbPriceChange30min.Checked);
        }

        private void cbChange1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange1h", cbPriceChange1h.Checked);
        }

        private void cbChange2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange2h", cbPriceChange2h.Checked);
        }

        private void cbChange4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange4h", cbPriceChange4h.Checked);
        }

        private void cbChange6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange6h", cbPriceChange6h.Checked);
        }

        private void cbChange12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange12h", cbPriceChange12h.Checked);
        }

        private void cbChange24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("PriceChange24h", cbPriceChange24h.Checked);
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
            ColumnAction("VolumeQuote1min", cbVolumeQuote1min.Checked);
        }

        private void cbVolume3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote3min", cbVolumeQuote3min.Checked);
        }

        private void cbVolume5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote5min", cbVolumeQuote5min.Checked);
        }

        private void cbVolume15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote15min", cbVolumeQuote15min.Checked);
        }

        private void cbVolume30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote30min", cbVolumeQuote30min.Checked);
        }

        private void cbVolume1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote1h", cbVolumeQuote1h.Checked);
        }

        private void cbVolume2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote2h", cbVolumeQuote2h.Checked);
        }

        private void cbVolume4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote4h", cbVolumeQuote4h.Checked);
        }

        private void cbVolume6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote6h", cbVolumeQuote6h.Checked);
        }

        private void cbVolume12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote12h", cbVolumeQuote12h.Checked);
        }

        private void cbVolume24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeQuote24h", cbVolumeQuote24h.Checked);
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

        private void cbVolumeBase1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase1min", cbVolumeBase1min.Checked);
        }

        private void cbVolumeBase3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase3min", cbVolumeBase3min.Checked);
        }

        private void cbVolumeBase5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase5min", cbVolumeBase5min.Checked);
        }

        private void cbVolumeBase15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase15min", cbVolumeBase15min.Checked);
        }

        private void cbVolumeBase30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase30min", cbVolumeBase30min.Checked);
        }

        private void cbVolumeBase1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase1h", cbVolumeBase1h.Checked);
        }

        private void cbVolumeBase2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase2h", cbVolumeBase2h.Checked);
        }

        private void cbVolumeBase4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase4h", cbVolumeBase4h.Checked);
        }

        private void cbVolumeBase6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase6h", cbVolumeBase6h.Checked);
        }

        private void cbVolumeBase12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase12h", cbVolumeBase12h.Checked);
        }

        private void cbVolumeBase24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("VolumeBase24h", cbVolumeBase24h.Checked);
        }
        private void cbAmplitude1min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude1min", cbAmplitude1min.Checked);
        }

        private void cbAmplitude3min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude3min", cbAmplitude3min.Checked);
        }

        private void cbAmplitude5min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude5min", cbAmplitude5min.Checked);
        }

        private void cbAmplitude15min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude15min", cbAmplitude15min.Checked);
        }

        private void cbAmplitude30min_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude30min", cbAmplitude30min.Checked);
        }

        private void cbAmplitude1h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude1h", cbAmplitude1h.Checked);
        }

        private void cbAmplitude2h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude2h", cbAmplitude2h.Checked);
        }

        private void cbAmplitude4h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude4h", cbAmplitude4h.Checked);
        }

        private void cbAmplitude6h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude6h", cbAmplitude6h.Checked);
        }

        private void cbAmplitude12h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude12h", cbAmplitude12h.Checked);
        }

        private void cbAmplitude24h_CheckedChanged(object sender, EventArgs e)
        {
            ColumnAction("Amplitude24h", cbAmplitude24h.Checked);
        }

        private void dgBinanceTable_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 0)
            {
                e.SortResult = decimal.Parse(e.CellValue1.ToString().Replace(" %", "")).CompareTo(decimal.Parse(e.CellValue2.ToString().Replace(" %", "")));
                e.Handled = true;
            }
        }

        private void dgBinanceTable_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int index = e.RowIndex;
            string indexStr = (index + 1).ToString();
            object header = this.dgBinanceTable.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(indexStr))
                this.dgBinanceTable.Rows[index].HeaderCell.Value = indexStr;
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            LoadSettings();
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
            catch(Exception ex)
            {
            }
        }
        private void Logs(string msg)
        {
            InThread(() => this.tbLogs.AppendText($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} {msg}\n"));
        }
        private void Symbols(List<string> symbols)
        {
            InThread(() => this.ddlSymbols.Items.Clear());
            InThread(() => this.cblNotifySymbols.Items.Clear());

            symbols = symbols.OrderBy(q => q).ToList();

            foreach (var symbol in symbols)
            {
                InThread(() => this.ddlSymbols.Items.Add(symbol.Replace("BTC", "/BTC")));
                InThread(() => this.cblNotifySymbols.Items.Add(symbol.Replace("BTC", "/BTC")));
            }
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

                            if (this.cbFavorite.Checked && !this.lbFavorite.Items.Contains(market.Symbol.Replace("BTC", "/BTC")))
                                continue;

                            start:
                            bool find = false;
                            foreach (DataGridViewRow dr in this.dgBinanceTable.Rows)
                            {
                                if (dr.Cells["Symbol"].Value.ToString() == market.Symbol.Replace("BTC", "/BTC"))
                                {
                                    find = true;

                                    DataGridViewRow drBefore = (Convert.ToDecimal(dr.Cells["Ask"].Value) > 0) ? dr : null;

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

                                    if (this.rb1minTimeframe.Checked)
                                        InThread(() => {
                                        LoadBinanceCell(dr.Cells["Ask"], market.Ask);
                                        LoadBinanceCell(dr.Cells["Bid"], market.Bid);

                                        LoadBinanceCell(dr.Cells["PriceChange1min"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-1 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange3min"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-3 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange5min"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-5 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange15min"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-15 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange30min"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-30 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange1h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange2h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60*2 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange4h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60*4 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange6h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60*6 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange12h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60*12 + 1)).Close, market.Ticks1min.Last().Close), true);
                                        LoadBinanceCell(dr.Cells["PriceChange24h"], GetProfit(market.Ticks1min.Find(t => t.Time == market.Ticks1min[market.Ticks1min.Count - 2].Time.AddMinutes(-60*24 + 1)).Close, market.Ticks1min.Last().Close), true);

                                        LoadBinanceCell(dr.Cells["High1min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High3min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High5min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High15min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High30min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High1h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High2h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High4h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High6h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High12h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Max(h => h.High));
                                        LoadBinanceCell(dr.Cells["High24h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Max(h => h.High));

                                        LoadBinanceCell(dr.Cells["Low1min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low3min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low5min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low15min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low30min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low1h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low2h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low4h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low6h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low12h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12) && t.Low > 0).Min(h => h.Low));
                                        LoadBinanceCell(dr.Cells["Low24h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24) && t.Low > 0).Min(h => h.Low));

                                        LoadBinanceCell(dr.Cells["Amplitude1min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude3min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude5min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude15min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude30min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude1h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude2h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*2)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*2)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude4h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*4)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*4)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude6h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*6)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*6)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude12h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*12)).Min(m=>m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*12)).Max(m => m.High)));
                                        LoadBinanceCell(dr.Cells["Amplitude24h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*24)).Min(m => m.Low), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60*24)).Max(m => m.High)));

                                        LoadBinanceCell(dr.Cells["VolumeQuote1min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote3min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote5min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote15min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote30min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote1h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote2h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote4h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote6h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote12h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Sum(h => h.VolumeQuote));
                                        LoadBinanceCell(dr.Cells["VolumeQuote24h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Sum(h => h.VolumeQuote));

                                        LoadBinanceCell(dr.Cells["VolumeBase1min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase3min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase5min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase15min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase30min"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase1h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase2h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase4h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase6h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase12h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12)).Sum(h => h.VolumeBase));
                                        LoadBinanceCell(dr.Cells["VolumeBase24h"], market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24)).Sum(h => h.VolumeBase));     

                                        LoadBinanceCell(dr.Cells["VolumeChange1min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-1)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time >= DateTime.UtcNow.AddMinutes(0)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange3min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-3)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange5min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-5)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange15min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-15)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange30min"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-30)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange1h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(60 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange2h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(60 * 2 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60 * 2)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 2)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange4h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 4 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60 * 4)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * 60 * 4)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange6h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 6 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60 * 6)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * 60 * 6)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange12h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 12 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60 * 12)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * 60 * 12)).Sum(h => h.VolumeQuote)), true);
                                        LoadBinanceCell(dr.Cells["VolumeChange24h"], GetProfit(market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-60 * 24 * 2) && t.Time <= DateTime.UtcNow.AddMinutes(-60 * 24)).Sum(h => h.VolumeQuote), market.Ticks1min.FindAll(t => t.Time > DateTime.UtcNow.AddMinutes(-1 * 60 * 24)).Sum(h => h.VolumeQuote)), true);
                                        });
                                    else
                                        InThread(() => {
                                            LoadBinanceCell(dr.Cells["Ask"], market.Ask);
                                            LoadBinanceCell(dr.Cells["Bid"], market.Bid);

                                            LoadBinanceCell(dr.Cells["PriceChange1min"], GetProfit(market.Ticks1min[market.Ticks1min.Count - 2].Close, market.Ticks1min.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange3min"], GetProfit(market.Ticks3min[market.Ticks3min.Count - 2].Close, market.Ticks3min.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange5min"], GetProfit(market.Ticks5min[market.Ticks5min.Count - 2].Close, market.Ticks5min.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange15min"], GetProfit(market.Ticks15min[market.Ticks15min.Count - 2].Close, market.Ticks15min.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange30min"], GetProfit(market.Ticks30min[market.Ticks30min.Count - 2].Close, market.Ticks30min.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange1h"], GetProfit(market.Ticks1h[market.Ticks1h.Count - 2].Close, market.Ticks1h.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange2h"], GetProfit(market.Ticks2h[market.Ticks2h.Count - 2].Close, market.Ticks2h.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange4h"], GetProfit(market.Ticks4h[market.Ticks4h.Count - 2].Close, market.Ticks4h.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange6h"], GetProfit(market.Ticks6h[market.Ticks6h.Count - 2].Close, market.Ticks6h.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange12h"], GetProfit(market.Ticks12h[market.Ticks12h.Count - 2].Close, market.Ticks12h.Last().Close), true);
                                            LoadBinanceCell(dr.Cells["PriceChange24h"], GetProfit(market.Ticks24h[market.Ticks24h.Count - 2].Close, market.Ticks24h.Last().Close), true);

                                            LoadBinanceCell(dr.Cells["High1min"], market.Ticks1min.Last().High);
                                            LoadBinanceCell(dr.Cells["High3min"], market.Ticks3min.Last().High);
                                            LoadBinanceCell(dr.Cells["High5min"], market.Ticks5min.Last().High);
                                            LoadBinanceCell(dr.Cells["High15min"], market.Ticks15min.Last().High);
                                            LoadBinanceCell(dr.Cells["High30min"], market.Ticks30min.Last().High);
                                            LoadBinanceCell(dr.Cells["High1h"], market.Ticks1h.Last().High);
                                            LoadBinanceCell(dr.Cells["High2h"], market.Ticks2h.Last().High);
                                            LoadBinanceCell(dr.Cells["High4h"], market.Ticks4h.Last().High);
                                            LoadBinanceCell(dr.Cells["High6h"], market.Ticks6h.Last().High);
                                            LoadBinanceCell(dr.Cells["High12h"], market.Ticks12h.Last().High);
                                            LoadBinanceCell(dr.Cells["High24h"], market.Ticks24h.Last().High);

                                            LoadBinanceCell(dr.Cells["Low1min"], market.Ticks1min.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low3min"], market.Ticks3min.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low5min"], market.Ticks5min.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low15min"], market.Ticks15min.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low30min"], market.Ticks30min.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low1h"], market.Ticks1h.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low2h"], market.Ticks2h.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low4h"], market.Ticks4h.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low6h"], market.Ticks6h.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low12h"], market.Ticks12h.Last().Low);
                                            LoadBinanceCell(dr.Cells["Low24h"], market.Ticks24h.Last().Low);

                                            LoadBinanceCell(dr.Cells["Amplitude1min"], GetProfit(market.Ticks1min.Last().Low, market.Ticks1min.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude3min"], GetProfit(market.Ticks3min.Last().Low, market.Ticks3min.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude5min"], GetProfit(market.Ticks5min.Last().Low, market.Ticks5min.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude15min"], GetProfit(market.Ticks15min.Last().Low, market.Ticks15min.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude30min"], GetProfit(market.Ticks30min.Last().Low, market.Ticks30min.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude1h"], GetProfit(market.Ticks1h.Last().Low, market.Ticks1h.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude2h"], GetProfit(market.Ticks2h.Last().Low, market.Ticks2h.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude4h"], GetProfit(market.Ticks4h.Last().Low, market.Ticks4h.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude6h"], GetProfit(market.Ticks6h.Last().Low, market.Ticks6h.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude12h"], GetProfit(market.Ticks12h.Last().Low, market.Ticks12h.Last().High));
                                            LoadBinanceCell(dr.Cells["Amplitude24h"], GetProfit(market.Ticks24h.Last().Low, market.Ticks24h.Last().High));

                                            LoadBinanceCell(dr.Cells["VolumeQuote1min"], market.Ticks1min.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote3min"], market.Ticks3min.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote5min"], market.Ticks5min.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote15min"], market.Ticks15min.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote30min"], market.Ticks30min.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote1h"], market.Ticks1h.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote2h"], market.Ticks2h.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote4h"], market.Ticks4h.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote6h"], market.Ticks6h.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote12h"], market.Ticks12h.Last().VolumeQuote);
                                            LoadBinanceCell(dr.Cells["VolumeQuote24h"], market.Ticks24h.Last().VolumeQuote);

                                            LoadBinanceCell(dr.Cells["VolumeBase1min"], market.Ticks1min.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase3min"], market.Ticks3min.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase5min"], market.Ticks5min.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase15min"], market.Ticks15min.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase30min"], market.Ticks30min.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase1h"], market.Ticks1h.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase2h"], market.Ticks2h.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase4h"], market.Ticks4h.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase6h"], market.Ticks6h.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase12h"], market.Ticks12h.Last().VolumeBase);
                                            LoadBinanceCell(dr.Cells["VolumeBase24h"], market.Ticks24h.Last().VolumeBase);

                                            LoadBinanceCell(dr.Cells["VolumeChange1min"], GetProfit(market.Ticks1min[market.Ticks1min.Count - 2].VolumeQuote, market.Ticks1min.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange3min"], GetProfit(market.Ticks3min[market.Ticks3min.Count - 2].VolumeQuote, market.Ticks3min.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange5min"], GetProfit(market.Ticks5min[market.Ticks5min.Count - 2].VolumeQuote, market.Ticks5min.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange15min"], GetProfit(market.Ticks15min[market.Ticks15min.Count - 2].VolumeQuote, market.Ticks15min.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange30min"], GetProfit(market.Ticks30min[market.Ticks30min.Count - 2].VolumeQuote, market.Ticks30min.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange1h"], GetProfit(market.Ticks1h[market.Ticks1h.Count - 2].VolumeQuote, market.Ticks1h.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange2h"], GetProfit(market.Ticks2h[market.Ticks2h.Count - 2].VolumeQuote, market.Ticks2h.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange4h"], GetProfit(market.Ticks4h[market.Ticks4h.Count - 2].VolumeQuote, market.Ticks4h.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange6h"], GetProfit(market.Ticks6h[market.Ticks6h.Count - 2].VolumeQuote, market.Ticks6h.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange12h"], GetProfit(market.Ticks12h[market.Ticks12h.Count - 2].VolumeQuote, market.Ticks12h.Last().VolumeQuote), true);
                                            LoadBinanceCell(dr.Cells["VolumeChange24h"], GetProfit(market.Ticks24h[market.Ticks24h.Count - 2].VolumeQuote, market.Ticks24h.Last().VolumeQuote), true);
                                        });

                                    SendNotification(dr, drBefore);
                                    break;
                                }
                            }

                            if (!find)
                            {
                                InThread(() => this.dgBinanceTable.Rows.Add(
                                    market.Symbol.Replace("BTC", "/BTC"),
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
                catch (Exception ex)
                {
                    Logs(ex.ToString());
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void LoadBinanceCell(DataGridViewCell cell, decimal value, bool changeColor = false)
        {
            if (Convert.ToDecimal(cell.Value.ToString().Replace(" %", "")) == value && !changeColor)
                return;

            if (changeColor)
            {
                if (value < 0)
                    cell.Style.ForeColor = Color.Red;
                else if (value > 0)
                    cell.Style.ForeColor = Color.Green;
            }
            
            cell.Value = (!changeColor) ? $"{value}" : $"{value} %";
        }
        public decimal GetProfit(decimal first, decimal last)
        {
            if (first == 0)
                first = 1;
            if (last == 0)
                last = 1;
            return Math.Round(last * 100 / first - 100, 2);
        }

        public void SendNotification(DataGridViewRow dr_t, DataGridViewRow drBefore)
        {
            if (drBefore == null)
                return;

            if (this.tbTelegramApi.Text != "" && this.tbTelegramChatId.Text != "")
            {
                TelegramBotClient bot = new TelegramBotClient(this.tbTelegramApi.Text);
                foreach (DataGridViewRow dr in this.dgNotification.Rows)
                {
                    if (dr_t.Cells["Symbol"].Value.ToString() != dr.Cells["Symb"].Value.ToString())
                        continue;

                    decimal drChange = Convert.ToDecimal(dr.Cells["Change"].Value.ToString());
                    decimal dr_tValue = 0;
                    decimal drBeforeValue = 0;
                    if (dr.Cells["Type"].Value.ToString() == "Ask" || dr.Cells["Type"].Value.ToString() == "Bid")
                    {
                        dr_tValue = Convert.ToDecimal(dr_t.Cells[$"{dr.Cells["Type"].Value.ToString()}"].Value.ToString());
                        drBeforeValue = Convert.ToDecimal(drBefore.Cells[$"{dr.Cells["Type"].Value.ToString()}"].Value.ToString());
                    }
                    else
                    {
                        dr_tValue = Convert.ToDecimal(dr_t.Cells[$"{dr.Cells["Type"].Value.ToString()}{dr.Cells["Timeframe"].Value.ToString()}"].Value.ToString().Split(' ')[0].ToString());
                        drBeforeValue = Convert.ToDecimal(drBefore.Cells[$"{dr.Cells["Type"].Value.ToString()}{dr.Cells["Timeframe"].Value.ToString()}"].Value.ToString().Split(' ')[0].ToString());
                    }


                    bool less = dr.Cells["Change"].Value.ToString().Contains("<");
                    bool more = dr.Cells["Change"].Value.ToString().Contains(">");

                    

                    if (dr_tValue > drChange && more)
                    {
                        string Type = dr.Cells["Type"].Value.ToString();
                        string Symbol = $"[{dr.Cells["Symb"].Value.ToString()}](https://www.binance.com/en/trade/{dr.Cells["Symb"].Value.ToString().Replace("/", "_")})";
                        string drChangeValue = dr.Cells["Change"].Value.ToString();
                        string Timeframe = (dr.Cells["Type"].Value.ToString() == "Ask" || dr.Cells["Type"].Value.ToString() == "Bid") ? "" : dr.Cells["Timeframe"].Value.ToString();
                        string EndSymbol = dr.Cells["Change"].Value.ToString().Split(' ')[2].ToString();

                        string text = $"{Symbol}\n";
                        text = $"*{Type}* = `{dr_tValue}` `{EndSymbol}`\n";

                        if (dr.Cells["Type"].Value.ToString() == "PriceChange")
                            text = $"`{drBeforeValue}` → `{dr_tValue}`\n";

                        text = $"*{Type}* `{drChangeValue}` *{Timeframe}*\n";

                        Logs($"Telegram -> {dr.Cells["Symb"].Value.ToString()}");

                        bot.SendTextMessageAsync(Convert.ToInt32(this.tbTelegramChatId.Text), text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                    }
                    if (dr_tValue < drChange && less)
                    {
                        string Type = dr.Cells["Type"].Value.ToString();
                        string Symbol = $"[{dr.Cells["Symb"].Value.ToString()}](https://www.binance.com/en/trade/{dr.Cells["Symb"].Value.ToString().Replace("/", "_")})";
                        string drChangeValue = dr.Cells["Change"].Value.ToString();
                        string Timeframe = (dr.Cells["Type"].Value.ToString() == "Ask" || dr.Cells["Type"].Value.ToString() == "Bid") ? "" : dr.Cells["Timeframe"].Value.ToString();
                        string EndSymbol = dr.Cells["Change"].Value.ToString().Split(' ')[2].ToString();

                        string text = $"{Symbol}\n" +
                             $"*{Type}* = `{dr_tValue}` `{EndSymbol}`\n" +
                             $"*{Type}* `{drChangeValue}` *{Timeframe}*\n";

                        Logs($"Telegram -> {dr.Cells["Symb"].Value.ToString()}");
                        bot.SendTextMessageAsync(Convert.ToInt32(this.tbTelegramChatId.Text), text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                    }
                }
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists("config.json"))
                cfg = new Config();
            else
                cfg = Config.Reload();

            this.cbAsk.Checked = this.cfg.ask;
            this.cbBid.Checked = this.cfg.bid;

            this.cbPriceChange1min.Checked = this.cfg.PriceChange1min;
            this.cbPriceChange3min.Checked = this.cfg.PriceChange3min;
            this.cbPriceChange5min.Checked = this.cfg.PriceChange5min;
            this.cbPriceChange15min.Checked = this.cfg.PriceChange15min;
            this.cbPriceChange30min.Checked = this.cfg.PriceChange30min;
            this.cbPriceChange1h.Checked = this.cfg.PriceChange1h;
            this.cbPriceChange2h.Checked = this.cfg.PriceChange2h;
            this.cbPriceChange4h.Checked = this.cfg.PriceChange4h;
            this.cbPriceChange6h.Checked = this.cfg.PriceChange6h;
            this.cbPriceChange12h.Checked = this.cfg.PriceChange12h;
            this.cbPriceChange24h.Checked = this.cfg.PriceChange24h;

            this.cbHigh1min.Checked = this.cfg.High1min;
            this.cbHigh3min.Checked = this.cfg.High3min;
            this.cbHigh5min.Checked = this.cfg.High5min;
            this.cbHigh15min.Checked = this.cfg.High15min;
            this.cbHigh30min.Checked = this.cfg.High30min;
            this.cbHigh1h.Checked = this.cfg.High1h;
            this.cbHigh2h.Checked = this.cfg.High2h;
            this.cbHigh4h.Checked = this.cfg.High4h;
            this.cbHigh6h.Checked = this.cfg.High6h;
            this.cbHigh12h.Checked = this.cfg.High12h;
            this.cbHigh24h.Checked = this.cfg.High24h;

            this.cbLow1min.Checked = this.cfg.Low1min;
            this.cbLow3min.Checked = this.cfg.Low3min;
            this.cbLow5min.Checked = this.cfg.Low5min;
            this.cbLow15min.Checked = this.cfg.Low15min;
            this.cbLow30min.Checked = this.cfg.Low30min;
            this.cbLow1h.Checked = this.cfg.Low1h;
            this.cbLow2h.Checked = this.cfg.Low2h;
            this.cbLow4h.Checked = this.cfg.Low4h;
            this.cbLow6h.Checked = this.cfg.Low6h;
            this.cbLow12h.Checked = this.cfg.Low12h;
            this.cbLow24h.Checked = this.cfg.Low24h;

            this.cbAmplitude1min.Checked = this.cfg.Amplitude1min;
            this.cbAmplitude3min.Checked = this.cfg.Amplitude3min;
            this.cbAmplitude5min.Checked = this.cfg.Amplitude5min;
            this.cbAmplitude15min.Checked = this.cfg.Amplitude15min;
            this.cbAmplitude30min.Checked = this.cfg.Amplitude30min;
            this.cbAmplitude1h.Checked = this.cfg.Amplitude1h;
            this.cbAmplitude2h.Checked = this.cfg.Amplitude2h;
            this.cbAmplitude4h.Checked = this.cfg.Amplitude4h;
            this.cbAmplitude6h.Checked = this.cfg.Amplitude6h;
            this.cbAmplitude12h.Checked = this.cfg.Amplitude12h;
            this.cbAmplitude24h.Checked = this.cfg.Amplitude24h;

            this.cbVolumeQuote1min.Checked = this.cfg.VolumeQuote1min;
            this.cbVolumeQuote3min.Checked = this.cfg.VolumeQuote3min;
            this.cbVolumeQuote5min.Checked = this.cfg.VolumeQuote5min;
            this.cbVolumeQuote15min.Checked = this.cfg.VolumeQuote15min;
            this.cbVolumeQuote30min.Checked = this.cfg.VolumeQuote30min;
            this.cbVolumeQuote1h.Checked = this.cfg.VolumeQuote1h;
            this.cbVolumeQuote2h.Checked = this.cfg.VolumeQuote2h;
            this.cbVolumeQuote4h.Checked = this.cfg.VolumeQuote4h;
            this.cbVolumeQuote6h.Checked = this.cfg.VolumeQuote6h;
            this.cbVolumeQuote12h.Checked = this.cfg.VolumeQuote12h;
            this.cbVolumeQuote24h.Checked = this.cfg.VolumeQuote24h;

            this.cbVolumeBase1min.Checked = this.cfg.VolumeBase1min;
            this.cbVolumeBase3min.Checked = this.cfg.VolumeBase3min;
            this.cbVolumeBase5min.Checked = this.cfg.VolumeBase5min;
            this.cbVolumeBase15min.Checked = this.cfg.VolumeBase15min;
            this.cbVolumeBase30min.Checked = this.cfg.VolumeBase30min;
            this.cbVolumeBase1h.Checked = this.cfg.VolumeBase1h;
            this.cbVolumeBase2h.Checked = this.cfg.VolumeBase2h;
            this.cbVolumeBase4h.Checked = this.cfg.VolumeBase4h;
            this.cbVolumeBase6h.Checked = this.cfg.VolumeBase6h;
            this.cbVolumeBase12h.Checked = this.cfg.VolumeBase12h;
            this.cbVolumeBase24h.Checked = this.cfg.VolumeBase24h;

            this.cbVolumeChange1min.Checked = this.cfg.VolumeChange1min;
            this.cbVolumeChange3min.Checked = this.cfg.VolumeChange3min;
            this.cbVolumeChange5min.Checked = this.cfg.VolumeChange5min;
            this.cbVolumeChange15min.Checked = this.cfg.VolumeChange15min;
            this.cbVolumeChange30min.Checked = this.cfg.VolumeChange30min;
            this.cbVolumeChange1h.Checked = this.cfg.VolumeChange1h;
            this.cbVolumeChange2h.Checked = this.cfg.VolumeChange2h;
            this.cbVolumeChange4h.Checked = this.cfg.VolumeChange4h;
            this.cbVolumeChange6h.Checked = this.cfg.VolumeChange6h;
            this.cbVolumeChange12h.Checked = this.cfg.VolumeChange12h;
            this.cbVolumeChange24h.Checked = this.cfg.VolumeChange24h;

            this.cbFavorite.Checked = this.cfg.Favorite;

            this.lbFavorite.Items.Clear();
            foreach(var symbol in cfg.FavoriveSymbols)
            {
                this.lbFavorite.Items.Add(symbol);
            }

            this.dgNotification.Rows.Clear();
            foreach(var notify in cfg.notifications)
            {
                foreach (var ntf in notify.Guid)
                {
                    foreach (var n in ntf.Value)
                    {
                        this.dgNotification.Rows.Add(
                                                (n.Symbol.Count > 1) ? $"{n.Symbol.Count} Pairs" : $"{n.Symbol[0].ToString()}" ,
                                                n.Type,
                                                n.Timeframe,
                                                n.Change,
                                                ntf.Key,
                                                n.TelegramChatId);
                    }
                }
            }

            this.tbTelegramApi.Text = this.cfg.TelegramApiKey;
            this.lbChatId.Items.Clear();
            foreach(var chatid in cfg.cha)

            this.tbTelegramChatId.Text = this.cfg.TelegramChatID;

            this.rb1minTimeframe.Checked = this.cfg.Timeframe1min;
            this.rbAllTimeframe.Checked = this.cfg.TimeframeAll;
        }
        private void SaveSettings()
        {
            this.cfg.ask = this.cbAsk.Checked;
            this.cfg.bid = this.cbBid.Checked;

            this.cfg.PriceChange1min = this.cbPriceChange1min.Checked;
            this.cfg.PriceChange3min = this.cbPriceChange3min.Checked;
            this.cfg.PriceChange5min = this.cbPriceChange5min.Checked;
            this.cfg.PriceChange15min = this.cbPriceChange15min.Checked;
            this.cfg.PriceChange30min = this.cbPriceChange30min.Checked;
            this.cfg.PriceChange1h = this.cbPriceChange1h.Checked;
            this.cfg.PriceChange2h = this.cbPriceChange2h.Checked;
            this.cfg.PriceChange4h = this.cbPriceChange4h.Checked;
            this.cfg.PriceChange6h = this.cbPriceChange6h.Checked;
            this.cfg.PriceChange12h = this.cbPriceChange12h.Checked;
            this.cfg.PriceChange24h = this.cbPriceChange24h.Checked;

            this.cfg.High1min = this.cbHigh1min.Checked;
            this.cfg.High3min = this.cbHigh3min.Checked;
            this.cfg.High5min = this.cbHigh5min.Checked;
            this.cfg.High15min = this.cbHigh15min.Checked;
            this.cfg.High30min = this.cbHigh30min.Checked;
            this.cfg.High1h = this.cbHigh1h.Checked;
            this.cfg.High2h = this.cbHigh2h.Checked;
            this.cfg.High4h = this.cbHigh4h.Checked;
            this.cfg.High6h = this.cbHigh6h.Checked;
            this.cfg.High12h = this.cbHigh12h.Checked;
            this.cfg.High24h = this.cbHigh24h.Checked;

            this.cfg.Low1min = this.cbLow1min.Checked;
            this.cfg.Low3min = this.cbLow3min.Checked;
            this.cfg.Low5min = this.cbLow5min.Checked;
            this.cfg.Low15min = this.cbLow15min.Checked;
            this.cfg.Low30min = this.cbLow30min.Checked;
            this.cfg.Low1h = this.cbLow1h.Checked;
            this.cfg.Low2h = this.cbLow2h.Checked;
            this.cfg.Low4h = this.cbLow4h.Checked;
            this.cfg.Low6h = this.cbLow6h.Checked;
            this.cfg.Low12h = this.cbLow12h.Checked;
            this.cfg.Low24h = this.cbLow24h.Checked;

            this.cfg.Amplitude1min = this.cbAmplitude1min.Checked;
            this.cfg.Amplitude3min = this.cbAmplitude3min.Checked;
            this.cfg.Amplitude5min = this.cbAmplitude5min.Checked;
            this.cfg.Amplitude15min = this.cbAmplitude15min.Checked;
            this.cfg.Amplitude30min = this.cbAmplitude30min.Checked;
            this.cfg.Amplitude1h = this.cbAmplitude1h.Checked;
            this.cfg.Amplitude2h = this.cbAmplitude2h.Checked;
            this.cfg.Amplitude4h = this.cbAmplitude4h.Checked;
            this.cfg.Amplitude6h = this.cbAmplitude6h.Checked;
            this.cfg.Amplitude12h = this.cbAmplitude12h.Checked;
            this.cfg.Amplitude24h = this.cbAmplitude24h.Checked;

            this.cfg.VolumeQuote1min = this.cbVolumeQuote1min.Checked;
            this.cfg.VolumeQuote3min = this.cbVolumeQuote3min.Checked;
            this.cfg.VolumeQuote5min = this.cbVolumeQuote5min.Checked;
            this.cfg.VolumeQuote15min = this.cbVolumeQuote15min.Checked;
            this.cfg.VolumeQuote30min = this.cbVolumeQuote30min.Checked;
            this.cfg.VolumeQuote1h = this.cbVolumeQuote1h.Checked;
            this.cfg.VolumeQuote2h = this.cbVolumeQuote2h.Checked;
            this.cfg.VolumeQuote4h = this.cbVolumeQuote4h.Checked;
            this.cfg.VolumeQuote6h = this.cbVolumeQuote6h.Checked;
            this.cfg.VolumeQuote12h = this.cbVolumeQuote12h.Checked;
            this.cfg.VolumeQuote24h = this.cbVolumeQuote24h.Checked;

            this.cfg.VolumeBase1min = this.cbVolumeBase1min.Checked;
            this.cfg.VolumeBase3min = this.cbVolumeBase3min.Checked;
            this.cfg.VolumeBase5min = this.cbVolumeBase5min.Checked;
            this.cfg.VolumeBase15min = this.cbVolumeBase15min.Checked;
            this.cfg.VolumeBase30min = this.cbVolumeBase30min.Checked;
            this.cfg.VolumeBase1h = this.cbVolumeBase1h.Checked;
            this.cfg.VolumeBase2h = this.cbVolumeBase2h.Checked;
            this.cfg.VolumeBase4h = this.cbVolumeBase4h.Checked;
            this.cfg.VolumeBase6h = this.cbVolumeBase6h.Checked;
            this.cfg.VolumeBase12h = this.cbVolumeBase12h.Checked;
            this.cfg.VolumeBase24h = this.cbVolumeBase24h.Checked;

            this.cfg.VolumeChange1min = this.cbVolumeChange1min.Checked;
            this.cfg.VolumeChange3min = this.cbVolumeChange3min.Checked;
            this.cfg.VolumeChange5min = this.cbVolumeChange5min.Checked;
            this.cfg.VolumeChange15min = this.cbVolumeChange15min.Checked;
            this.cfg.VolumeChange30min = this.cbVolumeChange30min.Checked;
            this.cfg.VolumeChange1h = this.cbVolumeChange1h.Checked;
            this.cfg.VolumeChange2h = this.cbVolumeChange2h.Checked;
            this.cfg.VolumeChange4h = this.cbVolumeChange4h.Checked;
            this.cfg.VolumeChange6h = this.cbVolumeChange6h.Checked;
            this.cfg.VolumeChange12h = this.cbVolumeChange12h.Checked;
            this.cfg.VolumeChange24h = this.cbVolumeChange24h.Checked;

            this.cfg.Favorite = this.cbFavorite.Checked;

            this.cfg.FavoriveSymbols = new List<string>();
            foreach (var f in this.lbFavorite.Items)
                cfg.FavoriveSymbols.Add(f.ToString());

            this.cfg.notifications = new List<Notifications>();
            foreach (DataGridViewRow dr in dgNotification.Rows)
            {
                this.cfg.notifications.Add(
                    new Notifications
                    {
                        Symbol = dr.Cells["Symb"].Value.ToString(),
                        Type = dr.Cells["Type"].Value.ToString(),
                        Timeframe = dr.Cells["Timeframe"].Value.ToString(),
                        Change = dr.Cells["Change"].Value.ToString()
                    });
            }

            this.cfg.TelegramApiKey = this.tbTelegramApi.Text;
            this.cfg.TelegramChatID = this.tbTelegramChatId.Text;

            this.cfg.Timeframe1min = this.rb1minTimeframe.Checked;
            this.cfg.TimeframeAll = this.rbAllTimeframe.Checked;

            Config.Save(cfg);
        }
        private string GetMultiChange(string change)
        {
            switch(this.ddlNotifyType.SelectedItem.ToString())
            {
                case "Amplitude":
                case "PriceChange":
                case "VolumeChange":
                    return $"{((this.rbLess.Checked) ? "<" : ">")} {change} %";
                case "VolumeQuote":
                    return $"{((this.rbLess.Checked) ? "<" : ">")} {change} BTC";
                case "VolumeBase":
                    string endSymbol = "";
                    if (this.cblNotifySymbols.CheckedItems.Count == 1)
                    {
                        endSymbol = this.cblNotifySymbols.CheckedItems[0].ToString().Replace("/BTC", "") ;
                    }
                    return $"{((this.rbLess.Checked) ? "<" : ">")} {change} {endSymbol}";
                case "Ask":
                case "Bid":
                case "High":
                case "Low":
                    return $"{((this.rbLess.Checked) ? "<" : ">")} {change} ";
                default:
                    return $"{((this.rbLess.Checked) ? "<" : ">")} {change} ";
            }
        }

        #endregion

        private void btnAddFavorite_Click(object sender, EventArgs e)
        {
            if (this.ddlSymbols.SelectedItem.ToString() != "")
                this.lbFavorite.Items.Add(this.ddlSymbols.SelectedItem.ToString());
        }

        private void btnDeleteFavorite_Click(object sender, EventArgs e)
        {
            if (this.lbFavorite.SelectedItem.ToString() != "")
                this.lbFavorite.Items.Remove(this.lbFavorite.SelectedItem);
        }

        private void cbFavorite_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
        }

        private void btnTelegramTestMsg_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbTelegramApi.Text != "" && this.tbTelegramChatId.Text != "")
                {
                    TelegramBotClient bot = new TelegramBotClient(this.tbTelegramApi.Text);
                    bot.SendTextMessageAsync(Convert.ToInt32(this.tbTelegramChatId.Text), "Тестовое сообщение", parseMode: ParseMode.Markdown);
                }
                else
                {
                    MessageBox.Show("Следует заполнить поля");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка в заполненных полях телеграма");
            }
        }

        private void rbAllTimeframe_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
        }

        private void rb1minTimeframe_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
        }

        private void tbNotifyChange_Leave(object sender, EventArgs e)
        {
            if (!Decimal.TryParse(this.tbNotifyChange.Text, out decimal price))
            {
                MessageBox.Show($"Ошибка в коверитировании. Двоичное число должно быть через '{Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)}'");
                return;
            }
        }
    }
}
