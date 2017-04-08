using CBW.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class DictionaryProvider : BlobDataProvider
    {
        public DictionaryProvider(string connectionString)
            : base(connectionString)
        {
        }

        public WordDictionary DownloadDictionary()
        {
            Stream stream = new MemoryStream();
            base.Download("dict.bin", stream);
            stream.Seek(0, SeekOrigin.Begin);
            IFormatter formatter = new BinaryFormatter();
            return (WordDictionary)formatter.Deserialize(stream);
        }

        //public RelevanceTable DownloadRelevanceTable()
        //{
        //    Stream stream = new MemoryStream();
        //    base.Download("rel.bin", stream);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    IFormatter formatter = new BinaryFormatter();
        //    return (RelevanceTable)formatter.Deserialize(stream);
        //}

        public void UploadDictionary(WordDictionary dict)
        {
            Stream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, dict);
            stream.Seek(0, SeekOrigin.Begin);
            base.Upload("dict.bin", stream);
        }

        //public void UploadRelevanceTable(RelevanceTable table)
        //{
        //    Stream stream = new MemoryStream();
        //    IFormatter formatter = new BinaryFormatter();
        //    formatter.Serialize(stream, table);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    base.Upload("rel.bin", stream);
        //}
    }
}
