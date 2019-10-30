using System;

namespace GridBlazor.Tests
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public TestModelChild Child { get; set; }

        public TestModelChild[] List { get; set; }


        public Int16 Int16Field { get; set; }
        public UInt16 UInt16Field { get; set; }
        public UInt32 UInt32Field { get; set; }
        public UInt64 UInt64Field { get; set; }

        public override bool Equals(object obj)
        {
            var compareObject = obj as TestModel;
            if (compareObject == null) return false;

            return compareObject.Created == Created
                   && compareObject.Id == Id
                   && compareObject.Title == Title
                   && compareObject.Child.ChildCreated == Child.ChildCreated
                   && compareObject.Child.ChildTitle == Child.ChildTitle
                   && compareObject.Int16Field == Int16Field
                   && compareObject.UInt16Field == UInt16Field
                   && compareObject.UInt32Field == UInt32Field
                   && compareObject.UInt64Field == UInt64Field;
        }

        public override int GetHashCode()
        {
            return  new { Id, Title, Child.ChildCreated, Child.ChildTitle, Int16Field, UInt16Field, UInt32Field, UInt64Field }.GetHashCode();
        }
    } 

    public class TestModelChild
    {
        public string ChildTitle { get; set; }
        public DateTime ChildCreated { get; set; }
    }
}