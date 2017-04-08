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
    public class TableMemoKeywordRelation : TableEntity
    {
        public byte[] LinkedMemoIds { get; set; }
        public new DateTime Timestamp { get; set; }
    }
}
