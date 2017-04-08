using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public static class DateTimeParser
    {
        public static bool TryParse(string s, out ITimeEvalNode node)
        {
            string[] tokens = Tokenizer.Tokenize(s);
            TaggedItem[] items = new TagFactory(
                new NumberTagger(),
                new OperatorTagger(),
                new DateTimeTagger(),
                new IgnoreTagger())
                .Tag(tokens);
            TaggedItem[] aggregatedItems = NumberAggregator.Aggregate(items);
            return TryParse(aggregatedItems, out node);
        }

        public static bool TryParse(TaggedItem[] items, out ITimeEvalNode node)
        {
            ITimeEvalNode[] nodes;
            if (!TryScanOnce(items, out nodes))
            {
                node = null;
                return false;
            }
            try
            {
                node = BuildParseTree(nodes);
            }
            catch
            {
                node = null;
                return false;
            }
            return true;
        }

        private static int FindWorkHour(int hour)
        {
            if (hour < 6) return hour + 12;
            return hour;
        }

        private static bool TryScanOnce(IEnumerable<TaggedItem> items, out ITimeEvalNode[] nodes)
        {
            List<TaggedItem> workingSpace = new List<TaggedItem>();
            List<ITimeEvalNode> results = new List<ITimeEvalNode>();
            foreach (TaggedItem item in items)
            {
                if (item is TaggedOperatorItem)
                {
                    if (workingSpace.Count != 0)
                    {
                        ITimeEvalNode[] commitResult = CommitWorkingSpace(workingSpace);
                        if (commitResult != null)
                        {
                            results.AddRange(commitResult);
                            workingSpace.Clear();
                        }
                        else
                        {
                            nodes = null;
                            return false;
                        }
                    }
                    TaggedOperatorItem opItem = item as TaggedOperatorItem;
                    if (opItem.Info.Type == OperatorType.Binary)
                    {
                        results.Add(new BinaryOperatorNode(null, null, opItem.Info));
                    }
                    else if (opItem.Info.Type == OperatorType.Unary)
                    {
                        results.Add(new UnaryOperatorNode(null, opItem.Info));
                    }
                    else
                    {
                        throw new Exception("Operator type not recognized");
                    }
                    continue;
                }
                if (!(item is TaggedNumberItem) && !(item is TaggedDateTimeItem))
                {
                    if (item.Tag == Tag.Ignore)
                    {
                        continue;
                    }
                    else
                    {
                        nodes = null;
                        return false;
                    }
                }
                workingSpace.Add(item);
                if (workingSpace.Count == 2)
                {
                    ITimeEvalNode[] commitResult = CommitWorkingSpace(workingSpace);
                    if (commitResult != null)
                    {
                        results.AddRange(commitResult);
                        workingSpace.Clear();
                    }
                    else
                    {
                        nodes = null;
                        return false;
                    }
                }
            }
            if (workingSpace.Count > 0)
            {
                ITimeEvalNode[] commitResult = CommitWorkingSpace(workingSpace);
                if (commitResult != null)
                {
                    results.AddRange(commitResult);
                    workingSpace.Clear();
                }
                else
                {
                    nodes = null;
                    return false;
                }
            }
            nodes = results.ToArray();
            return true;
        }

        public static ITimeEvalNode[] CommitWorkingSpace(List<TaggedItem> workingSpace)
        {
            List<ITimeEvalNode> results = new List<ITimeEvalNode>();
            ITimeEvalNode node;
            if (TryParseExact(workingSpace.ToArray(), out node))
            {
                results.Add(node);
                return results.ToArray();
            }
            if (workingSpace.Count > 1)
            {
                if (TryParseExact(new TaggedItem[] { workingSpace[0] }, out node))
                {
                    ITimeEvalNode node2;
                    if (TryParseExact(new TaggedItem[] { workingSpace[1] }, out node2))
                    {
                        results.Add(new BinaryOperatorNode(node, node2, DateTimeOperation.Additive));
                        return results.ToArray();
                    }
                }
            }
            return null;
        }

        private static bool TryParseExact(TaggedItem[] items, out ITimeEvalNode node)
        {
            if (items.Length == 0)
            {
                node = new DayLeafNode(null, 0);
                return true;
            }
            else if (items.Length == 1)
            {
                TaggedItem item = items[0];
                if (item is TaggedDateTimeItem)
                {
                    TaggedDateTimeItem dtItem = item as TaggedDateTimeItem;

                    // Second
                    if (dtItem.Type == DateTimeType.Second)
                    {
                        node = new SecondLeafNode(item.Term);
                        return true;
                    }

                    // Minute
                    else if (dtItem.Type == DateTimeType.Minute)
                    {
                        if (dtItem.Term.StartsWith("quarter"))
                        {
                            node = new MinuteLeafNode(item.Term, 15);
                        }
                        else
                        {
                            node = new MinuteLeafNode(item.Term);
                        }
                        return true;
                    }

                    // Hour
                    else if (dtItem.Type == DateTimeType.Hour)
                    {
                        node = new HourLeafNode(item.Term);
                        return true;
                    }

                    // DayTime
                    else if (dtItem.Type == DateTimeType.DayTime)
                    {
                        if (dtItem.Term == "am" || dtItem.Term == "morning")
                        {
                            node = new DayTimeLeafNode(true);
                            return true;
                        }
                        else if (dtItem.Term == "pm" || dtItem.Term == "afternoon" || dtItem.Term == "evening" || dtItem.Term == "night")
                        {
                            node = new DayTimeLeafNode(false);
                            return true;
                        }
                        node = null;
                        return false;
                    }
                    // Today
                    else if (dtItem.Type == DateTimeType.Today)
                    {
                        if (dtItem.Term == "today")
                        {
                            node = new TodayLeafNode(item.Term, 0);
                        }
                        else if (dtItem.Term == "tomorrow")
                        {
                            node = new TodayLeafNode(item.Term, 1);
                        }
                        else if (dtItem.Term == "yesterday")
                        {
                            node = new TodayLeafNode(item.Term, -1);
                        }
                        else
                        {
                            node = null;
                            return false;
                        }
                        return true;
                    }
                    // Day
                    else if (dtItem.Type == DateTimeType.Day)
                    {
                        node = new DayLeafNode(null, 1);
                        return true;
                    }
                    // Day of Week
                    else if (dtItem.Type == DateTimeType.DayOfWeek)
                    {
                        if (dtItem.Term == "monday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Monday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "tuesday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Tuesday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "wednesday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Wednesday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "thursday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Thursday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "friday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Friday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "saturday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Saturday, item.Term);
                            return true;
                        }
                        else if (dtItem.Term == "sunday")
                        {
                            node = new DayOfWeekLeafNode(DayOfWeek.Sunday, item.Term);
                            return true;
                        }
                        node = null;
                        return false;
                    }
                    // Week
                    else if (dtItem.Type == DateTimeType.Week)
                    {
                        if (dtItem.Term == "fortnight")
                        {
                            node = new WeekLeafNode(item.Term, 2);
                        }
                        else
                        {
                            node = new WeekLeafNode(item.Term);
                        }
                        return true;
                    }
                    // Month
                    else if (dtItem.Type == DateTimeType.Month)
                    {
                        node = new MonthLeafNode(item.Term, 1);
                        return true;
                    }
                    // Month of Year
                    else if (dtItem.Type == DateTimeType.MonthOfYear)
                    {
                        if (dtItem.Term == "january")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.January);
                        }
                        else if (dtItem.Term == "feburary")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.Feburary);
                        }
                        else if (dtItem.Term == "march")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.March);
                        }
                        else if (dtItem.Term == "april")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.April);
                        }
                        else if (dtItem.Term == "may")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.May);
                        }
                        else if (dtItem.Term == "june")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.June);
                        }
                        else if (dtItem.Term == "july")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.July);
                        }
                        else if (dtItem.Term == "august")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.August);
                        }
                        else if (dtItem.Term == "september")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.September);
                        }
                        else if (dtItem.Term == "october")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.October);
                        }
                        else if (dtItem.Term == "november")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.November);
                        }
                        else if (dtItem.Term == "december")
                        {
                            node = new MonthOfYearLeafNode(MonthOfYear.December);
                        }
                        else
                        {
                            node = null;
                            return false;
                        }
                        return true;
                    }
                    // Year
                    else if (dtItem.Type == DateTimeType.Year)
                    {
                        if (dtItem.Term.StartsWith("centur"))
                        {
                            node = new YearLeaflNode(item.Term, 100);
                        }
                        else
                        {
                            node = new YearLeaflNode(item.Term);
                        }
                        return true;
                    }
                    else
                    {
                        node = null;
                        return false;
                    }
                }
                else if (item is TaggedNumberItem)
                {
                    TaggedNumberItem numItem = item as TaggedNumberItem;

                    // Parse hour (e.g. at four)
                    if (numItem.Number >= 1 && numItem.Number < 12 && numItem.Tag == Tag.Number && numItem.Term != "a" && numItem.Term != "an")
                    {
                        node = new BinaryOperatorNode(
                            new HourOfDayLeafNode(FindWorkHour((int)numItem.Number)),
                            new MinuteOfHourLeafNode(0),
                            DateTimeOperation.Additive);
                        return true;
                    }

                    // Parse day of month
                    if (numItem.Number > 0 && numItem.Number <= 31 && numItem.Tag == Tag.NumberOrdinal)
                    {
                        node = new DayOfMonthLeafNode((int)numItem.Number);
                        return true;
                    }

                    // Parse year (e.g. twenty twenty)
                    else if (numItem.Number > 1100 && numItem.Number < 2100 && numItem.Tag == Tag.Number)
                    {
                        node = new YearOfHistoryLeafNode((int)numItem.Number);
                        return true;
                    }
                }
                node = null;
                return false;
            }
            else if (items.Length == 2)
            {
                if (items[0] is TaggedNumberItem && items[1] is TaggedDateTimeItem)
                {
                    ITimeEvalNode dt;
                    TaggedNumberItem numItem = items[0] as TaggedNumberItem;
                    if ((items[1] as TaggedDateTimeItem).Type == DateTimeType.Today)
                    {
                        node = null;
                        return false;
                    }
                    if (items[0].Tag == Tag.Number &&
                                numItem.Number <= 12 &&
                                (items[1] as TaggedDateTimeItem).Type == DateTimeType.HourOfDay)
                    {
                        node = new BinaryOperatorNode(
                            new HourOfDayLeafNode(FindWorkHour((int)numItem.Number)),
                            new MinuteOfHourLeafNode(0),
                            DateTimeOperation.Additive);
                        return true;
                    }
                    if (TryParseExact(new TaggedItem[] { items[1] }, out dt))
                    {
                        if (dt is TimeEvalLeafNode)
                        {
                            TimeEvalLeafNode dtTimespan = dt as TimeEvalLeafNode;

                            // 4 pm
                            if (items[0].Tag == Tag.Number &&
                                numItem.Number <= 12 &&
                                (items[1] as TaggedDateTimeItem).Type == DateTimeType.DayTime)
                            {
                                node = new BinaryOperatorNode(
                                    new BinaryOperatorNode(
                                    new HourOfDayLeafNode((int)numItem.Number),
                                    new MinuteOfHourLeafNode(0),
                                    DateTimeOperation.Additive),
                                    dtTimespan,
                                    DateTimeOperation.Additive);
                            }
                            else if (numItem.Number > 1 
                                && !items[1].Term.EndsWith("s") 
                                && (items[1] as TaggedDateTimeItem).Type != DateTimeType.HourOfDay)
                            {
                                node = null;
                                return false;
                            }
                            // 4 days
                            else
                            {
                                dtTimespan.Quantity *= numItem.Number;
                                node = dt;
                            }
                            return true;
                        }
                    }
                    node = null;
                    return false;
                }

                // July 4th
                else if (items[0] is TaggedDateTimeItem && items[1] is TaggedNumberItem)
                {
                    TaggedNumberItem numItem = items[1] as TaggedNumberItem;
                    if (numItem.Tag == Tag.NumberOrdinal && numItem.Number < 31)
                    {
                        ITimeEvalNode dt;
                        if (TryParseExact(new TaggedItem[] { items[0] }, out dt))
                        {
                            if (dt is MonthOfYearLeafNode)
                            {
                                node = new BinaryOperatorNode(
                                    dt,
                                    new DayOfMonthLeafNode((int)numItem.Number),
                                    DateTimeOperation.Additive);
                                return true;
                            }
                        }
                    }
                    node = null;
                    return false;
                }

                // Four thirty
                else if (items[0] is TaggedNumberItem && items[1] is TaggedNumberItem)
                {
                    if (items[0].Tag == Tag.Number && items[1].Tag == Tag.Number)
                    {
                        TaggedNumberItem num1 = items[0] as TaggedNumberItem;
                        TaggedNumberItem num2 = items[1] as TaggedNumberItem;
                        if (num1.Number <= 24 && num2.Number < 60)
                        {
                            node = new BinaryOperatorNode(
                                new HourOfDayLeafNode(
                                    FindWorkHour((int)num1.Number)),
                                new MinuteOfHourLeafNode(
                                    (int)num2.Number),
                                    DateTimeOperation.Additive);
                            return true;
                        }
                    }
                    node = null;
                    return false;
                }
                node = null;
                return false;
            }
            node = null;
            return false;
        }

        private static ITimeEvalNode BuildParseTree(ITimeEvalNode[] nodes)
        {
            if (nodes.Length == 1)
            {
                return nodes[0];
            }
            Dictionary<int, int> operatorTable = new Dictionary<int, int>();
            List<ITimeEvalNode> results = new List<ITimeEvalNode>();
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] is OperatorNode)
                {
                    if (nodes[i] is BinaryOperatorNode)
                    {
                        BinaryOperatorNode binNode = nodes[i] as BinaryOperatorNode;
                        if (binNode.LeftChild == null && binNode.RightChild == null)
                        {
                            operatorTable.Add(i, binNode.Info.Priority);
                        }
                    }
                    else if (nodes[i] is UnaryOperatorNode)
                    {
                        UnaryOperatorNode uNode = nodes[i] as UnaryOperatorNode;
                        if (uNode.Child == null)
                        {
                            operatorTable.Add(i, uNode.Info.Priority);
                        }
                    }
                }
            }
            if (operatorTable.Count == 0)
            {
                results.Add(new BinaryOperatorNode(nodes[0], nodes[1], DateTimeOperation.Additive));
                for (int i = 2; i < nodes.Length; i++)
                {
                    ITimeEvalNode node1 = results[0];
                    results.Clear();
                    results.Add(new BinaryOperatorNode(node1, nodes[i], DateTimeOperation.Additive));
                }
                return BuildParseTree(results.ToArray());
            }
            else
            {
                // Just evaluate the node with maximum priority
                int maxOpIndex = operatorTable.OrderByDescending(x => x.Value).First().Key;
                ITimeEvalNode _opNode = nodes[maxOpIndex];
                if (_opNode is BinaryOperatorNode)
                {
                    BinaryOperatorNode binNode = _opNode as BinaryOperatorNode;
                    ITimeEvalNode node1 = null, node2 = null;
                    ITimeEvalNode[] result = null;
                    if (maxOpIndex == 0)
                    {
                        node1 = new DayLeafNode(null, 0);
                        node2 = nodes[maxOpIndex + 1];
                        result = new ITimeEvalNode[nodes.Length - 1];
                        if (binNode.Info.Direction == OperatorReadDirection.Left)
                        {
                            result[0] = new BinaryOperatorNode(node1, node2, binNode.Info);
                        }
                        else
                        {
                            result[0] = new BinaryOperatorNode(node2, node1, binNode.Info);
                        }
                    }
                    else
                    {
                        node1 = nodes[maxOpIndex - 1];
                    }

                    if (maxOpIndex == nodes.Length - 1)
                    {
                        node1 = nodes[maxOpIndex - 1];
                        node2 = new DayLeafNode(null, 0);
                        result = new ITimeEvalNode[nodes.Length - 1];
                        if (binNode.Info.Direction == OperatorReadDirection.Left)
                        {
                            result[result.Length - 1] = new BinaryOperatorNode(node1, node2, binNode.Info);
                        }
                        else
                        {
                            result[result.Length - 1] = new BinaryOperatorNode(node2, node1, binNode.Info);
                        }
                    }
                    else
                    {
                        node2 = nodes[maxOpIndex + 1];
                    }

                    if (result == null)
                    {
                        result = new ITimeEvalNode[nodes.Length - 2];
                        if (binNode.Info.Direction == OperatorReadDirection.Left)
                        {
                            result[maxOpIndex - 1] = new BinaryOperatorNode(node1, node2, binNode.Info);
                        }
                        else
                        {
                            result[maxOpIndex - 1] = new BinaryOperatorNode(node2, node1, binNode.Info);
                        }
                    }

                    if (maxOpIndex > 1)
                    {
                        Array.Copy(nodes, 0, result, 0, maxOpIndex - 1);
                    }
                    if (nodes.Length > maxOpIndex + 2)
                    {
                        Array.Copy(nodes, maxOpIndex + 2, result, maxOpIndex, nodes.Length - maxOpIndex - 2);
                    }

                    return BuildParseTree(result);
                }
                else
                {
                    UnaryOperatorNode uNode = _opNode as UnaryOperatorNode;
                    ITimeEvalNode[] result = new ITimeEvalNode[nodes.Length - 1];
                    if (uNode.Info.Direction == OperatorReadDirection.Left)
                    {
                        if (maxOpIndex == nodes.Length - 1)
                        {
                            throw new Exception("Unary operator out of boundary");
                        }
                        if (maxOpIndex > 0)
                        {
                            Array.Copy(nodes, result, maxOpIndex);
                        }
                        if (nodes.Length - maxOpIndex > 2)
                        {
                            Array.Copy(nodes, maxOpIndex + 2, result, maxOpIndex + 1, nodes.Length - maxOpIndex - 2);
                        }
                        result[maxOpIndex] = new UnaryOperatorNode(nodes[maxOpIndex + 1], uNode.Info);
                    }
                    else
                    {
                        if (maxOpIndex == 0)
                        {
                            throw new Exception("Unary operator out of boundary");
                        }
                        if (maxOpIndex > 1)
                        {
                            Array.Copy(nodes, result, maxOpIndex - 1);
                        }
                        if (nodes.Length - maxOpIndex > 1)
                        {
                            Array.Copy(nodes, maxOpIndex + 1, result, maxOpIndex, nodes.Length - maxOpIndex - 1);
                        }
                        result[maxOpIndex - 1] = new UnaryOperatorNode(nodes[maxOpIndex - 1], uNode.Info);
                    }
                    return BuildParseTree(result);
                }
            }
            return null;
        }
    }
}
