using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;


namespace DataAccess.Model.PassportMonitoring
{
    public partial class PassportPlate
    {
        [JsonProperty("parkerentryid")]
        public string Parkerentryid { get; set; }

        [JsonProperty("entrytime")]
        public DateTime? Entrytime { get; set; }

        [JsonProperty("exittime")]
        public DateTime? Exittime { get; set; }

        [JsonProperty("vehicleid")]
        public string Vehicleid { get; set; }

        [JsonProperty("licenseplatenumber")]
        public string Licenseplatenumber { get; set; }

        [JsonProperty("expirationtimeinsecs")]
        public string Expirationtimeinsecs { get; set; }

        [JsonProperty("lpn")]
        public string Lpn { get; set; }

        [JsonProperty("stateabbreviation")]
        public string Stateabbreviation { get; set; }

        [JsonProperty("ratename")]
        public string Ratename { get; set; }

        [JsonProperty("zonenumber")]
        public string Zonenumber { get; set; }

        public int ExpiredMinutes(CustomerTime customerTime, int? gracePeriodInSeconds)
        {
            if (this.Exittime.HasValue)
            {
                return (int)this.Exittime.Value.AddSeconds(gracePeriodInSeconds.Value).Subtract(customerTime.CurrentTime.Value).TotalMinutes;
            }
            else
            {
                return (-1);
            }
        }
        public int ExpiredMinutes(int? graceseconds)
        {
            if (!string.IsNullOrEmpty(this.Expirationtimeinsecs))
            {
                TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt32(this.Expirationtimeinsecs));
                if (graceseconds.HasValue)
                   span = span.Add(new TimeSpan(0, 0, 0, graceseconds.Value));
                return (int)span.TotalMinutes;
            }
            else
            {
                return (-1);
            }
        }
    }
}
