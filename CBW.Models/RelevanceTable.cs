using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.Models
{
    [Serializable]
    public class RelevanceTable
    {
        public Dictionary<int, Dictionary<int, int>> Table { get; set; }

        public RelevanceTable()
        {
            this.Table = new Dictionary<int, Dictionary<int, int>>();
        }

        public void Add(int wordA, int wordB)
        {
            this.Add(wordA, wordB, 1);
        }

        public void Add(int wordA, int wordB, int relevance)
        {
            Dictionary<int, int> wordAList, wordBList;
            if (!Table.TryGetValue(wordA, out wordAList))
            {
                wordAList = new Dictionary<int, int>();
                Table.Add(wordA, wordAList);
            }
            if (!Table.TryGetValue(wordB, out wordBList))
            {
                wordBList = new Dictionary<int, int>();
                Table.Add(wordB, wordBList);
            }

            if (!wordAList.ContainsKey(wordB))
            {
                wordAList.Add(wordB, relevance);
            }
            else
            {
                wordAList[wordB] = wordAList[wordB] + relevance;
            }
            if (!wordBList.ContainsKey(wordA))
            {
                wordBList.Add(wordA, relevance);
            }
            else
            {
                wordBList[wordA] = wordBList[wordA] + relevance;
            }
        }

        public void Remove(int wordA, int wordB)
        {
            Dictionary<int, int> wordAList, wordBList;
            if (!Table.TryGetValue(wordA, out wordAList))
            {
                return;
            }
            if (!Table.TryGetValue(wordB, out wordBList))
            {
                return;
            }

            wordAList.Remove(wordB);
            wordBList.Remove(wordA);
        }

        public int GetRelevance(int wordA, int wordB)
        {
            Dictionary<int, int> wordAList;

            if (!Table.TryGetValue(wordA, out wordAList))
            {
                return 0;
            }
            else
            {
                int rel;
                if (wordAList.TryGetValue(wordB, out rel))
                {
                    return rel;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
