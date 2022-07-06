using GridShared.Columns;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared
{
    public interface IGridColumnCollection<T> : IGridColumnCollection
    {
        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="column">Columns</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add(IGridColumn<T> column);

        /// <summary>
        ///     Add new column to the grid, without specifying model property. Using this you must specify RenderValueAs method.
        /// </summary>
        /// <returns>Added column</returns>
        IGridColumn<T> Add();

        /// <summary>
        ///     Add new column to the grid, without specifying model property. Using this you must specify RenderValueAs method.
        /// </summary>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add(bool hidden);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add(string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add(bool hidden, string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, bool hidden);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer, string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer,  bool hidden);

        /// <summary>
        ///     Add new column based on property info, using reflection
        /// </summary>
        /// <returns>Added column</returns>
        IGridColumn<T> Add(PropertyInfo pi);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="column">Columns</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert(int position, IGridColumn<T> column);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, bool hidden);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="columnName">Specify column internal static name, used for sorting and filtering</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer, string columnName);

        /// <summary>
        ///     Add new column to the grid
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <param name="constraint">Member of generic class</param>
        /// <param name="hidden">Hidden column not display in grid, but you can get values from client side</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, IComparer<TKey> comparer, bool hidden);

        /// <summary>
        ///     Add new column based on property info, using reflection
        /// </summary>
        /// <param name="position">Position to insert</param>
        /// <returns>Added column</returns>
        IGridColumn<T> Insert(int position, PropertyInfo pi);

        /// <summary>
        ///     Get added column by member expression
        /// </summary>
        /// <param name="constraint">Member of generic class</param>
        /// <returns>Found column or NULL, if column not found</returns>
        IGridColumn<T> Get<TKey>(Expression<Func<T, TKey>> constraint);

        /// <summary>
        ///     Get column by internal name
        /// </summary>
        IGridColumn<T> Get(string name);

        /// <summary>
        ///     Parent grid
        /// </summary>
        IGrid Grid { get; }
    }

    public interface IGridColumnCollection : IEnumerable<IGridColumn>
    {
        /// <summary>
        ///     Get column by internal name
        /// </summary>
        IGridColumn GetByName(string name);
    }
}