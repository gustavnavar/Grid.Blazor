using GridShared.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridBlazor.Tests.Utility
{
    [TestClass]
    public class PropertiesHelperTests
    {


        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void TestColumnNameBuilding()
        {
            Expression<Func<TestModel, DateTime>> expr = m => m.Created;

            string name = PropertiesHelper.BuildColumnNameFromMemberExpression((MemberExpression)expr.Body);
            Assert.AreEqual(name, "Created");

        }

        [TestMethod]
        public void TestColumnNameBuildingChilds()
        {
            Expression<Func<TestModel, string>> expr = m => m.Child.ChildTitle;
            string name = PropertiesHelper.BuildColumnNameFromMemberExpression((MemberExpression)expr.Body);
            Assert.AreEqual(name, "Child.ChildTitle");

        }

        [TestMethod]
        public void TestGetPropertyFromColumnName()
        {
            const string columnName = "Created";
            IEnumerable<PropertyInfo> sequence;
            var pi = PropertiesHelper.GetPropertyFromColumnName(columnName, typeof(TestModel), out sequence);
            Assert.AreEqual(sequence.Count(), 1);
            Assert.AreEqual(pi.Name, "Created");
        }

        [TestMethod]
        public void TestGetPropertyFromColumnNameChilds()
        {
            const string columnName = "Child.ChildTitle";
            IEnumerable<PropertyInfo> sequence;
            var pi = PropertiesHelper.GetPropertyFromColumnName(columnName, typeof(TestModel), out sequence);
            Assert.AreEqual(sequence.Count(), 2);
            Assert.AreEqual(pi.Name, "ChildTitle");
        }



    }
}