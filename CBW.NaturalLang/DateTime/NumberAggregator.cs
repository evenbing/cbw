using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public static class NumberAggregator
    {
        public static TaggedItem[] Aggregate(IEnumerable<TaggedItem> items)
        {
            List<TaggedItem> aggregateList = new List<TaggedItem>();
            List<TaggedNumberItem> aggregateNumber = new List<TaggedNumberItem>();

            int i = 0;
            bool isNum = false;
            TaggedItem andTerm = null;
            foreach (TaggedItem item in items)
            {
                if (item is TaggedNumberItem)
                {
                    TaggedNumberItem aggregateItem;
                    TaggedNumberItem toAdd = item as TaggedNumberItem;
                    aggregateNumber.Add(toAdd);
                    if (!TryAggregateOnce(aggregateNumber, out aggregateItem))
                    {
                        aggregateNumber.Remove(toAdd);
                        TryAggregateOnce(aggregateNumber, out aggregateItem);
                        aggregateList.Add(aggregateItem);
                        aggregateNumber.Clear();
                        aggregateNumber.Add(toAdd);
                    }
                    isNum = true;
                    andTerm = null;
                }
                else
                {
                    if (andTerm == null)
                    {
                        if (aggregateNumber.Count > 0 && isNum)
                        {
                            if (item.Term == "and")
                            {
                                andTerm = item;
                                continue;
                            }
                            else
                            {
                                // Submit the deck
                                TaggedNumberItem aggregateItem;
                                if (TryAggregateOnce(aggregateNumber, out aggregateItem))
                                {
                                    aggregateList.Add(aggregateItem);
                                }
                                aggregateNumber.Clear();
                            }
                        }
                    }
                    else
                    {
                        aggregateList.Add(andTerm);
                        andTerm = null;
                    }
                    isNum = false;
                    aggregateList.Add(item);
                }
            }
            if (aggregateNumber.Count > 0)
            {
                TaggedNumberItem aggregateItem;
                if (TryAggregateOnce(aggregateNumber, out aggregateItem))
                {
                    aggregateList.Add(aggregateItem);
                }
                aggregateNumber.Clear();
            }
            return aggregateList.ToArray();
        }

        private static bool TryAggregateOnce(IEnumerable<TaggedNumberItem> items, out TaggedNumberItem result)
        {
            double aggregated = 0;
            foreach (TaggedNumberItem item in items)
            {
                if (aggregated == 0.5)
                {
                    if (item.Number == 1 && items.Count() == 2)
                    {
                        result = new TaggedNumberItem()
                        {
                            Term = "half",
                            Number = .5,
                            Tag = Tag.Number
                        };
                        return true;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }
                if (Math.Floor(item.Number) != item.Number)
                {
                    aggregated = 0.5;
                    continue;
                }
                if (aggregated == 0)
                {
                    aggregated += item.Number;
                    continue;
                }
                if (aggregated < item.Number)
                {
                    if (item.Number % 100 == 0)
                    {
                        aggregated *= item.Number;
                    }
                    else
                    {
                        if (aggregated > 13)
                        {
                            aggregated *= 100;
                            aggregated += item.Number;
                        }
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
                }
                else
                {
                    if (Math.Floor(Math.Log10(aggregated)) - Math.Floor(Math.Log10(item.Number)) >= 1)
                    {
                        aggregated += item.Number;
                    }
                    else
                    {
                        if (aggregated > 13)
                        {
                            aggregated *= 100;
                            aggregated += item.Number;
                        }
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
                }
            }
            result = new TaggedNumberItem()
            {
                Number = aggregated,
                Tag = items.Last().Tag,
                Term = String.Join(" ", items.Select(x => x.Term))
            };
            return true;
        }
    }
}
