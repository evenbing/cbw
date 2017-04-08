using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.WindowsPhone
{
    public class MemoItem
    {
        public DateTime CreatedTime {get; set;}

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime RemindTime { get; set; }
        public string Content { get; set; }
        public bool HasAlarm { get; set; }

        private string[] _keywords;

        public string[] Keywords
        {
            get
            {
                return this._keywords;
            }
            set
            {
                this._keywords = value;
                KeywordsString = String.Join(", ", this._keywords);
            }
        }

        public string KeywordsString { get; set; }

        public int Id { get; set; }

        [JsonIgnore]
        public string ShortCreatedTime
        {
            get { return CreatedTime.ToString("HH:mm");  }
        }

        public MemoItem()
        {
            HasAlarm = false;
        }

        public override string ToString()
        {
            return Content + " Created Time";
        }

        public void TurnOffAlarm()
        {
            HasAlarm = true;
        }

        public void TurnOnAlarn()
        {
            HasAlarm = false;
        }
    }
}
