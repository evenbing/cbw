using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Models
{
    [DataServiceEntity]
    public class TableMemoEntity : TableEntity
    {
        public byte[] KeywordIds { get; set; }
        public string RawContent { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime RemindTime { get; set; }
        public bool HasAlarm { get; set; }
        public bool IsDeleted { get; set; }
        public new DateTime Timestamp { get; set; }
    }
}
