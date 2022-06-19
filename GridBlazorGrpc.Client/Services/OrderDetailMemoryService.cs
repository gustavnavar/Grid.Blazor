using GridBlazorGrpc.Shared.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Client.Services
{
    public class OrderDetailMemoryService : IMemoryDataService<OrderDetail>
    {
        private readonly Action<IGridColumnCollection<OrderDetail>> _columns;
        private readonly IEnumerable<SelectItem> _products;

        public IList<OrderDetail> Items { get; private set; }

        public OrderDetailMemoryService(Action<IGridColumnCollection<OrderDetail>> columns,
            IEnumerable<SelectItem> products)
        {
            _columns = columns;
            _products = products;
            Items = new List<OrderDetail>();
        }

        public ItemsDTO<OrderDetail> GetGridRows(QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<OrderDetail>(Items, query, true, "Grid", _columns)
                    .Sortable()
                    .WithPaging(10)
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false, false);

            // return items to displays
            var items = server.ItemsToDisplay;
            return items;
        }

        public async Task<OrderDetail> Get(params object[] keys)
        {
            int orderID;
            int productID;
            int.TryParse(keys[0].ToString(), out orderID);
            int.TryParse(keys[1].ToString(), out productID);
            var item = Items.SingleOrDefault(o => o.OrderID == orderID && o.ProductID == productID);
            return await Task.FromResult(item);
        }

        public async Task Insert(OrderDetail item)
        {
            var it = Items.SingleOrDefault(o => o.OrderID == item.OrderID && o.ProductID == item.ProductID);
            if (it == null)
            {
                item.Product = new Product();
                item.Product.ProductID = item.ProductID;
                item.Product.ProductName = _products.SingleOrDefault(r => r.Value == item.ProductID.ToString())?.Title;
                Items.Add(item);
                await Task.CompletedTask;
            }
        }

        public async Task Update(OrderDetail item)
        {
            var it = Items.SingleOrDefault(o => o.OrderID == item.OrderID && o.ProductID == item.ProductID);
            if (it != null)
            {
                Items.Remove(it);
                if (item.Product == null)
                    item.Product = new Product();
                item.Product.ProductID = item.ProductID;
                item.Product.ProductName = _products.SingleOrDefault(r => r.Value == item.ProductID.ToString())?.Title;
                Items.Add(item);
                await Task.CompletedTask;
            }
        }

        public async Task Delete(params object[] keys)
        {
            var item = await Get(keys);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
    }
}
