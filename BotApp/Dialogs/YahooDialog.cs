using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BotApp.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BotApp.Dialogs
{
    [Serializable]
    public class YahooDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(GetStockListen);
        }


        private static async Task<double?> GetStockRateAsync(string StockSymbol)
        {
            try
            {
                string ServiceURL = $"http://finance.yahoo.com/d/quotes.csv?s={StockSymbol}&f=sl1d1nd";
                string ResultInCSV;
                using (WebClient client = new WebClient())
                {
                    ResultInCSV = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
                }
                var FirstLine = ResultInCSV.Split('\n')[0];
                var Price = FirstLine.Split(',')[1];
                if (Price != null && Price.Length >= 0)
                {
                    double result;
                    if (double.TryParse(Price, out result))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (WebException ex)
            {
                //handle your exception here  
                throw ex;
            }
        }

        public virtual async Task GetStockListen(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var StockSymbol = await argument;

            string StockRateString;
            StockLUIS StLUIS = await YahooDialog.GetEntityFromLUIS(StockSymbol.Text);
          

            if (StLUIS.intents.Count() > 0 && StLUIS.entities.Count() > 0)
            {
                switch (StLUIS.intents[0].intent)
                {
                    case "StockPrice":
                        StockRateString = await YahooDialog.GetStock(StLUIS.entities[0].entity);
                        break;
                    case "StockPrice2":
                        StockRateString = await YahooDialog.GetStock(StLUIS.entities[0].entity);
                        break;
                    default:
                        StockRateString = "Sorry, I am not getting you...";
                        break;
                }
            }
            else
            {
                StockRateString = "Sorry, I am not getting you...";
            }

            await context.PostAsync(StockRateString);
              context.Wait(GetStockListen);

            //double? dblStockValue = await YahooDialog.GetStockRateAsync(StockSymbol.Text);
            //if (dblStockValue == null)
            //{
            //    await context.PostAsync($"This \"{StockSymbol.Text}\" is not an valid stock symbol");
            //    context.Wait(GetStock);

            //}
            //else
            //{
            //    await context.PostAsync($"Stock : {StockSymbol.Text}\n Price : {dblStockValue}");


            //    context.Wait(GetStock);
            //}

        }

        private static async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await YahooDialog.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }

        private static async Task<StockLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            StockLUIS Data = new StockLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/60ce3b58-2007-4986-b29b-a5bde35de592?subscription-key=78e41751e72846acb5d41e25c8a9209e&timezoneOffset=0&verbose=true&spellCheck=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<StockLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }
    }

}

       
    
