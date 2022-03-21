using GridCore.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridCore
{
    /// <summary>
    ///     Base implementation of the Grid.Mvc
    /// </summary>
    public abstract class GridBase<T>
    {
        //pre-processors process items before adds to main collection (filtering and searching)
        private readonly List<IGridItemsProcessor<T>> _preprocessors = new List<IGridItemsProcessor<T>>();
        //processors process items after adds to main collection (sorting and paging)
        private readonly List<IGridItemsProcessor<T>> _processors = new List<IGridItemsProcessor<T>>();
        private IGridItemsProcessor<T> _totalsprocessor;
        protected IQueryable<T> BeforeItems; //items before processors
        protected IEnumerable<T> AfterItems; //items after processors
        protected Func<IQueryable<T>, Task<IList<T>>> ToListAsync;

        private int _itemsCount = -1; // total items count on collection
        private bool _itemsPreProcessed; //is preprocessors launched?
        private bool _itemsProcessed; //is processors launched?

        private Func<T, string> _rowCssClassesContraint;

        public abstract IGridSettingsProvider Settings { get; set; }

        public IQueryable<T> GridItems
        {
            get
            {
                //call preprocessors before:
                PreProcess();

                return BeforeItems;
            }
        }

        private void PreProcess()
        {
            if (!_itemsPreProcessed)
            {
                _itemsPreProcessed = true;
                foreach (var gridItemsProcessor in _preprocessors)
                {
                    BeforeItems = gridItemsProcessor.Process(BeforeItems);
                }
                // added to avoid 2nd EF opened task if counting later
                _itemsCount = BeforeItems.Count();

                // calculate totals
                _totalsprocessor.Process(BeforeItems);
            }
        }

        /// <summary>
        ///     Text in empty grid (no items for display)
        /// </summary>
        public string EmptyGridText { get; set; }

        /// <summary>
        /// Total count of items in the grid
        /// </summary>
        public int ItemsCount
        {
            get
            {
                //call preprocessors before:
                PreProcess();

                return _itemsCount;
            }
            set
            {
                _itemsCount = value; //value can be set by pager (for minimizing db calls)
            }
        }

        #region Custom row css classes
        public void SetRowCssClassesContraint(Func<T, string> contraint)
        {
            _rowCssClassesContraint = contraint;
        }

        public string GetRowCssClasses(object item)
        {
            if (_rowCssClassesContraint == null)
                return string.Empty;
            var typed = (T)item;
            if (typed == null)
                throw new InvalidCastException(string.Format("The item must be of type '{0}'", typeof(T).FullName));
            return _rowCssClassesContraint(typed);
        }

        #endregion

        protected void PrepareItemsToDisplay()
        {
            PrepareItemsToDisplayAsync().Wait();
        }

        protected async Task PrepareItemsToDisplayAsync(Func<IQueryable<T>, Task<IList<T>>> toListAsync = null)
        {
            if (!_itemsProcessed)
            {
                _itemsProcessed = true;
                IQueryable<T> itemsToProcess = GridItems;
                foreach (var processor in _processors.Where(p => p != null))
                {
                    if (processor.GetType().Equals(typeof(PagerGridItemsProcessor<T>)))
                        itemsToProcess = ((PagerGridItemsProcessor<T>)processor).Process(itemsToProcess, ItemsCount);
                    else
                        itemsToProcess = processor.Process(itemsToProcess);
                }
                if (toListAsync == null)
                    AfterItems = itemsToProcess.ToList();
                else
                    AfterItems = await toListAsync(itemsToProcess);
            }
        }

        #region Processors methods

        protected void AddItemsProcessor(IGridItemsProcessor<T> processor)
        {
            if (!_processors.Contains(processor))
                _processors.Add(processor);
        }

        protected void RemoveItemsProcessor(IGridItemsProcessor<T> processor)
        {
            if (_processors.Contains(processor))
                _processors.Remove(processor);
        }

        protected void AddItemsPreProcessor(IGridItemsProcessor<T> processor)
        {
            if (!_preprocessors.Contains(processor))
                _preprocessors.Add(processor);
        }

        protected void RemoveItemsPreProcessor(IGridItemsProcessor<T> processor)
        {
            if (_preprocessors.Contains(processor))
                _preprocessors.Remove(processor);
        }

        protected void InsertItemsProcessor(int position, IGridItemsProcessor<T> processor)
        {
            if (!_processors.Contains(processor))
                _processors.Insert(position, processor);
        }

        protected void SetTotalsProcessor(IGridItemsProcessor<T> processor)
        {
            _totalsprocessor = processor;
        }
        #endregion
    }
}