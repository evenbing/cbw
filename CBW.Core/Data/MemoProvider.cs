using CBW.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class MemoProvider : TableDataProvider
    {
        public MemoProvider(string connectionString)
            : base(connectionString, "cbwmemo")
        {
        }

        public void Add(string userId, Memo memo)
        {
            TableMemoEntity tableMemo = new TableMemoEntity()
            {
                RowKey = (Int32.MaxValue - memo.Id).ToString(),
                KeywordIds = memo.KeywordIds.GetBytes(),
                PartitionKey = userId,
                RawContent = memo.RawContent,
                Longitude = memo.Longitude == null ? 0 : memo.Longitude.Value,
                Latitude = memo.Latitude == null ? 0 : memo.Latitude.Value,
                CreatedTime = memo.CreatedTime,
                RemindTime = memo.RemindTime == null ? DateTime.UtcNow : memo.RemindTime.Value,
                HasAlarm = memo.HasAlarm,
                IsDeleted = false
            };
            this.Context.AddObject(this.TableName, tableMemo);
            this.Context.SaveChanges();
        }

        public void Update(string userId, Memo memo)
        {
            TableMemoEntity entity = this.Context.CreateQuery<TableMemoEntity>(this.TableName)
                .Where(x => x.PartitionKey == userId && x.RowKey == (Int32.MaxValue - memo.Id).ToString()).FirstOrDefault();
            if (entity != null && !entity.IsDeleted)
            {
                entity.HasAlarm = memo.HasAlarm;
                entity.RawContent = memo.RawContent;
                entity.RemindTime = memo.RemindTime == null ? DateTime.UtcNow : memo.RemindTime.Value;
            }
            else
            {
                throw new MemoNotFoundException(memo.Id);
            }
            this.Context.UpdateObject(entity);
            this.Context.SaveChanges();
        }

        public void Delete(string userId, int memoId)
        {
            TableMemoEntity entity = this.Context.CreateQuery<TableMemoEntity>(this.TableName)
                .Where(x => x.PartitionKey == userId && x.RowKey == (Int32.MaxValue - memoId).ToString()).FirstOrDefault();
            if (entity != null && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
            }
            else
            {
                throw new MemoNotFoundException(memoId);
            }
            this.Context.UpdateObject(entity);
            this.Context.SaveChanges();
        }

        public bool TryGet(string userId, int memoId, out Memo memo)
        {
            TableMemoEntity tableMemo = this.Context.CreateQuery<TableMemoEntity>(this.TableName)
               .Where(
               x => x.PartitionKey == userId && x.RowKey == (Int32.MaxValue - memoId).ToString()).FirstOrDefault();

            if (tableMemo == null)
            {
                memo = null;
                return false;
            }
            if (tableMemo.IsDeleted)
            {
                memo = null;
                return false;
            }
            memo = Transform(tableMemo);
            return true;
        }

        public Memo Get(string userId, int memoId)
        {
            TableMemoEntity tableMemo = this.Context.CreateQuery<TableMemoEntity>(this.TableName)
                .Where(
                x => x.PartitionKey == userId && x.RowKey == (Int32.MaxValue - memoId).ToString()).FirstOrDefault();
            if (tableMemo == null)
            {
                throw new MemoNotFoundException(memoId);
            }
            if (tableMemo.IsDeleted)
            {
                throw new MemoNotFoundException(memoId);
            }
            return Transform(tableMemo);
        }

        private Memo Transform(TableMemoEntity tableMemo)
        {
            return new Memo()
            {
                Id = Int32.MaxValue - Int32.Parse(tableMemo.RowKey),
                KeywordIds = tableMemo.KeywordIds.ToInt32Array(),
                Keywords = CBW.NaturalLang.KeywordsExtraction.ExtractUniqueKeywords(tableMemo.RawContent),
                RawContent = tableMemo.RawContent,
                UserId = tableMemo.PartitionKey,
                Latitude = (tableMemo.Latitude == 0) ? null : new double?(tableMemo.Latitude),
                Longitude = (tableMemo.Longitude == 0) ? null : new double?(tableMemo.Longitude),
                HasAlarm = tableMemo.HasAlarm,
                CreatedTime = tableMemo.CreatedTime,
                RemindTime = tableMemo.RemindTime
            };
        }

        public IEnumerable<Memo> Get(string userId, int limit = 5, int begin = Int32.MaxValue - 1)
        {
            int zeroId = GetCurrentMemoId(userId);

            TableQuery<TableMemoEntity> projectionQuery = new TableQuery<TableMemoEntity>().Where(
                TableQuery.CombineFilters(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual,((long)Int32.MaxValue - begin).ToString())),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForBool("IsDeleted", QueryComparisons.Equal, false))).Take(limit);

            return this.Table.ExecuteQuery(projectionQuery)
                .Select(x => Transform((TableMemoEntity)x))
                .ToList();
        }

        public int GetCurrentMemoId(string userId)
        {
            var first = this.Context.CreateQuery<TableMemoEntity>(this.TableName)
                .Where(
                x => x.PartitionKey == userId).Take(1).FirstOrDefault();
            if (first == null)
            {
                return 0;
            }
            return Int32.MaxValue - Int32.Parse(first.RowKey);
        }
    }
}
