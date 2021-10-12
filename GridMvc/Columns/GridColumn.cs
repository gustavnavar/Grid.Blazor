using GridCore;
using GridCore.Columns;
using GridShared.Columns;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GridMvc.Columns
{
    /// <summary>
    ///     Default implementation of Grid column
    /// </summary>
    public class GridColumn<T, TDataType> : GridCoreColumn<T, TDataType>
    {

        public GridColumn(Expression<Func<T, TDataType>> expression, ISGrid grid) : this(expression, null, grid)
        { }

        public GridColumn(Expression<Func<T, TDataType>> expression, IComparer<TDataType> comparer, ISGrid grid)
            : base (expression, comparer, grid)
        {

        }

        public override IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            if (componentType.IsSubclassOf(typeof(ViewComponent)))
            {
                ComponentType = componentType;
                Actions = actions;
                Functions = functions;
                Object = obj;
            }
            return this;
        }

        public override IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj, bool enableCard = true)
        {
            Type createComponentType = typeof(TCreateComponent);
            if (createComponentType.IsSubclassOf(typeof(ViewComponent)))
            {
                CreateComponentType = createComponentType;
            }
            Type readComponentType = typeof(TReadComponent);
            if (readComponentType.IsSubclassOf(typeof(ViewComponent)))
            {
                ReadComponentType = readComponentType;
            }
            Type updateComponentType = typeof(TUpdateComponent);
            if (updateComponentType.IsSubclassOf(typeof(ViewComponent)))
            {
                UpdateComponentType = updateComponentType;
            }
            Type deleteComponentType = typeof(TDeleteComponent);
            if (deleteComponentType.IsSubclassOf(typeof(ViewComponent)))
            {
                DeleteComponentType = deleteComponentType;
            }
            CrudActions = actions;
            CrudFunctions = functions;
            CrudObject = obj;
            EnableCard = enableCard;
            return this;
        }

    }
}