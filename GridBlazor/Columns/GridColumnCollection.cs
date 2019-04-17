using GridShared;
using GridShared.Columns;
using GridShared.Sorting;
using GridShared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridBlazor.Columns
{
    /// <summary>
    ///     Collection of collumns
    /// </summary>
    internal class GridColumnCollection<T> : KeyedCollection<string, IGridColumn>, IGridColumnCollection<T>
    {
        private readonly IColumnBuilder<T> _columnBuilder;
        public IGridSortSettings SortSettings { get; set; }

        public GridColumnCollection(IColumnBuilder<T> columnBuilder, IGridSortSettings sortSettings)
        {
            _columnBuilder = columnBuilder;
            SortSettings = sortSettings;
        }

        #region IGridColumnCollection<T> Members

        public IGridColumn<T> Add()
        {
            return Add(false);
        }

        public IGridColumn<T> Add(bool hidden)
        {
            return Add((Expression<Func<T, string>>)null, hidden);
        }

        public IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint)
        {
            return Add(constraint, false);
        }

        public IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, string columnName)
        {
            IGridColumn<T> newColumn = CreateColumn(constraint, false, columnName);
            return Add(newColumn);
        }

        public IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, bool hidden)
        {
            IGridColumn<T> newColumn = CreateColumn(constraint, hidden, string.Empty);
            return Add(newColumn);
        }

        public IGridColumn<T> Add(PropertyInfo pi)
        {
            IGridColumn<T> newColumn = _columnBuilder.CreateColumn(pi);
            if (newColumn == null) return null;
            return Add(newColumn);
        }

        public IGridColumn<T> Add(IGridColumn<T> column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            try
            {
                base.Add(column);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(string.Format("Column '{0}' already exist in the grid", column.Name));
            }
            UpdateColumnsSorting();
            return column;
        }

        public IGridColumn<T> Insert(int position, IGridColumn<T> column)
        {
            base.Insert(position, column);
            UpdateColumnsSorting();
            return column;
        }

        public IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint)
        {
            return Insert(position, constraint, false);
        }


        public IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, string columnName)
        {
            IGridColumn<T> newColumn = CreateColumn(constraint, false, columnName);
            return Insert(position, newColumn);
        }

        public IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, bool hidden)
        {
            IGridColumn<T> newColumn = CreateColumn(constraint, hidden, string.Empty);
            return Insert(position, newColumn);
        }

        public new IEnumerator<IGridColumn> GetEnumerator()
        {
            return base.GetEnumerator();
        }



        public IGridColumn<T> Get<TKey>(Expression<Func<T, TKey>> constraint)
        {
            var expr = constraint.Body as MemberExpression;
            if (expr == null)
                throw new ArgumentException(
                    string.Format("Expression '{0}' must be a member expression", constraint), "constraint");

            var name = PropertiesHelper.BuildColumnNameFromMemberExpression(expr);
            return this.FirstOrDefault(c => !string.IsNullOrEmpty(c.Name) && String.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase)) as IGridColumn<T>;
        }

        public IGridColumn GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return this.FirstOrDefault(c => !string.IsNullOrEmpty(c.Name) && String.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion

        protected override string GetKeyForItem(IGridColumn item)
        {
            return item.Name;
        }

        private IGridColumn<T> CreateColumn<TKey>(Expression<Func<T, TKey>> constraint, bool hidden, string columnName)
        {
            IGridColumn<T> newColumn = _columnBuilder.CreateColumn(constraint, hidden);
            if (!string.IsNullOrEmpty(columnName))
                newColumn.Name = columnName;
            return newColumn;
        }

        internal void UpdateColumnsSorting()
        {
            if (!string.IsNullOrEmpty(SortSettings.ColumnName))
            {
                foreach (IGridColumn gridColumn in this)
                {
                    gridColumn.IsSorted = gridColumn.Name == SortSettings.ColumnName;
                    if (gridColumn.Name == SortSettings.ColumnName)
                        gridColumn.Direction = SortSettings.Direction;
                    else
                        gridColumn.Direction = null;
                }
            }
        }
    }
}