using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Models
{
    public class Memo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [JsonProperty("Content")]
        public string RawContent { get; set; }
        public string[] Keywords { get; set; }
        [JsonIgnore]
        public int[] KeywordIds { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? RemindTime { get; set; }
        public bool HasAlarm { get; set; }
    }
}
