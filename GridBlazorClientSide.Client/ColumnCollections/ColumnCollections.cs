﻿using GridBlazor.Resources;
using GridBlazorClientSide.Client.Pages;
using GridBlazorClientSide.Client.Resources;
using GridBlazorClientSide.Shared.Models;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.ColumnCollections
{
    public class ColumnCollections
    {
        public const string CompanyNameFilter = "CompanyNameFilter";

        public static Action<IGridColumnCollection<Order>> OrderColumns = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetTooltip("Order ID is ... ").SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Customer.Country" hidden column: */
            c.Add(o => o.Customer.Country, true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>, IComparer<string>> OrderColumnsWithComparer = (c, comparer) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName, comparer).Titled(SharedResource.CompanyName)
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Customer.Country" hidden column: */
            c.Add(o => o.Customer.Country, true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsWithTotals = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .Format("{0:yyyy-MM-dd}").SetWidth(120)
            .Max(true).Min(true);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250)
            .Max(true).Min(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250)
            .Max(true).Min(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}")
            .Sum(true).Average(true).Max(true).Min(true);

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsGroupable = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .ThenSortBy(o => o.ShipVia)
            .ThenSortByDescending(o => o.Freight)
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "ShipVia" column: */
            c.Add(o => o.ShipVia).Titled("Via");

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>, IList<Func<object, Task>>, object> 
            OrderColumnsWithEdit = (c, functions, obj) =>
        {
            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).RenderComponentAs<ButtonCellEdit>(obj);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).SetWidth(100).Css("hidden-xs") //hide on phones
            .RenderComponentAs<ButtonDbUpdate>(functions);

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsCheckbox = c =>
        {
            /* Adding checkbox column: */
            c.Add("CheckboxColumn").SetCheckboxColumn(true, o => o.Customer.IsVip).SetWidth(40);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .SetWidth(120).RenderComponentAs<TooltipCell>();

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
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
            .SetInputType(InputType.Week)
            .SetFilterWidgetType("Week")
            .RenderValueAs(o => DateTimeUtils.GetWeekDateTimeString(o.OrderDate))
            .SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250).SetCrudHidden(true).SetReadOnlyOnUpdate(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250).SetCrudHidden(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel).SetCrudHidden(true);

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

            /* Adding not mapped column, that renders a component */
            c.Add(true).Titled("Images").RenderCrudComponentAs<Carousel, Carousel, NullComponent, NullComponent>();
        };

        public static Action<IGridColumnCollection<Order>, string, Func<object[], bool, bool, bool, bool, Task<IGrid>>>
            OrderColumnsWithNestedCrud = (c, path, subgrids) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetTooltip("Order ID is ... ").SetWidth(100);

            /* Adding "CustomerID" column: */
            c.Add(o => o.CustomerID, true).SetSelectField(true, o => o.Customer.CustomerID + " - " + o.Customer.CompanyName, path + $"api/SampleData/GetAllCustomers");

            /* Adding "EmployeeID" column: */
            c.Add(o => o.EmployeeID, true).SetSelectField(true, o => o.Employee.EmployeeID.ToString() + " - " + o.Employee.FirstName + " " + o.Employee.LastName, path + $"api/SampleData/GetAllEmployees");

            /* Adding "ShipVia" column: */
            c.Add(o => o.ShipVia, true).SetSelectField(true, o => o.Shipper == null ? "" : o.Shipper.ShipperID.ToString() + " - " + o.Shipper.CompanyName, path + $"api/SampleData/GetAllShippers");

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            .SetInputType(InputType.Month)
            .SetFilterWidgetType("Month")
            .Format("{0:yyyy-MM}")
            .SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250).SetCrudHidden(true).SetReadOnlyOnUpdate(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250).SetCrudHidden(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel).SetCrudHidden(true);

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

            /* Adding hidden "OrderDetails" column for a CRUD subgrid: */
            c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid("tabGroup1", subgrids, ("OrderID", "OrderID"));

            /* Adding not mapped column, that renders a component in a tab */
            c.Add(true).Titled("Images").RenderCrudComponentAs<NullComponent, NullComponent, Carousel, Carousel>().SetTabGroup("tabGroup1");
        };

        public static Action<IGridColumnCollection<Order>, string> OrderColumnsWithCustomCrud = (c, path) =>
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
            .SetWidth(250).SetCrudHidden(true).SetReadOnlyOnUpdate(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250).SetCrudHidden(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel).SetCrudHidden(true);
        };

        public static Action<IGridColumnCollection<Order>> OrderColumnsWithSubgrids = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            //.SortInitialDirection(GridSortDirection.Descending)
            .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
            .Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250)
            //.ThenSortByDescending(o => o.OrderID)
            //.SetInitialFilter(GridFilterType.StartsWith, "a")
            .SetFilterWidgetType(CompanyNameFilter);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<Order>, string, IEnumerable<SelectItem>, 
            Func<object[], bool, bool, bool, bool, Task<IGrid>>> 
            OrderColumnsAllFeatures = (c, path, list, subgrids) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetTooltip("Order ID is ... ").SetWidth(100);

            /* Adding "CustomerID" column: */
            c.Add(o => o.CustomerID, true).SetSelectField(true, o => o.Customer.CustomerID + " - " + o.Customer.CompanyName, path + $"api/SampleData/GetAllCustomers");

            /* Adding "EmployeeID" column: */
            c.Add(o => o.EmployeeID, true).SetSelectField(true, o => o.Employee.EmployeeID.ToString() + " - " + o.Employee.FirstName + " " + o.Employee.LastName, path + $"api/SampleData/GetAllEmployees");

            /* Adding "ShipVia" column: */
            c.Add(o => o.ShipVia).Titled("Via")
            .SetWidth(250).RenderValueAs(o => o.Shipper == null ? "" : o.Shipper.CompanyName)
            .SetSelectField(true, o => o.Shipper == null ? "" : o.Shipper.ShipperID.ToString() + " - " + o.Shipper.CompanyName, path + $"api/SampleData/GetAllShippers")
            .SetListFilter(list);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate)
            //.SortInitialDirection(GridSortDirection.Descending)
            .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
            .SetInputType(InputType.DateTimeLocal)
            .SetFilterWidgetType("DateTimeLocal")
            .Format("{0:yyyy-MM-dd HH:mm}").SetWidth(120)
            .Max(true).Min(true);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName)
            .SetWidth(250)
            .SetCrudHidden(true).SetReadOnlyOnUpdate(true)
            //.ThenSortByDescending(o => o.OrderID)
            //.SetInitialFilter(GridFilterType.StartsWith, "a")
            .SetFilterWidgetType(CompanyNameFilter)
            .Max(true).Min(true);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250)
            .SetCrudHidden(true)
            .Max(true).Min(true);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight)
            .Titled(SharedResource.Freight)
            .Format("{0:F}")
            .Sum(true).Average(true).Max(true).Min(true);

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).Css("hidden-xs") //hide on phones
            .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel).SetCrudHidden(true);

            /* Adding "Photo" column: */
            c.Add().Encoded(false).Sanitized(false).SetWidth("5%").SetCrudHidden(true)
            .RenderValueAs(o => $"<img width='50' height='50' src='data:image/bmp;base64,{o.Employee.Base64String}' />");

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

            /* Adding hidden "OrderDetails" column for a CRUD subgrid: */
            c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid("tabGroup1", subgrids, ("OrderID", "OrderID"));
        };

        public static Action<IGridColumnCollection<Order>, IList<Func<object, Task>>> OrderColumnsMultipleGrids = (c, functions) =>
        {
            /* Adding not mapped column, that renders body, using inline Razor html helper */
            c.Add().Encoded(false).Sanitized(false).SetWidth(30).RenderComponentAs<ButtonCellMultipleGrids>(functions);

            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID).Titled(SharedResource.Number).SetWidth(100);

            /* Adding "OrderDate" column: */
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate).Format("{0:yyyy-MM-dd}").SetWidth(120);

            /* Adding "CompanyName" column: */
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName).SetWidth(250);

            /* Adding "ContactName" column: */
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250);

            /* Adding "Freight" column: */
            c.Add(o => o.Freight).Titled(SharedResource.Freight).Format("{0:F}");

            /* Adding "Vip customer" column: */
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(70).RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
        };

        public static Action<IGridColumnCollection<OrderDetail>> OrderDetailColumns = c =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID, true)
                .SetWidth(100);

            /* Adding "ProductID" column: */
            c.Add(o => o.ProductID)
                .Titled("ProdId")
                .SetWidth(100);

            /* Adding "ProductName" column: */
            c.Add(o => o.Product.ProductName)
                .Titled("ProdName")
                .SetWidth(250);

            /* Adding "Quantity" column: */
            c.Add(o => o.Quantity)
                .Titled("Quant")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "UnitPrice" column: */
            c.Add(o => o.UnitPrice)
                .Titled("Unit Price")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "Discount" column: */
            c.Add(o => o.Discount)
                .Titled("Disc")
                .SetWidth(100)
                .Format("{0:F}");
        };

        public static Action<IGridColumnCollection<OrderDetail>, string> OrderDetailColumnsCrud = (c, path) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID, true)
                .SetPrimaryKey(true)
                .SetWidth(100);

            /* Adding "ProductID" column: */
            c.Add(o => o.ProductID)
                .SetPrimaryKey(true)
                .SetSelectField(true, o => o.Product.ProductID + " - " + o.Product.ProductName, path + $"api/SampleData/GetAllProducts")
                .Titled("ProdId")
                .SetWidth(100);

            /* Adding "ProductName" column: */
            c.Add(o => o.Product.ProductName)
                .Titled("ProdName")
                .SetCrudHidden(true)
                .SetWidth(250);

            /* Adding "Quantity" column: */
            c.Add(o => o.Quantity)
                .Titled("Quant")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "UnitPrice" column: */
            c.Add(o => o.UnitPrice)
                .Titled("Unit Price")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "Discount" column: */
            c.Add(o => o.Discount)
                .Titled("Disc")
                .SetWidth(100)
                .Format("{0:F}");
        };

        public static Action<IGridColumnCollection<OrderDetail>, string> 
            OrderDetailColumnsAllFeatures = (c, path) =>
        {
            /* Adding "OrderID" column: */
            c.Add(o => o.OrderID, true)
                .SetPrimaryKey(true)
                //.SortInitialDirection(GridSortDirection.Descending)
                .SetWidth(100);

            /* Adding "ProductID" column: */
            c.Add(o => o.ProductID)
                .SetPrimaryKey(true)
                .SetSelectField(true, o => o.Product.ProductID + " - " + o.Product.ProductName, path + $"api/SampleData/GetAllProducts")
                .Titled("ProdId")
                //.ThenSortByDescending(o => o.ProductID)
                .SetWidth(100);

            /* Adding "ProductName" column: */
            c.Add(o => o.Product.ProductName)
                .Titled("ProdName")
                .SetCrudHidden(true)
                .SetWidth(250);

            /* Adding "Quantity" column: */
            c.Add(o => o.Quantity)
                .Titled("Quant")
                .SetCellCssClassesContraint(o => o.Quantity >= 50 ? "red" : "")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "UnitPrice" column: */
            c.Add(o => o.UnitPrice)
                .Titled("Unit Price")
                .SetWidth(100)
                .Format("{0:F}");

            /* Adding "Discount" column: */
            c.Add(o => o.Discount)
                .Titled("Disc")
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

        public static Action<IGridColumnCollection<Employee>> EmployeeColumns = c =>
        {
            c.Add(o => o.EmployeeID).Titled(SharedResource.Number).SetWidth(100);
            c.Add(o => o.FirstName).SetWidth(250);
            c.Add(o => o.LastName).SetWidth(250);
            c.Add(o => o.HireDate).SetWidth(120);
            c.Add().Encoded(false).Sanitized(false).SetWidth("5%")
                .RenderValueAs(o => $"<img width='50' height='50' src='data:image/bmp;base64,{o.Base64String}' />");
        };

        public static Action<IGridColumnCollection<Customer>> CustomerColumns = c =>
        {
            c.Add(o => o.CustomerID).Titled(SharedResource.Number).SetWidth(100);
            c.Add(o => o.CompanyName).SetWidth(250);
            c.Add(o => o.ContactName).SetWidth(250);
            c.Add(o => o.Phone);
            c.Add(o => o.Address).SetWidth(250);
            c.Add(o => o.City).SetWidth(250);
            c.Add(o => o.PostalCode);
            c.Add(o => o.Country).SetWidth(250);
        };
    }
}
