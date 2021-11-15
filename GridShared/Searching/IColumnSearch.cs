using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Searching
{
    public interface IColumnSearch<T>
    {
        Expression GetExpression(string value, bool onlyTextColumns, ParameterExpression parameter, 
            MethodInfo removeDiacritics = null);
    }
}