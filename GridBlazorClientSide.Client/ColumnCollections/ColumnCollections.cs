﻿using GridBlazorClientSide.Client.Pages;
using GridBlazorClientSide.Client.Resources;
using GridBlazorClientSide.Shared.Models;
using GridShared;
using System;
using System.Collections.Generic;

namespace GridBlazorClientSide.Client.ColumnCollections
{
    public class ColumnCollections
    {
        public const string CompanyNameFilter = "CompanyNameFilter";

        public static Action<IGridColumnCollection<Order>> OrderColumns = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsWithTotals = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            .Format("{0:yyyy-MM-dd}").SetWidth(120)
            .Max(true).Min(true);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250)
            .Max(true).Min(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250)
            .Max(true).Min(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}")
            .Sum(true).Average(true).Max(true).Min(true);

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsGroupable = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>, object> OrderColumnsWithEdit = (c, obj) =>
        {
            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).RenderComponentAs<ButtonCell>(obj);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsCheckbox = c =>
        {
            /* Adding checkbox column: */
            c.Add().RenderComponentAs<CheckboxCell>();

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>, string> OrderColumnsWithCrud = (c, path) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "CustomerID" column: */
            c.Add(o => o.CustomerID, true).SetSelectField(true, o => o.Customer.CustomerID + " - " + o.Customer.CompanyName, path + $"api/SampleData/GetAllCustomers");

            /* Adding "EmployeeID" column: */
            c.Add(o => o.EmployeeID, true).SetSelectField(true, o => o.Employee.EmployeeID.ToString() + " - " + o.Employee.FirstName + " " + o.Employee.LastName, path + $"api/SampleData/GetAllEmployees");

            /* Adding "ShipVia" column: */
            c.Add(o => o.ShipVia, true).SetSelectField(true, o => o.Shipper == null ? "" : o.Shipper.ShipperID.ToString() + " - " + o.Shipper.CompanyName, path + $"api/SampleData/GetAllShippers");


            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250).SetCrudHidden(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250).SetCrudHidden(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No").SetCrudHidden(true);

            /* Adding hidden "RequiredDate" column: */
            c.Add(o => o.RequiredDate, true).Format("{0:yyyy-MM-dd}");

            /* Adding hidden "ShippedDate" column: */
            c.Add(o => o.ShippedDate, true).Format("{0:yyyy-MM-dd}");

            /* Adding hidden "ShipName" column: */
            c.Add(o => o.ShipName, true);

            /* Adding hidden "ShipAddress" column: */
            c.Add(o => o.ShipAddress, true);

            /* Adding hidden "ShipCity" column: */
            c.Add(o => o.ShipCity, true);

            /* Adding hidden "ShipPostalCode" column: */
            c.Add(o => o.ShipPostalCode, true);

            /* Adding hidden "ShipRegion" column: */
            c.Add(o => o.ShipRegion, true);

            /* Adding hidden "ShipCountry" column: */
            c.Add(o => o.ShipCountry, true);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsWithCustomCrud = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250).SetCrudHidden(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250).SetCrudHidden(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No").SetCrudHidden(true);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsWithSubgrids = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            //.SortInitialDirection(GridSortDirection.Descending)
            .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250)
            //.ThenSortByDescending(o => o.OrderID)
            //.SetInitialFilter(GridFilterType.StartsWith, "a")
            .SetFilterWidgetType(CompanyNameFilter);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>, IList<Action<object>>> OrderColumnsAllFeatures = (c, actions) =>
        {
            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).SetWidth(30).Css("hidden-xs") //hide on phones
            .RenderComponentAs<ButtonCell>(actions);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date")
            //.SortInitialDirection(GridSortDirection.Descending)
            .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
            .Format("{0:yyyy-MM-dd}").SetWidth(120)
            .Max(true).Min(true);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company")
            .SetWidth(250)
            //.ThenSortByDescending(o => o.OrderID)
            //.SetInitialFilter(GridFilterType.StartsWith, "a")
            .SetFilterWidgetType(CompanyNameFilter)
            .Max(true).Min(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250)
            .Max(true).Min(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled("Freight")
            .Format("{0:F}")
            .Sum(true).Average(true).Max(true).Min(true);


            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<Order>, IList<Action<object>>> OrderColumnsMultipleGrids = (c, actions) =>
        {
            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).SetWidth(30).RenderComponentAs<ButtonCell>(actions);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled("Number").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled("Date").Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled("Company").SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight).Titled("Freight").Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled("Is Vip").SetWidth(70).RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
        };

        public static Action<IGridColumnCollection<OrderDetail>> OrderDetailColumns = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID)
                .Titled("Order Number")
                .SetWidth(100);

            /* Adding "ProductID" column: */
            c.Add(o => o.ProductID)
                .Titled("Product Number")
                .SetWidth(100);

            /* Adding "ProductName" column: */
            c.Add(o => o.Product.ProductName)
                .Titled("Product Name")
                .SetWidth(250);

            /* Adding "Quantity" column: */
            c.Add(o => o.Quantity)
                .Titled("Quantity")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "UnitPrice" column: */
            c.Add(o => o.UnitPrice)
                .Titled("Unit Price")
                .SetWidth(100)
                .Format("{0:F}");
        };

        public static Action<IGridColumnCollection<OrderDetail>> OrderDetailColumnsAllFeatures = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID)
                .Titled("Order Number")
                //.SortInitialDirection(GridSortDirection.Descending)
                .SetWidth(100);

            /* Adding "ProductID" column: */
            c.Add(o => o.ProductID)
                .Titled("Product Number")
                //.ThenSortByDescending(o => o.ProductID)
                .SetWidth(100);

            /* Adding "ProductName" column: */
            c.Add(o => o.Product.ProductName)
                .Titled("Product Name")
                .SetWidth(250);

            /* Adding "Quantity" column: */
            c.Add(o => o.Quantity)
                .Titled("Quantity")
                .SetCellCssClassesContraint(o => o.Quantity >= 50 ? "red" : "")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "UnitPrice" column: */
            c.Add(o => o.UnitPrice)
                .Titled("Unit Price")
                .SetWidth(100)
                .Format("{0:F}");
        };

        public static Action<IGridColumnCollection<Customer>> CustomersColumns = c =>
        {
            /* Adding "CustomerID" column: */

            c.Add(o => o.CustomerID)
                .Titled("ID");

            /* Adding "CompanyName" column: */
            c.Add(o => o.CompanyName)
                .Titled("Company");

            /* Adding "ContactName" column: */
            c.Add(o => o.ContactName).Titled("ContactName").SetWidth(250);

            /* Adding "Address" column: */
            c.Add(o => o.Address)
                .Titled("Address");

            /* Adding "City" column: */
            c.Add(o => o.City)
                .Titled("City");

            /* Adding "PostalCode" column: */
            c.Add(o => o.PostalCode)
                .Titled("Postal Code");

            /* Adding "Country" column: */
            c.Add(o => o.Country)
                .Titled("Country");
        };
    }
}
