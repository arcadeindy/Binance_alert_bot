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
            client.BinanceSymbols += Symbols;
            client.BinanceMarkets += Markets;

            client.UpdateNotifications(cfg);

            Task.Run(() => client.BinanceSetCredentials());
        }
        private void btnAddNotify_Click(object sender, EventArgs e)
        {
            if (this.cblNotifySymbols.CheckedItems.Count == 0 
                && this.ddlNotifyType.SelectedItem.ToString() == "" 
                && this.ddlNotifyTimeframe.SelectedItem.ToString() == "" 
                && this.tbNotifyChange.Text != ""
                && this.ddlGiud.Text != ""
                && this.ddlChatId.SelectedItem.ToString() != "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            string guid = Guid.NewGuid().ToString();

            this.dgNotification.Rows.Add(
                (cblNotifySymbols.CheckedItems.Count > 1) ? $"{cblNotifySymbols.CheckedItems.Count} Pairs" : cblNotifySymbols.CheckedItems[0].ToString(),
                this.ddlNotifyType.SelectedItem.ToString(),
                this.ddlNotifyTimeframe.SelectedItem.ToString(),
                GetMultiChange(this.tbNotifyChange.Text),
                this.ddlGiud.Text,
                this.ddlChatId.Text,
                guid);

            List<string> list = new List<string>();
            for (int i = 0; i < cblNotifySymbols.CheckedItems.Count; i++)
            {
                list.Add(cblNotifySymbols.CheckedItems[i].ToString());
            }

            start:
            if (this.cfg.notifications.Guid.ContainsKey(this.ddlGiud.Text))
            {
                this.cfg.notifications.Guid[this.ddlGiud.Text].Add(
                    new Notification()
                    {
                        Change = GetMultiChange(this.tbNotifyChange.Text),
                        Symbol = list,
                        TelegramChatId = Convert.ToInt64(this.ddlChatId.SelectedValue.ToString()),
                        TelegramName = this.ddlChatId.Text,
                        Timeframe = this.ddlNotifyTimeframe.SelectedItem.ToString(),
                        Type = this.ddlNotifyType.SelectedItem.ToString(),
                        Guid = guid
                    });
            }
            else
            {
                this.ddlGiud.Items.Insert(0, this.ddlGiud.Text);
                this.cfg.notifications.Guid.Add(this.ddlGiud.Text, new List<Notification>());
                goto start;
            }

            SaveConfig();
        }

        private void btnNotifyDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgNotification.SelectedRows)
            {
                foreach (var g in this.cfg.notifications.Guid)
                {
                    g.Value.RemoveAll(x=> x.Guid == row.Cells["Secret"].Value.ToString());
                }
                this.dgNotification.Rows.RemoveAt(row.Index);
            }
            SaveConfig();
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
                MessageBox.Show(ex.ToString());
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

        private void Markets(List<Market> markets)
        {
            foreach (var market in markets)
            {
                if (!market.used)
                    continue;

                var find = false;
                foreach (DataGridViewRow row in this.dgBinanceTable.Rows)
                {
                    if (row.Cells["Symbol"].Value.ToString() == $"{market.BaseSymbol}/{market.QuoteSymbol}")
                    {
                        UpdateRow(row, market);
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    AddRow(this.dgBinanceTable, market);
                    UpdateRow(this.dgBinanceTable.Rows[this.dgBinanceTable.Rows.Count - 1], market);
                }
            }
        }

        private void UpdateRow(DataGridViewRow row, Market market)
        {
            InThread(() =>
            {
                row.Cells["Symbol"].Value = $"{market.BaseSymbol}/{market.QuoteSymbol}";
                row.Cells["Ask"].Value = market.Ask;
                row.Cells["Bid"].Value = market.Bid;
                row.Cells["PriceChange1min"].Value = market.MI["PriceChange1min"].ChangeValueProcentage;
                row.Cells["PriceChange1min"].Style.BackColor = market.MI["PriceChange1min"].BackColorColumn;
                row.Cells["PriceChange3min"].Value = market.MI["PriceChange3min"].ChangeValueProcentage;
                row.Cells["PriceChange3min"].Style.BackColor = market.MI["PriceChange3min"].BackColorColumn;
                row.Cells["PriceChange5min"].Value = market.MI["PriceChange5min"].ChangeValueProcentage;
                row.Cells["PriceChange5min"].Style.BackColor = market.MI["PriceChange5min"].BackColorColumn;
                row.Cells["PriceChange15min"].Value = market.MI["PriceChange15min"].ChangeValueProcentage;
                row.Cells["PriceChange15min"].Style.BackColor = market.MI["PriceChange15min"].BackColorColumn;
                row.Cells["PriceChange30min"].Value = market.MI["PriceChange30min"].ChangeValueProcentage;
                row.Cells["PriceChange30min"].Style.BackColor = market.MI["PriceChange30min"].BackColorColumn;
                row.Cells["PriceChange1h"].Value = market.MI["PriceChange1h"].ChangeValueProcentage;
                row.Cells["PriceChange1h"].Style.BackColor = market.MI["PriceChange1h"].BackColorColumn;
                row.Cells["PriceChange2h"].Value = market.MI["PriceChange2h"].ChangeValueProcentage;
                row.Cells["PriceChange2h"].Style.BackColor = market.MI["PriceChange2h"].BackColorColumn;
                row.Cells["PriceChange4h"].Value = market.MI["PriceChange4h"].ChangeValueProcentage;
                row.Cells["PriceChange4h"].Style.BackColor = market.MI["PriceChange4h"].BackColorColumn;
                row.Cells["PriceChange6h"].Value = market.MI["PriceChange6h"].ChangeValueProcentage;
                row.Cells["PriceChange6h"].Style.BackColor = market.MI["PriceChange6h"].BackColorColumn;
                row.Cells["PriceChange12h"].Value = market.MI["PriceChange12h"].ChangeValueProcentage;
                row.Cells["PriceChange12h"].Style.BackColor = market.MI["PriceChange12h"].BackColorColumn;
                row.Cells["PriceChange24h"].Value = market.MI["PriceChange24h"].ChangeValueProcentage;
                row.Cells["PriceChange24h"].Style.BackColor = market.MI["PriceChange24h"].BackColorColumn;


                row.Cells["High1min"].Value = market.MI["High1min"].NewValue;
                row.Cells["High3min"].Value = market.MI["High3min"].NewValue;
                row.Cells["High5min"].Value = market.MI["High5min"].NewValue;
                row.Cells["High15min"].Value = market.MI["High15min"].NewValue;
                row.Cells["High30min"].Value = market.MI["High30min"].NewValue;
                row.Cells["High1h"].Value = market.MI["High1h"].NewValue;
                row.Cells["High2h"].Value = market.MI["High2h"].NewValue;
                row.Cells["High4h"].Value = market.MI["High4h"].NewValue;
                row.Cells["High6h"].Value = market.MI["High6h"].NewValue;
                row.Cells["High12h"].Value = market.MI["High12h"].NewValue;
                row.Cells["High24h"].Value = market.MI["High24h"].NewValue;

                row.Cells["Low1min"].Value = market.MI["Low1min"].NewValue;
                row.Cells["Low3min"].Value = market.MI["Low3min"].NewValue;
                row.Cells["Low5min"].Value = market.MI["Low5min"].NewValue;
                row.Cells["Low15min"].Value = market.MI["Low15min"].NewValue;
                row.Cells["Low30min"].Value = market.MI["Low30min"].NewValue;
                row.Cells["Low1h"].Value = market.MI["Low1h"].NewValue;
                row.Cells["Low2h"].Value = market.MI["Low2h"].NewValue;
                row.Cells["Low4h"].Value = market.MI["Low4h"].NewValue;
                row.Cells["Low6h"].Value = market.MI["Low6h"].NewValue;
                row.Cells["Low12h"].Value = market.MI["Low12h"].NewValue;
                row.Cells["Low24h"].Value = market.MI["Low24h"].NewValue;

                row.Cells["Amplitude1min"].Value = market.MI["Amplitude1min"].ChangeValueProcentage;
                row.Cells["Amplitude1min"].Style.BackColor = market.MI["Amplitude1min"].BackColorColumn;
                row.Cells["Amplitude3min"].Value = market.MI["Amplitude3min"].ChangeValueProcentage;
                row.Cells["Amplitude3min"].Style.BackColor = market.MI["Amplitude3min"].BackColorColumn;
                row.Cells["Amplitude5min"].Value = market.MI["Amplitude5min"].ChangeValueProcentage;
                row.Cells["Amplitude5min"].Style.BackColor = market.MI["Amplitude5min"].BackColorColumn;
                row.Cells["Amplitude15min"].Value = market.MI["Amplitude15min"].ChangeValueProcentage;
                row.Cells["Amplitude15min"].Style.BackColor = market.MI["Amplitude15min"].BackColorColumn;
                row.Cells["Amplitude30min"].Value = market.MI["Amplitude30min"].ChangeValueProcentage;
                row.Cells["Amplitude30min"].Style.BackColor = market.MI["Amplitude30min"].BackColorColumn;
                row.Cells["Amplitude1h"].Value = market.MI["Amplitude1h"].ChangeValueProcentage;
                row.Cells["Amplitude1h"].Style.BackColor = market.MI["Amplitude1h"].BackColorColumn;
                row.Cells["Amplitude2h"].Value = market.MI["Amplitude2h"].ChangeValueProcentage;
                row.Cells["Amplitude2h"].Style.BackColor = market.MI["Amplitude2h"].BackColorColumn;
                row.Cells["Amplitude4h"].Value = market.MI["Amplitude4h"].ChangeValueProcentage;
                row.Cells["Amplitude4h"].Style.BackColor = market.MI["Amplitude4h"].BackColorColumn;
                row.Cells["Amplitude6h"].Value = market.MI["Amplitude6h"].ChangeValueProcentage;
                row.Cells["Amplitude6h"].Style.BackColor = market.MI["Amplitude6h"].BackColorColumn;
                row.Cells["Amplitude12h"].Value = market.MI["Amplitude12h"].ChangeValueProcentage;
                row.Cells["Amplitude12h"].Style.BackColor = market.MI["Amplitude12h"].BackColorColumn;
                row.Cells["Amplitude24h"].Value = market.MI["Amplitude24h"].ChangeValueProcentage;
                row.Cells["Amplitude24h"].Style.BackColor = market.MI["Amplitude24h"].BackColorColumn;

                row.Cells["VolumeQuote1min"].Value = market.MI["VolumeQuote1min"].NewValue;
                row.Cells["VolumeQuote3min"].Value = market.MI["VolumeQuote3min"].NewValue;
                row.Cells["VolumeQuote5min"].Value = market.MI["VolumeQuote5min"].NewValue;
                row.Cells["VolumeQuote15min"].Value = market.MI["VolumeQuote15min"].NewValue;
                row.Cells["VolumeQuote30min"].Value = market.MI["VolumeQuote30min"].NewValue;
                row.Cells["VolumeQuote1h"].Value = market.MI["VolumeQuote1h"].NewValue;
                row.Cells["VolumeQuote2h"].Value = market.MI["VolumeQuote2h"].NewValue;
                row.Cells["VolumeQuote4h"].Value = market.MI["VolumeQuote4h"].NewValue;
                row.Cells["VolumeQuote6h"].Value = market.MI["VolumeQuote6h"].NewValue;
                row.Cells["VolumeQuote12h"].Value = market.MI["VolumeQuote12h"].NewValue;
                row.Cells["VolumeQuote24h"].Value = market.MI["VolumeQuote24h"].NewValue;

                row.Cells["VolumeBase1min"].Value = market.MI["VolumeBase1min"].NewValue;
                row.Cells["VolumeBase3min"].Value = market.MI["VolumeBase3min"].NewValue;
                row.Cells["VolumeBase5min"].Value = market.MI["VolumeBase5min"].NewValue;
                row.Cells["VolumeBase15min"].Value = market.MI["VolumeBase15min"].NewValue;
                row.Cells["VolumeBase30min"].Value = market.MI["VolumeBase30min"].NewValue;
                row.Cells["VolumeBase1h"].Value = market.MI["VolumeBase1h"].NewValue;
                row.Cells["VolumeBase2h"].Value = market.MI["VolumeBase2h"].NewValue;
                row.Cells["VolumeBase4h"].Value = market.MI["VolumeBase4h"].NewValue;
                row.Cells["VolumeBase6h"].Value = market.MI["VolumeBase6h"].NewValue;
                row.Cells["VolumeBase12h"].Value = market.MI["VolumeBase12h"].NewValue;
                row.Cells["VolumeBase24h"].Value = market.MI["VolumeBase24h"].NewValue;

                row.Cells["VolumeChange1min"].Value = market.MI["VolumeChange1min"].ChangeValueProcentage;
                row.Cells["VolumeChange1min"].Style.BackColor = market.MI["VolumeChange1min"].BackColorColumn;
                row.Cells["VolumeChange3min"].Value = market.MI["VolumeChange3min"].ChangeValueProcentage;
                row.Cells["VolumeChange3min"].Style.BackColor = market.MI["VolumeChange3min"].BackColorColumn;
                row.Cells["VolumeChange5min"].Value = market.MI["VolumeChange5min"].ChangeValueProcentage;
                row.Cells["VolumeChange5min"].Style.BackColor = market.MI["VolumeChange5min"].BackColorColumn;
                row.Cells["VolumeChange15min"].Value = market.MI["VolumeChange15min"].ChangeValueProcentage;
                row.Cells["VolumeChange15min"].Style.BackColor = market.MI["VolumeChange15min"].BackColorColumn;
                row.Cells["VolumeChange30min"].Value = market.MI["VolumeChange30min"].ChangeValueProcentage;
                row.Cells["VolumeChange30min"].Style.BackColor = market.MI["VolumeChange30min"].BackColorColumn;
                row.Cells["VolumeChange1h"].Value = market.MI["VolumeChange1h"].ChangeValueProcentage;
                row.Cells["VolumeChange1h"].Style.BackColor = market.MI["VolumeChange1h"].BackColorColumn;
                row.Cells["VolumeChange2h"].Value = market.MI["VolumeChange2h"].ChangeValueProcentage;
                row.Cells["VolumeChange2h"].Style.BackColor = market.MI["VolumeChange2h"].BackColorColumn;
                row.Cells["VolumeChange4h"].Value = market.MI["VolumeChange4h"].ChangeValueProcentage;
                row.Cells["VolumeChange4h"].Style.BackColor = market.MI["VolumeChange4h"].BackColorColumn;
                row.Cells["VolumeChange6h"].Value = market.MI["VolumeChange6h"].ChangeValueProcentage;
                row.Cells["VolumeChange6h"].Style.BackColor = market.MI["VolumeChange6h"].BackColorColumn;
                row.Cells["VolumeChange12h"].Value = market.MI["VolumeChange12h"].ChangeValueProcentage;
                row.Cells["VolumeChange12h"].Style.BackColor = market.MI["VolumeChange12h"].BackColorColumn;
                row.Cells["VolumeChange24h"].Value = market.MI["VolumeChange24h"].ChangeValueProcentage;
                row.Cells["VolumeChange24h"].Style.BackColor = market.MI["VolumeChange24h"].BackColorColumn;


            });
        }
        private void AddRow(DataGridView dg, Market market)
        {
            InThread(()=> dg.Rows.Add(
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",


            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""));
        }

        private void ColumnAction(string ColumnName, bool Checked)
        {
            InThread(() => this.dgBinanceTable.Columns[ColumnName].Visible = Checked);
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
            foreach(var notify in cfg.notifications.Guid)
            {
                this.ddlGiud.Items.Insert(0, notify.Key);
                foreach (var n in notify.Value)
                {
                    
                    this.dgNotification.Rows.Add(
                                            (n.Symbol.Count > 1) ? $"{n.Symbol.Count} Pairs" : $"{n.Symbol[0].ToString()}",
                                            n.Type,
                                            n.Timeframe,
                                            n.Change,
                                            notify.Key,
                                            n.TelegramName,
                                            n.Guid);
                }
            }

            this.tbTelegramApi.Text = this.cfg.TelegramApiKey;

            this.dgTgChats.Rows.Clear();
            foreach (var chatid in cfg.TelegramChats)
            {
                this.ddlChatId.DataSource = new BindingSource(this.cfg.TelegramChats, null);
                this.ddlChatId.DisplayMember = "Value";
                this.ddlChatId.ValueMember = "Key";
                this.dgTgChats.Rows.Add(chatid.Key, chatid.Value);
            }

            this.tbTelegramChatId.Text = "";
            this.tbTelegramChatName.Text = "";

            this.tbDelay.Text = cfg.Delay.ToString();

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

            SaveConfig();
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
            if (this.lbFavorite.SelectedItem != null && this.lbFavorite.SelectedItem.ToString() != "")
                this.lbFavorite.Items.Remove(this.lbFavorite.SelectedItem);
        }

        private void cbFavorite_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
        }

        private void btnTelegramTestMsg_Click(object sender, EventArgs e)
        {
            if (dgTgChats.SelectedRows.Count > 0)
            {
                foreach(DataGridViewRow row in dgTgChats.SelectedRows)
                {
                    try
                    {
                        TelegramBotClient bot = new TelegramBotClient(this.tbTelegramApi.Text);
                        bot.SendTextMessageAsync(Convert.ToInt64(row.Cells[0].Value), $"Тестовое сообщение от \"{row.Cells[1].Value.ToString()}\"", parseMode: ParseMode.Markdown);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка в заполненных полях телеграма");
                    }
                }
            }
            else
            {
                MessageBox.Show("Для отправки тестового сообщения выберете чат из списка");
            }
        }

        private void rbAllTimeframe_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
            this.cfg.Timeframe1min = rb1minTimeframe.Checked;
            this.cfg.TimeframeAll = rbAllTimeframe.Checked;
            SaveConfig();
        }

        private void rb1minTimeframe_CheckedChanged(object sender, EventArgs e)
        {
            this.dgBinanceTable.Rows.Clear();
            this.cfg.Timeframe1min = rb1minTimeframe.Checked;
            this.cfg.TimeframeAll = rbAllTimeframe.Checked;
            SaveConfig();
        }

        private void tbNotifyChange_Leave(object sender, EventArgs e)
        {
            if (!Decimal.TryParse(this.tbNotifyChange.Text, out decimal price))
            {
                MessageBox.Show($"Ошибка в коверитировании. Двоичное число должно быть через '{Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)}'");
                return;
            }
        }

        private void btnAddTgChat_Click(object sender, EventArgs e)
        {
            if (this.tbTelegramChatId.Text != "")
            {
                if (Int64.TryParse(this.tbTelegramChatId.Text, out long chatid))
                {
                    InThread(() =>
                    {
                        this.dgTgChats.Rows.Add(
                            this.tbTelegramChatId.Text,
                            this.tbTelegramChatName.Text);
                    });
                    this.cfg.TelegramChats.Add(chatid, this.tbTelegramChatName.Text);

                    SaveConfig();

                    this.ddlChatId.DataSource = new BindingSource(this.cfg.TelegramChats, null);
                    this.ddlChatId.DisplayMember = "Value";
                    this.ddlChatId.ValueMember = "Key";

                    this.tbTelegramChatId.Text = "";
                    this.tbTelegramChatName.Text = "";
                }
                else
                {
                    MessageBox.Show("Id чата не соответствует требованиям");
                }
                
            }
            else
            {
                MessageBox.Show("Заполните Id чата");
            }
        }

        private void btnDeleteTgChat_Click(object sender, EventArgs e)
        {
            if (this.dgTgChats.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgTgChats.SelectedRows)
                {
                    this.cfg.TelegramChats.Remove(Convert.ToInt64(row.Cells[0].Value.ToString()));
                    SaveConfig();
                    InThread(() =>
                    {
                        this.ddlChatId.DataSource = new BindingSource(this.cfg.TelegramChats, null);
                        this.ddlChatId.DisplayMember = "Value";
                        this.ddlChatId.ValueMember = "Key";
                        this.dgTgChats.Rows.Remove(row);
                    });
                }
            }
        }

        private void SaveConfig()
        {
            Config.Save(cfg);
            if (client != null)
                client.UpdateNotifications(cfg);
        }

        private void tbDelay_Leave(object sender, EventArgs e)
        {
            if (Int32.TryParse(this.tbDelay.Text, out int deley))
            {
                cfg.Delay = deley;
                SaveConfig();
            }
            else
            {
                MessageBox.Show("Значение должно быть целым числом");
            }
        }

        private void tbTelegramApi_Leave(object sender, EventArgs e)
        {
            cfg.TelegramApiKey = this.tbTelegramApi.Text;
        }

        private void cbNotifyAllSymbols_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNotifyAllSymbols.Checked)
                for (int i = 0; i < cblNotifySymbols.Items.Count; i++)
                {
                    cblNotifySymbols.SetItemChecked(i, true);
                }
        }
    }
}
