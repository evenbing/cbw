using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CBW.NaturalLang;
using System.Collections.Generic;

namespace CBW.NaturalLang.Tests
{
    [TestClass]
    public class DateTimeExtractorTests
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            new NumberTagger();
            new OperatorTagger();
            new DateTimeTagger();
        }

        [TestMethod]
        public void ExtractWord()
        {
            IEnumerable<DateTime> x = DateTimeExtractor.Extract("Tomorrow");
            DateTime dt = x.First();
            DateTime ans = DateTime.Now.AddDays(1);
            Assert.AreEqual(
                new DateTime(ans.Year, ans.Month, ans.Day, ans.Hour, ans.Minute, ans.Second),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ExtractSingle()
        {
            IEnumerable<DateTime> x = DateTimeExtractor.Extract("Wake me up seven thirty tomorrow");
            DateTime dt = x.First();
            Assert.AreEqual(
                DateTime.Now.Date.AddDays(1).AddHours(7).AddMinutes(30),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
        }

        [TestMethod]
        public void ExtractDouble()
        {
            IEnumerable<DateTime> x = DateTimeExtractor.Extract("Wake me up seven thirty tomorrow morning and three o'clock tomorrow afternoon");
            DateTime dt = x.First();
            DateTime dt2 = x.Skip(1).First();
            Assert.AreEqual(
                DateTime.Now.Date.AddDays(1).AddHours(7).AddMinutes(30),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
            Assert.AreEqual(
                DateTime.Now.Date.AddDays(1).AddHours(15),
                new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second));
        }

        [TestMethod]
        public void ExtractDoubleNumeric()
        {
            IEnumerable<DateTime> x = DateTimeExtractor.Extract("Wake me up 7:30 tomorrow morning and 3:00 tomorrow afternoon");
            DateTime dt = x.First();
            DateTime dt2 = x.Skip(1).First();
            Assert.AreEqual(
                DateTime.Now.Date.AddDays(1).AddHours(7).AddMinutes(30),
                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
            Assert.AreEqual(
                DateTime.Now.Date.AddDays(1).AddHours(15),
                new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second));
        }
    }
}
