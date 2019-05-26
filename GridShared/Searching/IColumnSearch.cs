using System.Linq.Expressions;

namespace GridShared.Searching
{
    public interface IColumnSearch<T>
    {
        Expression GetExpression(string value, bool onlyTextColumns, ParameterExpression parameter);
    }
}