using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridMvc.Tests.Utility
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
            QueryBuilder qb = new QueryBuilder();
            qb.Add("key1", "value1");
            qb.Add("key2", "value2");
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.QueryString = qb.ToQueryString();

            _builder = new CustomQueryStringBuilder(GridExtensions.Convert(request.Query));

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
            QueryBuilder qb = new QueryBuilder();
            qb.Add("key1", "value1");
            qb.Add("key2", "value2");
            qb.Add("key3", "value3");
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.QueryString = qb.ToQueryString();

            _builder = new CustomQueryStringBuilder(GridExtensions.Convert(request.Query));

            var str1 = _builder.GetQueryStringWithParameter("key4", "value4");
            Assert.AreEqual(str1, "?key1=value1&key2=value2&key3=value3&key4=value4");

            str1 = _builder.GetQueryStringWithParameter("key4", "value4new");
            Assert.AreEqual(str1, "?key1=value1&key2=value2&key3=value3&key4=value4new");
        }
    }
}