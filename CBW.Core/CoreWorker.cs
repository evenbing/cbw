using CBW.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class CoreWorker
    {
        public static WordDictionary Dictionary { get; set; }
        //public static RelevanceTable Table { get; set; }
        public string ConnectionString { get; set; }
        public static List<string> TeachResponses { get; set; }

        public CoreWorker(string connectionString)
        {
            this.ConnectionString = connectionString;

            if (Dictionary == null)
            {
                DictionaryProvider provider = new DictionaryProvider(
                    connectionString);
                Dictionary = provider.DownloadDictionary();
                //Table = provider.DownloadRelevanceTable();
            }
            if (TeachResponses == null)
            {
                TeachResponses = File.ReadLines("Data\\TeachResponses.txt").ToList();
            }
        }

        public string Teach(string uid, TeachRequest request)
        {
            string[] requestWords = CBW.NaturalLang.KeywordsExtraction.ExtractUniqueKeywords((request.Content));
            //string[] requestWords = StringParser.Parse(request.Content);
            MemoProvider memoProvider = new MemoProvider(this.ConnectionString);
            int mid = memoProvider.GetCurrentMemoId(uid) + 1;
            Memo memo = new Memo()
            {
                Id = mid,
                RawContent = request.Content.ToLowerInvariant(),
                UserId = uid,
                CreatedTime = DateTime.UtcNow,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                RemindTime = request.RemindTime,
                HasAlarm = request.HasAlarm == null ? false : request.HasAlarm.Value
            };
            memoProvider.Add(uid, memo);

            this.AddKeywords(uid, memo, requestWords);
            return TeachResponses[new Random().Next(TeachResponses.Count)];
        }

        public void Update(string uid, Memo memo)
        {
            new MemoProvider(this.ConnectionString).Update(uid, memo);
            this.AddKeywords(uid, memo, CBW.NaturalLang.KeywordsExtraction.ExtractUniqueKeywords(memo.RawContent));
        }

        public IEnumerable<string> Retrieve(string uid, RetrieveRequest request)
        {
            //string[] requestWords = StringParser.Parse(request.Content);
            string[] requestWords = CBW.NaturalLang.KeywordsExtraction.ExtractUniqueKeywords(request.Content);
            HashSet<int> memosAnd = new HashSet<int>();
            HashSet<int> memosOr = new HashSet<int>();

            Parallel.ForEach<string>(requestWords, delegate(string keyword)
            {
                if (Dictionary.ContainsKey(keyword))
                {
                    int kid = Dictionary[keyword].Id;
                    int[] memoIds = new MemoKeywordRelationProvider(this.ConnectionString)
                        .GetRelatedMemoIds(uid, kid);

                    lock (this)
                    {
                        memosOr.UnionWith(memoIds);
                        if (memosAnd.Count == 0)
                        {
                            memosAnd.UnionWith(memoIds);
                        }
                        else
                        {
                            memosAnd.IntersectWith(memoIds);
                        }
                    }
                }
            });

            IEnumerable<Memo> memos = null;
            if (memosAnd.Count > 0)
            {
                memos = this.GetMemos(uid, memosAnd);
            }
            else
            {
                memos = this.GetMemos(uid, memosOr);
            }

            return memos.OrderByDescending(x => x.Id).Select(x => this.FormatSentence(x.RawContent));
        }

        protected IEnumerable<Memo> GetMemos(string uid, IEnumerable<int> memoIds)
        {
            List<Memo> results = new List<Memo>();
            Parallel.ForEach<int>(memoIds, delegate(int mid)
            {
                Memo memo;
                if (new MemoProvider(this.ConnectionString).TryGet(uid, mid, out memo))
                {
                    lock (this)
                    {
                        results.Add(memo);
                    }
                }
            });
            return results;
        }

        protected string FormatSentence(string raw)
        {
            string[] words = raw.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "i")
                {
                    words[i] = "you";
                }
                else if (words[i] == "my")
                {
                    words[i] = "your";
                }
                else if (words[i] == "am")
                {
                    words[i] = "are";
                }
                else if (words[i] == "i'm")
                {
                    words[i] = "you're";
                }
                else if (words[i] == "we")
                {
                    words[i] = "you";
                }
                else if (words[i] == "we're")
                {
                    words[i] = "you're";
                }
            }
            return String.Join(" ", words);
        }

        protected void AddKeywords(string uid, Memo memo, string[] keywords)
        {
            List<int> keywordIds = new List<int>();
            foreach (string keyword in keywords)
            {
                if (Dictionary.ContainsKey(keyword))
                {
                    int kid = Dictionary[keyword].Id;
                    keywordIds.Add(kid);
                }
            }

            memo.KeywordIds = keywordIds.ToArray();
            new MemoKeywordRelationProvider(this.ConnectionString).Add(uid, memo.Id, memo.KeywordIds);
        }
    }
}
