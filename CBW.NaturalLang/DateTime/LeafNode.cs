using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class TimeEvalLeafNode : ITimeEvalNode
    {
        public string Term { get; set; }
        public Func<DateTime, DateTime, DateTimeOperation, DateTime> Evaluate { get; set; }
        public double Quantity { get; set; }
        public DateTime GetCurrentValue(DateTime now)
        {
            return this.Evaluate(now, now, DateTimeOperation.Additive);
        }

        public TimeEvalLeafNode(string term = null, int quantity = 1)
        {
            this.Term = term;
            this.Quantity = quantity;
        }

        public TimeEvalLeafNode(Func<DateTime, DateTime, DateTimeOperation, DateTime> evaluate, string term = null, int quantity = 1)
            : this(term, quantity)
        {
            this.Evaluate = evaluate;
        }
    }

    public class YearOfHistoryLeafNode : TimeEvalLeafNode
    {
        public int Year { get; set; }
        public YearOfHistoryLeafNode(int year)
            : base()
        {
            this.Year = year;

            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
            {
                return dt.AddYears(this.Year).AddYears(-dt.Year);
            };
        }
    }

    public class YearLeaflNode : TimeEvalLeafNode
    {
        public YearLeaflNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
                {
                    if (op == DateTimeOperation.Zero)
                    {
                        return new DateTime(dt.Year, 1, 1);
                    }
                    if (Math.Floor(this.Quantity) == this.Quantity)
                    {
                        return dt.AddYears((int)this.Quantity * (int)op);
                    }
                    else
                    {
                        return dt.AddMonths((int)(this.Quantity * 12) * (int)op);
                    }
                };
        }
    }

    public class MonthOfYearLeafNode : TimeEvalLeafNode
    {
        public MonthOfYear Month { get; set; }

        public MonthOfYearLeafNode(MonthOfYear month, int quantity = 1)
            : base(month.ToString().ToLowerInvariant(), quantity)
        {
            this.Month = month;
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
            {
                return dt.AddMonths(-dt.Month + 1).AddMonths((int)this.Month);
                //.AddYears((int)this.Quantity * (int)op);
            };
        }
    }

    public class MonthLeafNode : TimeEvalLeafNode
    {
        public MonthLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
            {
                if (op == DateTimeOperation.Zero)
                {
                    return new DateTime(dt.Year, dt.Month, 1);
                }
                if (Math.Floor(this.Quantity) == this.Quantity)
                {
                    return dt.AddMonths((int)this.Quantity * (int)op);
                }
                else
                {
                    return dt.AddDays((int)(this.Quantity * 30) * (int)op);
                }
            };
        }
    }

    public class DayOfMonthLeafNode : TimeEvalLeafNode
    {
        public int Day { get; set; }
        public DayOfMonthLeafNode(int day)
            : base()
        {
            this.Day = day;

            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
            {
                return dt.AddDays(this.Day).AddDays(-dt.Day);
            };
        }
    }

    public class DayOfWeekLeafNode : TimeEvalLeafNode
    {
        public DayOfWeek Day { get; set; }

        public DayOfWeekLeafNode(DayOfWeek day, string term = null, int quantity = 1)
            : base(term, quantity)
        {
            this.Day = day;
            this.Evaluate =
                (DateTime now, DateTime dt, DateTimeOperation op)
                => FindNearestDayOfWeek(dt, (int)(this.Quantity * (int)op));
        }

        public DateTime FindNearestDayOfWeek(DateTime startingDay, int quantity)
        {
            return startingDay.AddDays(this.Day - startingDay.DayOfWeek).AddDays(quantity * 7);
        }
    }

    public class WeekLeafNode : TimeEvalLeafNode
    {
        public WeekLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                   =>
            {
                if (op == DateTimeOperation.Zero)
                {
                    return dt.Date.AddDays(DayOfWeek.Sunday - dt.DayOfWeek);
                }
                else
                {
                    return dt.AddDays((int)(this.Quantity * 7) * (int)op);
                }
            };
        }
    }

    public class TodayLeafNode : TimeEvalLeafNode
    {
        public int Offset { get; set; }

        public TodayLeafNode(string term, int offset)
            : base(term, 0)
        {
            this.Offset = offset;
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op) =>
            {
                return now.Date.AddDays(this.Offset).AddHours(dt.Hour).AddMinutes(dt.Minute).AddSeconds(dt.Second);
            };
        }
    }

    public class DayLeafNode : TimeEvalLeafNode
    {
        public DayLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                   =>
            {
                if (op == DateTimeOperation.Zero)
                {
                    return dt.Date;
                }
                else
                {
                    return dt.AddDays((int)(this.Quantity * (int)op));
                }
            };
        }
    }

    public class DayTimeLeafNode : TimeEvalLeafNode
    {
        public bool IsAM { get; set; }
        public DayTimeLeafNode(bool am)
            : base()
        {
            this.IsAM = am;
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
                {
                    if (this.IsAM)
                    {
                        if (dt.Hour > 12)
                        {
                            return dt.AddHours(-12);
                        }
                    }
                    else
                    {
                        if (dt.Hour < 12)
                        {
                            return dt.AddHours(12);
                        }
                    }
                    return dt;
                };
        }
    }

    public class HourOfDayLeafNode : TimeEvalLeafNode
    {
        public int Hour { get; set; }
        public HourOfDayLeafNode(int hour)
            : base()
        {
            this.Hour = hour;
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
                {
                    return dt.AddHours(-dt.Hour).AddHours(this.Hour);
                };
        }
        public override string ToString()
        {
            return this.Hour.ToString();
        }
    }

    public class HourLeafNode : TimeEvalLeafNode
    {
        public HourLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                   =>
            {
                if (op == DateTimeOperation.Zero)
                {
                    return dt.Date.AddHours(dt.Hour);
                }
                else
                {
                    return dt.AddHours(this.Quantity * (int)op);
                }
            };
        }
    }

    public class MinuteOfHourLeafNode : TimeEvalLeafNode
    {
        public int Minute { get; set; }
        public MinuteOfHourLeafNode(int minute)
            : base()
        {
            this.Minute = minute;
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                =>
                {
                    return dt.AddMinutes(-dt.Minute).AddMinutes(this.Minute).AddSeconds(-dt.Second);
                };
        }
        public override string ToString()
        {
            return this.Minute.ToString();
        }
    }

    public class MinuteLeafNode : TimeEvalLeafNode
    {
        public MinuteLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                   =>
            {
                if (op == DateTimeOperation.Zero)
                {
                    return dt.Date.AddHours(dt.Hour).AddMinutes(dt.Minute);
                }
                else
                {
                    return dt.AddMinutes(this.Quantity * (int)op);
                }
            };
        }
    }

    public class SecondLeafNode : TimeEvalLeafNode
    {
        public SecondLeafNode(string term, int quantity = 1)
            : base(term, quantity)
        {
            this.Evaluate = (DateTime now, DateTime dt, DateTimeOperation op)
                   =>
            {
                return dt.AddSeconds(this.Quantity * (int)op);
            };
        }
    }
}
