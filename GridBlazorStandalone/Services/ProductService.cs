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
    public class ProductService : IProductService
    {
        private static List<Product> _products;

        public static List<Product> Products
        {
            get
            {
                if (_products == null || _products.Count == 0)
                    Init();
                return _products;
            }
            set
            {
                _products = value;
            }
        }
        public ProductService()
        {
        }

        public ItemsDTO<Product> GetProductsGridRows(Action<IGridColumnCollection<Product>> columns, QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<Product>(Products, query, true, "productsGrid", columns)
                .WithPaging(10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .WithGridItemsCount()
                .Groupable(true)
                .Searchable(true, false, false);

            var items = server.ItemsToDisplay;
            return items;
        }

        public IEnumerable<SelectItem> GetAllProducts()
        {
            return Products.Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - " + r.ProductName)).ToList();
        }

        public async Task<Product> Get(params object[] keys)
        {
            int productId;
            int.TryParse(keys[0].ToString(), out productId);
            var order = Products.SingleOrDefault(r => r.ProductID == productId);
            return await Task.FromResult(order);
        }

        public async Task Insert(Product item)
        {
            if (item.ProductID == 0)
                item.ProductID = Products.Max(r => r.ProductID) + 1;

            var product = Products.SingleOrDefault(r => r.ProductID == item.ProductID);
            if (product == null)
            {
                Products.Add(item);
                await Task.CompletedTask;
            }
            else
            {
                throw new GridException("PROSRV-01", "Error creating the product");
            }
        }

        public async Task Update(Product item)
        {
            var product = Products.SingleOrDefault(r => r.ProductID == item.ProductID);
            if (product != null)
            {
                product = item;
                await Task.CompletedTask;
            }
            else
            {
                throw new GridException("PROSRV-02", "Error updating the product");
            }
        }

        public async Task Delete(params object[] keys)
        {
            var product = await Get(keys);
            if (product != null)
            {
                Products.Remove(product);
            }
            else
            {
                throw new GridException("PROSRV-03", "Error deleting the product");
            }
        }

        private static void Init()
        {
            _products = new List<Product>();

            _products.Add(new Product { ProductID = 1, ProductName = "Chai", SupplierID = 1, CategoryID = 1, QuantityPerUnit = "10 boxes x 20 bags", UnitPrice = 18.00m, UnitsInStock = 39, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 2, ProductName = "Chang", SupplierID = 1, CategoryID = 1, QuantityPerUnit = "24 - 12 oz bottles", UnitPrice = 19.00m, UnitsInStock = 17, UnitsOnOrder = 40, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 3, ProductName = "Aniseed Syrup", SupplierID = 1, CategoryID = 2, QuantityPerUnit = "12 - 550 ml bottles", UnitPrice = 10.00m, UnitsInStock = 13, UnitsOnOrder = 70, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", SupplierID = 2, CategoryID = 2, QuantityPerUnit = "48 - 6 oz jars", UnitPrice = 22.00m, UnitsInStock = 53, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", SupplierID = 2, CategoryID = 2, QuantityPerUnit = "36 boxes", UnitPrice = 21.35m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 6, ProductName = "Grandma's Boysenberry Spread", SupplierID = 3, CategoryID = 2, QuantityPerUnit = "12 - 8 oz jars", UnitPrice = 25.00m, UnitsInStock = 120, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", SupplierID = 3, CategoryID = 7, QuantityPerUnit = "12 - 1 lb pkgs.", UnitPrice = 30.00m, UnitsInStock = 15, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", SupplierID = 3, CategoryID = 2, QuantityPerUnit = "12 - 12 oz jars", UnitPrice = 40.00m, UnitsInStock = 6, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 9, ProductName = "Mishi Kobe Niku", SupplierID = 4, CategoryID = 6, QuantityPerUnit = "18 - 500 g pkgs.", UnitPrice = 97.00m, UnitsInStock = 29, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 10, ProductName = "Ikura", SupplierID = 4, CategoryID = 8, QuantityPerUnit = "12 - 200 ml jars", UnitPrice = 31.00m, UnitsInStock = 31, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 11, ProductName = "Queso Cabrales", SupplierID = 5, CategoryID = 4, QuantityPerUnit = "1 kg pkg.", UnitPrice = 21.00m, UnitsInStock = 22, UnitsOnOrder = 30, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 12, ProductName = "Queso Manchego La Pastora", SupplierID = 5, CategoryID = 4, QuantityPerUnit = "10 - 500 g pkgs.", UnitPrice = 38.00m, UnitsInStock = 86, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 13, ProductName = "Konbu", SupplierID = 6, CategoryID = 8, QuantityPerUnit = "2 kg box", UnitPrice = 6.00m, UnitsInStock = 24, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 14, ProductName = "Tofu", SupplierID = 6, CategoryID = 7, QuantityPerUnit = "40 - 100 g pkgs.", UnitPrice = 23.25m, UnitsInStock = 35, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 15, ProductName = "Genen Shouyu", SupplierID = 6, CategoryID = 2, QuantityPerUnit = "24 - 250 ml bottles", UnitPrice = 15.50m, UnitsInStock = 39, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 16, ProductName = "Pavlova", SupplierID = 7, CategoryID = 3, QuantityPerUnit = "32 - 500 g boxes", UnitPrice = 17.45m, UnitsInStock = 29, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 17, ProductName = "Alice Mutton", SupplierID = 7, CategoryID = 6, QuantityPerUnit = "20 - 1 kg tins", UnitPrice = 39.00m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 18, ProductName = "Carnarvon Tigers", SupplierID = 7, CategoryID = 8, QuantityPerUnit = "16 kg pkg.", UnitPrice = 62.50m, UnitsInStock = 42, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 19, ProductName = "Teatime Chocolate Biscuits", SupplierID = 8, CategoryID = 3, QuantityPerUnit = "10 boxes x 12 pieces", UnitPrice = 9.20m, UnitsInStock = 25, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 20, ProductName = "Sir Rodney's Marmalade", SupplierID = 8, CategoryID = 3, QuantityPerUnit = "30 gift boxes", UnitPrice = 81.00m, UnitsInStock = 40, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 21, ProductName = "Sir Rodney's Scones", SupplierID = 8, CategoryID = 3, QuantityPerUnit = "24 pkgs. x 4 pieces", UnitPrice = 10.00m, UnitsInStock = 3, UnitsOnOrder = 40, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 22, ProductName = "Gustaf's Knäckebröd", SupplierID = 9, CategoryID = 5, QuantityPerUnit = "24 - 500 g pkgs.", UnitPrice = 21.00m, UnitsInStock = 104, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 23, ProductName = "Tunnbröd", SupplierID = 9, CategoryID = 5, QuantityPerUnit = "12 - 250 g pkgs.", UnitPrice = 9.00m, UnitsInStock = 61, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 24, ProductName = "Guaraná Fantástica", SupplierID = 10, CategoryID = 1, QuantityPerUnit = "12 - 355 ml cans", UnitPrice = 4.50m, UnitsInStock = 20, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", SupplierID = 11, CategoryID = 3, QuantityPerUnit = "20 - 450 g glasses", UnitPrice = 14.00m, UnitsInStock = 76, UnitsOnOrder = 0, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 26, ProductName = "Gumbär Gummibärchen", SupplierID = 11, CategoryID = 3, QuantityPerUnit = "100 - 250 g bags", UnitPrice = 31.23m, UnitsInStock = 15, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 27, ProductName = "Schoggi Schokolade", SupplierID = 11, CategoryID = 3, QuantityPerUnit = "100 - 100 g pieces", UnitPrice = 43.90m, UnitsInStock = 49, UnitsOnOrder = 0, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 28, ProductName = "Rössle Sauerkraut", SupplierID = 12, CategoryID = 7, QuantityPerUnit = "25 - 825 g cans", UnitPrice = 45.60m, UnitsInStock = 26, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 29, ProductName = "Thüringer Rostbratwurst", SupplierID = 12, CategoryID = 6, QuantityPerUnit = "50 bags x 30 sausgs.", UnitPrice = 123.79m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 30, ProductName = "Nord-Ost Matjeshering", SupplierID = 13, CategoryID = 8, QuantityPerUnit = "10 - 200 g glasses", UnitPrice = 25.89m, UnitsInStock = 10, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 31, ProductName = "Gorgonzola Telino", SupplierID = 14, CategoryID = 4, QuantityPerUnit = "12 - 100 g pkgs", UnitPrice = 12.50m, UnitsInStock = 0, UnitsOnOrder = 70, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 32, ProductName = "Mascarpone Fabioli", SupplierID = 14, CategoryID = 4, QuantityPerUnit = "24 - 200 g pkgs.", UnitPrice = 32.00m, UnitsInStock = 9, UnitsOnOrder = 40, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 33, ProductName = "Geitost", SupplierID = 15, CategoryID = 4, QuantityPerUnit = "500 g", UnitPrice = 2.50m, UnitsInStock = 112, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 34, ProductName = "Sasquatch Ale", SupplierID = 16, CategoryID = 1, QuantityPerUnit = "24 - 12 oz bottles", UnitPrice = 14.00m, UnitsInStock = 111, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 35, ProductName = "Steeleye Stout", SupplierID = 16, CategoryID = 1, QuantityPerUnit = "24 - 12 oz bottles", UnitPrice = 18.00m, UnitsInStock = 20, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 36, ProductName = "Inlagd Sill", SupplierID = 17, CategoryID = 8, QuantityPerUnit = "24 - 250 g  jars", UnitPrice = 19.00m, UnitsInStock = 112, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 37, ProductName = "Gravad lax", SupplierID = 17, CategoryID = 8, QuantityPerUnit = "12 - 500 g pkgs.", UnitPrice = 26.00m, UnitsInStock = 11, UnitsOnOrder = 50, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 38, ProductName = "Côte de Blaye", SupplierID = 18, CategoryID = 1, QuantityPerUnit = "12 - 75 cl bottles", UnitPrice = 263.50m, UnitsInStock = 17, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 39, ProductName = "Chartreuse verte", SupplierID = 18, CategoryID = 1, QuantityPerUnit = "750 cc per bottle", UnitPrice = 18.00m, UnitsInStock = 69, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 40, ProductName = "Boston Crab Meat", SupplierID = 19, CategoryID = 8, QuantityPerUnit = "24 - 4 oz tins", UnitPrice = 18.40m, UnitsInStock = 123, UnitsOnOrder = 0, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 41, ProductName = "Jack's New England Clam Chowder", SupplierID = 19, CategoryID = 8, QuantityPerUnit = "12 - 12 oz cans", UnitPrice = 9.65m, UnitsInStock = 85, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", SupplierID = 20, CategoryID = 5, QuantityPerUnit = "32 - 1 kg pkgs.", UnitPrice = 14.00m, UnitsInStock = 26, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 43, ProductName = "Ipoh Coffee", SupplierID = 20, CategoryID = 1, QuantityPerUnit = "16 - 500 g tins", UnitPrice = 46.00m, UnitsInStock = 17, UnitsOnOrder = 10, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 44, ProductName = "Gula Malacca", SupplierID = 20, CategoryID = 2, QuantityPerUnit = "20 - 2 kg bags", UnitPrice = 19.45m, UnitsInStock = 27, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 45, ProductName = "Rogede sild", SupplierID = 21, CategoryID = 8, QuantityPerUnit = "1k pkg.", UnitPrice = 9.50m, UnitsInStock = 5, UnitsOnOrder = 70, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 46, ProductName = "Spegesild", SupplierID = 21, CategoryID = 8, QuantityPerUnit = "4 - 450 g glasses", UnitPrice = 12.00m, UnitsInStock = 95, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 47, ProductName = "Zaanse koeken", SupplierID = 22, CategoryID = 3, QuantityPerUnit = "10 - 4 oz boxes", UnitPrice = 9.50m, UnitsInStock = 36, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 48, ProductName = "Chocolade", SupplierID = 22, CategoryID = 3, QuantityPerUnit = "10 pkgs.", UnitPrice = 12.75m, UnitsInStock = 15, UnitsOnOrder = 70, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 49, ProductName = "Maxilaku", SupplierID = 23, CategoryID = 3, QuantityPerUnit = "24 - 50 g pkgs.", UnitPrice = 20.00m, UnitsInStock = 10, UnitsOnOrder = 60, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 50, ProductName = "Valkoinen suklaa", SupplierID = 23, CategoryID = 3, QuantityPerUnit = "12 - 100 g bars", UnitPrice = 16.25m, UnitsInStock = 65, UnitsOnOrder = 0, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 51, ProductName = "Manjimup Dried Apples", SupplierID = 24, CategoryID = 7, QuantityPerUnit = "50 - 300 g pkgs.", UnitPrice = 53.00m, UnitsInStock = 20, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 52, ProductName = "Filo Mix", SupplierID = 24, CategoryID = 5, QuantityPerUnit = "16 - 2 kg boxes", UnitPrice = 7.00m, UnitsInStock = 38, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 53, ProductName = "Perth Pasties", SupplierID = 24, CategoryID = 6, QuantityPerUnit = "48 pieces", UnitPrice = 32.80m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true });
            _products.Add(new Product { ProductID = 54, ProductName = "Tourtière", SupplierID = 25, CategoryID = 6, QuantityPerUnit = "16 pies", UnitPrice = 7.45m, UnitsInStock = 21, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 55, ProductName = "Pâté chinois", SupplierID = 25, CategoryID = 6, QuantityPerUnit = "24 boxes x 2 pies", UnitPrice = 24.00m, UnitsInStock = 115, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 56, ProductName = "Gnocchi di nonna Alice", SupplierID = 26, CategoryID = 5, QuantityPerUnit = "24 - 250 g pkgs.", UnitPrice = 38.00m, UnitsInStock = 21, UnitsOnOrder = 10, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 57, ProductName = "Ravioli Angelo", SupplierID = 26, CategoryID = 5, QuantityPerUnit = "24 - 250 g pkgs.", UnitPrice = 19.50m, UnitsInStock = 36, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 58, ProductName = "Escargots de Bourgogne", SupplierID = 27, CategoryID = 8, QuantityPerUnit = "24 pieces", UnitPrice = 13.25m, UnitsInStock = 62, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 59, ProductName = "Raclette Courdavault", SupplierID = 28, CategoryID = 4, QuantityPerUnit = "5 kg pkg.", UnitPrice = 55.00m, UnitsInStock = 79, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 60, ProductName = "Camembert Pierrot", SupplierID = 28, CategoryID = 4, QuantityPerUnit = "15 - 300 g rounds", UnitPrice = 34.00m, UnitsInStock = 19, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 61, ProductName = "Sirop d'érable", SupplierID = 29, CategoryID = 2, QuantityPerUnit = "24 - 500 ml bottles", UnitPrice = 28.50m, UnitsInStock = 113, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 62, ProductName = "Tarte au sucre", SupplierID = 29, CategoryID = 3, QuantityPerUnit = "48 pies", UnitPrice = 49.30m, UnitsInStock = 17, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 63, ProductName = "Vegie-spread", SupplierID = 7, CategoryID = 2, QuantityPerUnit = "15 - 625 g jars", UnitPrice = 43.90m, UnitsInStock = 24, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 64, ProductName = "Wimmers gute Semmelknödel", SupplierID = 12, CategoryID = 5, QuantityPerUnit = "20 bags x 4 pieces", UnitPrice = 33.25m, UnitsInStock = 22, UnitsOnOrder = 80, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", SupplierID = 2, CategoryID = 2, QuantityPerUnit = "32 - 8 oz bottles", UnitPrice = 21.05m, UnitsInStock = 76, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", SupplierID = 2, CategoryID = 2, QuantityPerUnit = "24 - 8 oz jars", UnitPrice = 17.00m, UnitsInStock = 4, UnitsOnOrder = 100, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 67, ProductName = "Laughing Lumberjack Lager", SupplierID = 16, CategoryID = 1, QuantityPerUnit = "24 - 12 oz bottles", UnitPrice = 14.00m, UnitsInStock = 52, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false });
            _products.Add(new Product { ProductID = 68, ProductName = "Scottish Longbreads", SupplierID = 8, CategoryID = 3, QuantityPerUnit = "10 boxes x 8 pieces", UnitPrice = 12.50m, UnitsInStock = 6, UnitsOnOrder = 10, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 69, ProductName = "Gudbrandsdalsost", SupplierID = 15, CategoryID = 4, QuantityPerUnit = "10 kg pkg.", UnitPrice = 36.00m, UnitsInStock = 26, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });
            _products.Add(new Product { ProductID = 70, ProductName = "Outback Lager", SupplierID = 7, CategoryID = 1, QuantityPerUnit = "24 - 355 ml bottles", UnitPrice = 15.00m, UnitsInStock = 15, UnitsOnOrder = 10, ReorderLevel = 30, Discontinued = false });
            _products.Add(new Product { ProductID = 71, ProductName = "Flotemysost", SupplierID = 15, CategoryID = 4, QuantityPerUnit = "10 - 500 g pkgs.", UnitPrice = 21.50m, UnitsInStock = 26, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 72, ProductName = "Mozzarella di Giovanni", SupplierID = 14, CategoryID = 4, QuantityPerUnit = "24 - 200 g pkgs.", UnitPrice = 34.80m, UnitsInStock = 14, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false });
            _products.Add(new Product { ProductID = 73, ProductName = "Röd Kaviar", SupplierID = 17, CategoryID = 8, QuantityPerUnit = "24 - 150 g jars", UnitPrice = 15.00m, UnitsInStock = 101, UnitsOnOrder = 0, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 74, ProductName = "Longlife Tofu", SupplierID = 4, CategoryID = 7, QuantityPerUnit = "5 kg pkg.", UnitPrice = 10.00m, UnitsInStock = 4, UnitsOnOrder = 20, ReorderLevel = 5, Discontinued = false });
            _products.Add(new Product { ProductID = 75, ProductName = "Rhönbräu Klosterbier", SupplierID = 12, CategoryID = 1, QuantityPerUnit = "24 - 0.5 l bottles", UnitPrice = 7.75m, UnitsInStock = 125, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false });
            _products.Add(new Product { ProductID = 76, ProductName = "Lakkalikööri", SupplierID = 23, CategoryID = 1, QuantityPerUnit = "500 ml", UnitPrice = 18.00m, UnitsInStock = 57, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false });
            _products.Add(new Product { ProductID = 77, ProductName = "Original Frankfurter grüne Soße", SupplierID = 12, CategoryID = 2, QuantityPerUnit = "12 boxes", UnitPrice = 13.00m, UnitsInStock = 32, UnitsOnOrder = 0, ReorderLevel = 15, Discontinued = false });

            /*
            SELECT '_products.Add(new Product {ProductID=' + CONVERT(varchar, [ProductID])
                  + ', ProductName="' + [ProductName]
                  + '", SupplierID=' + CONVERT(varchar, [SupplierID])
                  + ', CategoryID=' + CONVERT(varchar, [CategoryID])
                  + ', QuantityPerUnit="' + [QuantityPerUnit]
                  + '", UnitPrice=' + CONVERT(varchar, [UnitPrice])
                  + 'm, UnitsInStock=' + CONVERT(varchar, [UnitsInStock])
                  + ', UnitsOnOrder=' + CONVERT(varchar, [UnitsOnOrder])
                  + ', ReorderLevel=' + CONVERT(varchar, [ReorderLevel])
                  + ', Discontinued=' + IIF([Discontinued] = 0, 'false', 'true') + '});'
              FROM [NORTHWIND].[dbo].[Products]
            */
        }
    }

    public interface IProductService : ICrudDataService<Product>
    {
        ItemsDTO<Product> GetProductsGridRows(Action<IGridColumnCollection<Product>> columns, QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetAllProducts();
    }
}
