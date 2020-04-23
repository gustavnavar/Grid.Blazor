using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace GridBlazorOData.Server.Models
{
    public class EdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Employee>("Employees");
            builder.EntityType<Employee>().Ignore(r => r.Photo);
            builder.EntityType<Employee>().Property(r => r.Base64String);
            builder.EntitySet<Shipper>("Shippers");
            builder.EntitySet<Product>("Products");
            return builder.GetEdmModel();
        }
    }
}
