using GridShared;
using GridShared.Columns;
using GridShared.Pagination;
using GridShared.Totals;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace GridCore.Totals
{
    /// <summary>
    ///     Settings grid items, based on current searching settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TotalsGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly ISGrid _grid;
        private Func<IQueryable<T>, IQueryable<T>> _process;

        public TotalsGridItemsProcessor(ISGrid grid)
        {
            _grid = grid;
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            if (_process != null)
                return _process(items);

            if (items == null)
                return items;

            if (_grid.PagingType == PagingType.Virtualization && _grid.Pager.NoTotals)
                return items;

            foreach (IGridColumn<T> gridColumn in _grid.Columns)
            {
                ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

                if (gridColumn == null 
                    || (!gridColumn.IsSumEnabled && !gridColumn.IsAverageEnabled && !gridColumn.IsMaxEnabled && !gridColumn.IsMinEnabled))
                    continue;

                if (gridColumn.Totals == null)
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

                // check if the property is a Count method of an IEnumerable
                if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                {
                    expression = gridColumn.Totals.GetCountExpression(expression);
                    var lambdaExpression = Expression.Lambda<Func<T, Int32>>(expression, parameter);

                    if (gridColumn.IsSumEnabled)
                    {
                        gridColumn.SumValue = new Total((decimal?)items.Select(lambdaExpression).AsEnumerable().Sum());
                    }
                    if (gridColumn.IsAverageEnabled)
                    {
                        gridColumn.AverageValue = new Total((decimal?)items.Select(lambdaExpression).AsEnumerable().Average());
                    }
                    if (gridColumn.IsMaxEnabled)
                    {
                        gridColumn.MaxValue = new Total((decimal?)items.Select(lambdaExpression).AsEnumerable().Max());
                    }
                    if (gridColumn.IsMinEnabled)
                    {
                        gridColumn.MinValue = new Total((decimal?)items.Select(lambdaExpression).AsEnumerable().Min());
                    }

                    continue;
                }
               
                if (isNullable)
                {
                    if (type == typeof(Single))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T,Nullable<Single>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Int32))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Int32>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Int64))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Int64>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Double))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Double>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<Decimal>>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, Nullable<DateTime>>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total(items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total(items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(string))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, string>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total(items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total(items.Min(lambdaExpression));
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
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Int32))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Int32>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Int64))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Int64>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Double))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Double>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        var lambdaExpression = Expression.Lambda<Func<T, Decimal>>(expression, parameter);
                        if (gridColumn.IsSumEnabled)
                        {
                            gridColumn.SumValue = new Total((decimal?)items.Sum(lambdaExpression));
                        }
                        if (gridColumn.IsAverageEnabled)
                        {
                            gridColumn.AverageValue = new Total((decimal?)items.Average(lambdaExpression));
                        }
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total((decimal?)items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total((decimal?)items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, DateTime>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total(items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total(items.Min(lambdaExpression));
                        }
                    }
                    else if (type == typeof(string))
                    {
                        gridColumn.IsSumEnabled = false;
                        gridColumn.IsAverageEnabled = false;

                        var lambdaExpression = Expression.Lambda<Func<T, string>>(expression, parameter);
                        if (gridColumn.IsMaxEnabled)
                        {
                            gridColumn.MaxValue = new Total(items.Max(lambdaExpression));
                        }
                        if (gridColumn.IsMinEnabled)
                        {
                            gridColumn.MinValue = new Total(items.Min(lambdaExpression));
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


            foreach (IGridColumn<T> gridColumn in _grid.Columns.Where(r => ((IGridColumn<T>)r).Calculations.Any()))
            {
                foreach (var calculation in gridColumn.Calculations)
                {
                    var value = calculation.Value((IGridColumnCollection<T>)_grid.Columns);
                    Type type = value.GetType();

                    if (type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Double) || type == typeof(Decimal))
                    {
                        gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((decimal?)value));
                    }
                    else if (type == typeof(DateTime))
                    {
                        gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((DateTime)value));
                    }
                    else if (type == typeof(string))
                    {
                        gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((string)value));
                    }
                }
            }
            
            return items;         
        }

        public void SetProcess(Func<IQueryable<T>, IQueryable<T>> process)
        {
            _process = process;
        }

        #endregion
    }
}
