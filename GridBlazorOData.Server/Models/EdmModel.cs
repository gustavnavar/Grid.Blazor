using GridBlazorOData.Shared.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace GridBlazorOData.Server.Models
{
    public class EdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<Order>("OrdersWithErrors");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Employee>("Employees");
            builder.EntityType<Employee>().Ignore(r => r.Photo);
            builder.EntityType<Employee>().Property(r => r.Base64String);
            builder.EntitySet<Shipper>("Shippers");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Truck>("Trucks");
            return builder.GetEdmModel();
        }
    }
}
