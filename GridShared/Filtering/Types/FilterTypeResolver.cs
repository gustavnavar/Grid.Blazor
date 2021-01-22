using System;
using System.Collections.Generic;

namespace GridShared.Filtering.Types
{
    public class FilterTypeResolver
    {
        private readonly List<IFilterType> _filterCollection = new List<IFilterType>();

        public FilterTypeResolver()
        {
            //add default filter types to collection:
            _filterCollection.Add(new TextFilterType());
            _filterCollection.Add(new Int32FilterType());
            _filterCollection.Add(new Int16FilterType());
            _filterCollection.Add(new BooleanFilterType());
            _filterCollection.Add(new DateTimeFilterType());
            _filterCollection.Add(new DateTimeOffsetFilterType());
            _filterCollection.Add(new DecimalFilterType());
            _filterCollection.Add(new ByteFilterType());
            _filterCollection.Add(new SingleFilterType());
            _filterCollection.Add(new Int64FilterType());
            _filterCollection.Add(new DoubleFilterType());
            _filterCollection.Add(new UInt16FilterType());
            _filterCollection.Add(new UInt32FilterType());
            _filterCollection.Add(new UInt64FilterType());
            _filterCollection.Add(new GuidFilterType());
        }

        public IFilterType GetFilterType(Type type)
        {
            if (type.IsEnum)
                return new EnumFilterType(type);

            foreach (IFilterType filterType in _filterCollection)
            {
                if (filterType.TargetType.FullName == type.FullName)
                    return filterType;
            }
            return new TextFilterType(); //try to process column type as text (not safe)
        }
    }
}