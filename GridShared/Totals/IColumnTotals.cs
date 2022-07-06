using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridShared.Totals
{
    public interface IColumnTotals<T>
    {
        IList<string> GetNames();
        bool IsNullable();
        Type GetPropertyType(bool isNullable);
        Expression GetExpression(IList<string> names, ParameterExpression parameter);

        #region OData
        string GetFullName();
        #endregion
    }
}