using NuGet.Common;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace AlpTest.Models
{

    public class BpiSample
    {
        public DateTime time { get; set; }
        public float usd { get; set; }
        public float gbp { get; set; }
        public float eur { get; set; }
        public float czk { get; set; }

        public BpiSample(DateTime time, float usd, float gbp, float eur, float czk)
        {
            this.time = time;
            this.usd = usd;
            this.gbp = gbp;
            this.eur = eur;
            this.czk = czk;
        }   
    }


    public class BpiElement 
    { 
        public string code { get; set; } = string.Empty;
        public string symbol { get; set; } = string.Empty;
        public string rate { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public float rate_float { get; set; }
    }


    public class Bitcoin
    {
        public Dictionary<string, string> time = new Dictionary<string, string>();
        public Dictionary<string, BpiElement> bpi = new Dictionary<string, BpiElement>();
        public string chartName { get; set; } = string.Empty;


        [Display(Name = "Time of last sample")]
        public DateTime currentTime
        {
            get { return DateTime.Parse(time["updatedISO"]); }
        }

        [Display(Name = "Disclaimer")]
        public string disclaimer { get; set; } = string.Empty;

        [Display(Name = "USD")]
        [DisplayFormat(DataFormatString = "{0:N}")]// $
        public float valueUSD
        {
            get { return bpi["USD"].rate_float; }

        }

        [Display(Name = "GBP")]
        [DisplayFormat(DataFormatString = "{0:N}")]// £
        public float valueGBP
        {
            get { return bpi["GBP"].rate_float; }

        }

        [Display(Name = "EUR")]
        [DisplayFormat(DataFormatString = "{0:N}")]// €
        public float valueEUR
        {
            get { return bpi["EUR"].rate_float; }

        }

        [Display(Name = "CZK")]
        [DisplayFormat(DataFormatString = "{0:N}")] // Kč
        public float valueCZK
        {
            get { return bpi["CZK"].rate_float; }

        }

        private void Set(string key, BpiElement value)
        {
            if (bpi.ContainsKey(key))
            {
                bpi[key] = value;
            }
            else
            {
                bpi.Add(key, value);
            }
        }
        private BpiElement Get(string key)
        {
            BpiElement result = new BpiElement();

            if (bpi.ContainsKey(key))
            {
                result = bpi[key];
            }

            return result;
        }

        public void AddCZK(float rate)
        {
            float czk = valueEUR * rate;
            BpiElement e = new BpiElement();
            e.rate_float = czk;
            Set("CZK", e);
        }
    }
}
