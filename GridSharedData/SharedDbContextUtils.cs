using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GridShared.Data
{
    public static class SharedDbContextUtils
    {
        public static string ConnectionString = "Server=.\\SQLEXPRESS;Database=NorthWind;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static DbContextOptionsBuilder UseGridBlazorDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        public static void ApplyToModelBuilder(DatabaseFacade databaseFacade, ModelBuilder modelBuilder)
        {
            
        }
    }
}