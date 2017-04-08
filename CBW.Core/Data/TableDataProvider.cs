using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class TableDataProvider
    {
        public TableServiceContext Context { get; set; }
        public string TableName { get; set; }
        public string ConnectionString { get; set; }
        public CloudTableClient Client { get; set; }
        public CloudTable Table { get; set; }

        public TableDataProvider(string connectionString, string tableName)
        {
            this.TableName = tableName;
            CloudStorageAccount account;
            account = CloudStorageAccount.Parse(connectionString);
            this.Client = account.CreateCloudTableClient();
            this.Table = this.Client.GetTableReference(tableName);
            this.Table.CreateIfNotExists();
            this.ConnectionString = connectionString;
            this.Context = this.Client.GetTableServiceContext();
        }
    }
}
