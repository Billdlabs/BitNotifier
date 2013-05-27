using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using BLNotifications.Annotations;
using BLNotifications.Models;

namespace BLNotifications
{
    class notifications : INotifyPropertyChanged
    {
        private static volatile notifications instance;
        private static readonly object syncRoot = new object();

        private notifications()
        {
            startTimer();
        }

        public static notifications Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new notifications();
                        }
                    }
                }
                return instance;
            }
        }

        //various globally accessable properties.
        public event PropertyChangedEventHandler PropertyChanged;
        public string MtGoxAvg { get; set; }
        public string MtGoxBuy { get; set; }
        public string MtGoxHigh { get; set; }
        public string MtGoxLast { get; set; }
        public decimal MtGoxLastDec { get; set; }
        public string MtGoxLow { get; set; }
        public string MtGoxSell { get; set; }
        public string MtGoxVol { get; set; }
        public bool ResponseGood { get; set; }
        public MtGoxResponse MtGoxResponse { get; set; }
        public Uri RequestURI { get; set; }
        public int CurrencyNumber { get; set; }
        
        //api props
        public string MtGoxSecret { get; set; }
        public string MtGoxApiKey { get; set; }


        private void startTimer()
        {
            var goxTimer = new System.Timers.Timer(10000) { AutoReset = true };
            goxTimer.Elapsed += GoxCheck;
            goxTimer.Enabled = true;
            GC.KeepAlive(goxTimer);
        }

        public void RefreshOrderHistory()
        {
            MessageBox.Show(MtGoxApiKey + MtGoxSecret);
            
        }

        public void UpdateCurrency()
        {
            switch (CurrencyNumber)
            {
                case 0:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCUSD/ticker");
                    break;
                case 1:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCAUD/ticker");
                    break;
                case 2:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCCAD/ticker");
                    break;
                case 3:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCCHF/ticker");
                    break;
                case 4:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCCNY/ticker");
                    break;
                case 5:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCDKK/ticker");
                    break;
                case 6:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCEUR/ticker");
                    break;
                case 7:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCGBP/ticker");
                    break;
                case 8:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCHKD/ticker");
                    break;
                case 9:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCJPY/ticker");
                    break;
                case 10:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCNZD/ticker");
                    break;
                case 11:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCPLN/ticker");
                    break;
                case 12:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCRUB/ticker");
                    break;
                case 13:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCSEK/ticker");
                    break;
                case 14:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCSGD/ticker");
                    break;
                case 15:
                    RequestURI = new Uri("https://data.mtgox.com/api/1/BTCTHB/ticker");
                    break;
            }
        }

        private void GoxCheck(object source, EventArgs e)
        {
            try
            {
                MtGoxResponse tickerResponse;
                var request = (HttpWebRequest)WebRequest.Create(RequestURI);
                request.Timeout = 60000;
                request.Method = "GET";
                request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US, en");
                request.UserAgent = "BilldLabs Notifier";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var js = new JavaScriptSerializer();
                        var responseText = reader.ReadToEnd();
                        tickerResponse = (MtGoxResponse)js.Deserialize(responseText, typeof(MtGoxResponse));

                        var result = tickerResponse.@return;

                        MtGoxAvg = result.avg.display;
                        MtGoxBuy = result.buy.display;
                        MtGoxHigh = result.high.display;
                        MtGoxLast = result.last.display;
                        MtGoxLastDec = result.last.value;
                        MtGoxLow = result.low.display;
                        MtGoxSell = result.sell.display;
                        MtGoxVol = result.vol.display;
                        ResponseGood = true;
                        PropertyChanged(this, new PropertyChangedEventArgs("All Properties."));
                    }
                }
            }
            catch (Exception ex)
            {
                var exc = e;
                //most likely the server ignored our request.
                ResponseGood = false;
                PropertyChanged(this, new PropertyChangedEventArgs("ResponseGood."));
            }
        }
    }
}
