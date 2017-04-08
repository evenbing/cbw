using CBW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class MemoKeywordRelationProvider : TableDataProvider
    {
        public MemoKeywordRelationProvider(string connectionString)
            : base(connectionString, "cbwmemokey")
        {
        }

        public void Add(string userId, int memoId, int[] keywordIds)
        {
            Parallel.ForEach(keywordIds, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(int kid)
            {
                var data = this.Context.CreateQuery<TableMemoKeywordRelation>(this.TableName)
                    .Where(x => x.PartitionKey == userId && x.RowKey == kid.ToString()).FirstOrDefault();
                if (data == null)
                {
                    this.Context.AddObject(this.TableName, new TableMemoKeywordRelation()
                    {
                        PartitionKey = userId,
                        RowKey = kid.ToString(),
                        LinkedMemoIds = new int[] { memoId }.GetBytes()
                    });
                }
                else
                {
                    data.LinkedMemoIds = data.LinkedMemoIds.ToInt32Array().Concat(new int[] { memoId }).ToArray().GetBytes();
                    this.Context.UpdateObject(data);
                }
            });
            this.Context.SaveChanges();
        }

        public int[] GetRelatedMemoIds(string userId, int keywordId)
        {
            var data = this.Context.CreateQuery<TableMemoKeywordRelation>(this.TableName)
                .Where(x => x.PartitionKey == userId && x.RowKey == keywordId.ToString())
                .FirstOrDefault();
            if (data == null)
            {
                return new int[0];
            }
            else
            {
                return data.LinkedMemoIds.ToInt32Array();
            }
        }
    }
}
