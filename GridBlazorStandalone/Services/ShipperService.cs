using GridBlazorStandalone.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorStandalone.Services
{
    public class ShipperService : IShipperService
    {
        private static List<Shipper> _shippers;

        public static List<Shipper> Shippers
        {
            get
            {
                if (_shippers == null || _shippers.Count == 0)
                    Init();
                return _shippers;
            }
            set
            {
                _shippers = value;
            }
        }

        public ShipperService()
        {
        }

        public ItemsDTO<Shipper> GetShippersGridRows(Action<IGridColumnCollection<Shipper>> columns, QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<Shipper>(Shippers, query, true, "shippersGrid", columns)
                .WithPaging(10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .WithGridItemsCount()
                .Groupable(true)
                .Searchable(true, false, false)
                .SetRemoveDiacritics<StringUtils>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return items;
        }

        public IEnumerable<SelectItem> GetAllShippers()
        {
            return Shippers.Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - " + r.CompanyName)).ToList();
        }

        public async Task<Shipper> Get(params object[] keys)
        {
            int shipperId;
            int.TryParse(keys[0].ToString(), out shipperId);
            var order = Shippers.SingleOrDefault(r => r.ShipperID == shipperId);
            return await Task.FromResult(order);
        }

        public async Task Insert(Shipper item)
        {
            if (item.ShipperID == 0)
                item.ShipperID = Shippers.Max(r => r.ShipperID) + 1;

            var shipper = Shippers.SingleOrDefault(r => r.ShipperID == item.ShipperID);
            if (shipper == null)
            {
                Shippers.Add(item);
                await Task.CompletedTask;
            }
            else
            {
                throw new GridException("SHISRV-01", "Error creating the shipper");
            }
        }

        public async Task Update(Shipper item)
        {
            var shipper = Shippers.SingleOrDefault(r => r.ShipperID == item.ShipperID);
            if (shipper != null)
            {
                shipper = item;
                await Task.CompletedTask;
            }
            else
            {
                throw new GridException("SHISRV-02", "Error updating the shipper");
            }
        }

        public async Task Delete(params object[] keys)
        {
            var shipper = await Get(keys);
            if (shipper != null)
            {
                Shippers.Remove(shipper);
            }
            else
            {
                throw new GridException("SHISRV-03", "Error deleting the shipper");
            }
        }

        private static void Init()
        {
            _shippers = new List<Shipper>();

            _shippers.Add(new Shipper { ShipperID = 1, CompanyName = "Speedy Express", Phone = "(503) 555-9831" });
            _shippers.Add(new Shipper { ShipperID = 2, CompanyName = "United Package", Phone = "(503) 555-3199" });
            _shippers.Add(new Shipper { ShipperID = 3, CompanyName = "Federal Shipping", Phone = "(503) 555-9931" });

            /**
            SELECT '_shippers.Add(new Shipper {ShipperID=' + CONVERT(varchar, [ShipperID])
                  + ', CompanyName="' + [CompanyName]
                  + '", Phone="' + IIF([Phone] IS NULL, 'null', [Phone]) + '"});'
              FROM [NORTHWIND].[dbo].[Shippers]
            */
        }
    }

    public interface IShipperService : ICrudDataService<Shipper>
    {
        ItemsDTO<Shipper> GetShippersGridRows(Action<IGridColumnCollection<Shipper>> columns, QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetAllShippers();
    }
}
