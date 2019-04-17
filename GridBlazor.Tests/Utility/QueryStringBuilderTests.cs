using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridBlazor.Tests.Utility
{
    [TestClass]
    public class QueryStringBuilderTests
    {
        private CustomQueryStringBuilder _builder;

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void TestExcept()
        {
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();
            query.Add("key1", "value1");
            query.Add("key2", "value2");

            _builder = new CustomQueryStringBuilder(query);

            var str1 = _builder.GetQueryStringExcept(new[] { "key1" });
            Assert.AreEqual(str1, "?key2=value2");

            str1 = _builder.GetQueryStringExcept(new[] { "key2" });
            Assert.AreEqual(str1, "?key1=value1");

            str1 = _builder.GetQueryStringExcept(new[] { "key1", "key2" });
            Assert.AreEqual(str1, string.Empty);
        }

        [TestMethod]
        public void TestWithParameter()
        {
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();
            query.Add("key1", "value1");
            query.Add("key2", "value2");
            query.Add("key3", "value3");

            _builder = new CustomQueryStringBuilder(query);

            var str1 = _builder.GetQueryStringWithParameter("key4", "value4");
            Assert.AreEqual(str1, "?key1=value1&key2=value2&key3=value3&key4=value4");

            str1 = _builder.GetQueryStringWithParameter("key4", "value4new");
            Assert.AreEqual(str1, "?key1=value1&key2=value2&key3=value3&key4=value4new");
        }
    }
}