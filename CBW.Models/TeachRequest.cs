using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Models
{
    public class TeachRequest
    {
        public string Content { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime? RemindTime { get; set; }
        public bool? HasAlarm { get; set; }
    }
}
