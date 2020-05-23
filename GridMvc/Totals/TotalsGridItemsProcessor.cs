using GridMvc.Columns;
using GridShared.Columns;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GridMvc.Totals
{
    /// <summary>
    ///     Settings grid items, based on current searching settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TotalsGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly ISGrid _grid;

        public TotalsGridItemsProcessor(ISGrid grid)
        {
            _grid = grid;
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            foreach (IGridColumn<T> gridColumn in _grid.Columns)
            {
                if (gridColumn == null || gridColumn.Totals == null)
                {
                    gridColumn.IsSumEnabled = false;
                    gridColumn.IsAverageEnabled = false;
                    gridColumn.IsMaxEnabled = false;
                    gridColumn.IsMinEnabled = false;
                    continue;
                }

                bool isNullable = gridColumn.Totals.IsNullable();
                Type type = gridColumn.Totals.GetPropertyType(isNullable);

                var names = gridColumn.Totals.GetNames();
                if (names == null)
                {
                    gridColumn.IsSumEnabled = false;
                    gridColumn.IsAverageEnabled = false;
                    gridColumn.IsMaxEnabled = false;
                    gridColumn.IsMinEnabled = false;
                    continue;
                }
                var expression = gridColumn.Totals.GetExpression(names, parameter);
                if (expression == null)
                {
                    gridColumn.IsSumEnabled = false;
                    gridColumn.IsAverageEnabled = false;
                    gridColumn.IsMaxEnabled = false;
                    gridColumn.IsMinEnabled = false;
                    continue;
                }

                if (isNullable)
                {
                    if (type == typeof(Single))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T,Nullable<Single>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Int32))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Int32>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Int64))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Int64>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Double))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Double>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Decimal>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<DateTime>>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, string>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;
                        gridColumn.IsMaxEnabled = false;
                        gridColumn.IsMinEnabled = false;
                    }
                }
                else
                {
                    if (type == typeof(Single))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Single>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Int32))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Int32>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Int64))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Int64>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Double))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Double>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Decimal>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            ((ISGridColumn)gridColumn).SumValue = (decimal?)items.Sum(lambdaExpression);
                            gridColumn.SumString = getString(((ISGridColumn)gridColumn).SumValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            ((ISGridColumn)gridColumn).AverageValue = (decimal?)items.Average(lambdaExpression);
                            gridColumn.AverageString = getString(((ISGridColumn)gridColumn).AverageValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, DateTime>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, string>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            ((ISGridColumn)gridColumn).MaxValue = items.Max(lambdaExpression);
                            gridColumn.MaxString = getString(((ISGridColumn)gridColumn).MaxValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            ((ISGridColumn)gridColumn).MinValue = items.Min(lambdaExpression);
                            gridColumn.MinString = getString(((ISGridColumn)gridColumn).MinValue, ((ISGridColumn)gridColumn).ValuePattern);
                        }
                    }
                    else
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;
                        gridColumn.IsMaxEnabled = false;
                        gridColumn.IsMinEnabled = false;
                    }
                }
            }

            return items;         
        }

        #endregion

        private string getString(object value, string valuePattern)
        {
            if (value == null)
                return null;
            try
            {
                if (!string.IsNullOrEmpty(valuePattern))
                    return string.Format(valuePattern, value);
                else
                    return value.ToString();
            }
            catch (Exception)
            {
                return value.ToString();
            }
        }
    }
}
