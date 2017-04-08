using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CBW.NaturalLang;
using System.Collections.Generic;
using System.Linq;

namespace CBW.NaturalLang.Tests
{
    [TestClass]
    public class DateTimeParserTests
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            new NumberTagger();
            new OperatorTagger();
            new DateTimeTagger();
        }

        [TestMethod]
        public void ParseComplexYearDateTime()
        {
            DateTime dt = Parse("Two forty PM on 4th of July Twenty Twelve");
            Assert.AreEqual(
                new DateTime(2012, 7, 4, 14, 40, 0),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseDayOfWeekSimpleHour()
        {
            DateTime dt = Parse("Three in the morning next Friday");
            Assert.AreEqual(
                DateTime.Now.AddDays(5 - (int)(DateTime.Now.DayOfWeek)).AddDays(7).Date.AddHours(3),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseUnarySubstractive()
        {
            DateTime dt = Parse("Three hours ago");
            DateTime answer = (DateTime.Now - new TimeSpan(3, 0, 0));
            Assert.AreEqual(
                answer.Date.AddHours(answer.Hour).AddMinutes(answer.Minute).AddSeconds(answer.Second),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseHourOfDay()
        {
            DateTime dt = Parse("Three o'clock");
            Assert.AreEqual(
                DateTime.Now.Date.AddHours(15),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseBinarySubstractive()
        {
            DateTime dt = Parse("A quarter to eight");
            Assert.AreEqual(
                DateTime.Now.Date.AddHours(8).AddMinutes(-15),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseFloatPointNumberAggregation()
        {
            DateTime dt = Parse("Half an hour later");
            DateTime answer = (DateTime.Now + new TimeSpan(0, 30, 0));
            Assert.AreEqual(
                answer.Date.AddHours(answer.Hour).AddMinutes(answer.Minute).AddSeconds(answer.Second),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseFloatPointNumberAggregationSubstractive()
        {
            DateTime dt = Parse("Half an hour ago");
            DateTime answer = (DateTime.Now - new TimeSpan(0, 30, 0));
            Assert.AreEqual(
                answer.Date.AddHours(answer.Hour).AddMinutes(answer.Minute).AddSeconds(answer.Second),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ParseFloatPointNumberAggregationMinute()
        {
            DateTime dt = Parse("Half a minute earlier");
            DateTime answer = (DateTime.Now - new TimeSpan(0, 0, 30));
            Assert.AreEqual(
                answer.Date.AddHours(answer.Hour).AddMinutes(answer.Minute).AddSeconds(answer.Second),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        private static DateTime Parse(string source)
        {
            ITimeEvalNode node;
            DateTimeParser.TryParse(source, out node);
            return node.GetCurrentValue(DateTime.Now);
        }
    }
}
