using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.PassportMonitoring
{

    public class PassportSpace
    {
        private List<Datum> _data;
        public PassportSpace()
        {
            _data = new List<Datum>();
        }
        [JsonProperty("status")]
        public long status { get; set; }
        [JsonProperty("data")]
        public List<Datum> data 
        {
            get { return this._data; }
            set { this._data = value; }
        }
    }

    public class Datum
    {
        [JsonProperty("operator_id")]
        public string operator_id { get; set; }
        [JsonProperty("zoneid")]
        public string zoneid { get; set; }
        [JsonProperty("zonenumber")]
        public string zonenumber { get; set; }
        [JsonProperty("zonename")]
        public string zonename { get; set; }
        [JsonProperty("zonetype")]
        public Zonetype zonetype { get; set; }
        [JsonProperty("enablecashcheckin")]
        public string enablecashcheckin { get; set; }
        [JsonProperty("forcelogofftime")]
        public string forcelogofftime { get; set; }
        [JsonProperty("locations_spaces")]
        public Locations_Spaces[] locations_spaces { get; set; }
        [JsonProperty("space_count")]
        public int space_count { get; set; }
        [JsonProperty("location_count")]
        public int location_count { get; set; }
        [JsonProperty("checkinenabled")]
        public bool checkinenabled { get; set; }
        [JsonProperty("location_lpn")]
        public object[] location_lpn { get; set; }
    }

    public class Zonetype
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
    }

    public class Locations_Spaces
    {
        [JsonProperty("locationid")]
        public string locationid { get; set; }
        [JsonProperty("locationname")]
        public string locationname { get; set; }
        [JsonProperty("zoneid")]
        public string zoneid { get; set; }
        [JsonProperty("spaces")]
        public Space[] spaces { get; set; }
    }

    public class Space
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("number")]
        public string number { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("zone_id")]
        public string zone_id { get; set; }
        [JsonProperty("zonenumber")]
        public string zonenumber { get; set; }
        [JsonProperty("entrytime")]
        public string entrytime { get; set; }
        [JsonProperty("exittime")]
        public string exittime { get; set; }
        [JsonProperty("formattedentrytime")]
        public string formattedentrytime { get; set; }
        [JsonProperty("formattedexittime")]
        public string formattedexittime { get; set; }
        [JsonProperty("activelpn")]
        public bool activelpn { get; set; }
        [JsonProperty("expiration")]
        public string expiration { get; set; }
        [JsonProperty("expirationtimeinsecs")]
        public string expirationtimeinsecs { get; set; }
        [JsonProperty("isactive")]
        public object isactive { get; set; }
        [JsonProperty("occupied")]
        public string occupied { get; set; }
    }

}
