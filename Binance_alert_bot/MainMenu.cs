using Binance_alert_bot.Binance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            Task.Run(() =>
            {
                client = new BinanceBotClient();
                client.BinanceLog += Logs;
                client.BinanceSetCredentials();
            });
        }
        #endregion

        #region Methods
        private void InThread(MethodInvoker mth)
        {
            start:
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
                goto start;
            }
        }
        private void Logs(string msg)
        {
            InThread(() => this.tbLogs.AppendText($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} {msg}\n"));
        }
        #endregion

        

        
    }
}
