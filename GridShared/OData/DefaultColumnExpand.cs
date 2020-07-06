using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridShared.OData
{
    public class DefaultColumnExpand<T, TData> : IColumnExpand<T>
    {
        private readonly Expression<Func<T, TData>> _expression;

        public DefaultColumnExpand(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
        }

        public string GetName()
        {
            try
            {
                List<string> names = new List<string>();
                Expression expression = _expression.Body;
                while (expression.NodeType != ExpressionType.Parameter)
                {
                    names.Add(((MemberExpression)expression).Member.Name);
                    expression = ((MemberExpression)expression).Expression;
                }

                string result = "";
                for (int i = 1 ; i <= names.Count - 1; i++)
                {
                    result = names[i] + result;
                    if (i != names.Count - 1)
                        result = "($expand=" + result + ")";
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
