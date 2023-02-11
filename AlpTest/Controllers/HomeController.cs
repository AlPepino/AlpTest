using AlpTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace AlpTest.Controllers
{
    public class HomeController : Controller
    {

        private static DateTime TimeStampBtn { get; set; }
        private static DateOnly DateCNB { get; set; }
        public static float RateCZK { get; private set; }
        public static Bitcoin CoinData = new Bitcoin();


        public async Task<IActionResult> Index()
        {
            DateTime time = DateTime.Now;
            if (DateCNB != DateOnly.FromDateTime(time))
            {
                DateCNB = DateOnly.FromDateTime(time);
                using (var clientCNB = new HttpClient())
                {
                    using (var responseCNB = await clientCNB.GetAsync("https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt"))
                    {
                        string apiResponseCNB = await responseCNB.Content.ReadAsStringAsync();
                        string[] rows = apiResponseCNB.Split("\n");
                        foreach (string row in rows)
                        {
                            if (row.Contains("EUR"))
                            {
                                string[] euro = row.Split("|");
                                RateCZK = float.Parse(euro[euro.Count() - 1]);
                                break;
                            }
                        }
                    }
                }
            }
            var t = TimeStampBtn;
            if ( t.AddSeconds(20) > time )
            {
                return View(CoinData);
            } else
            {
                TimeStampBtn = time;
                CoinData = new Bitcoin();
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        CoinData = JsonConvert.DeserializeObject<Bitcoin>(apiResponse);
                    }
                }
                CoinData.AddCZK(RateCZK);
                return View(CoinData);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> UpdateBitcoin()
        {
            DateTime time = DateTime.Now;
            var t = TimeStampBtn;
            if (t.AddSeconds(20) <= time)
            {
                TimeStampBtn = time;
                CoinData = new Bitcoin();
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        CoinData = JsonConvert.DeserializeObject<Bitcoin>(apiResponse);
                    }
                }
                CoinData.AddCZK(RateCZK);
            }
            BpiSample sample = new BpiSample(CoinData.currentTime, CoinData.valueUSD, CoinData.valueGBP, CoinData.valueEUR, CoinData.valueCZK);
            return Json(sample);
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}