using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Npgsql.NameTranslation;

namespace GridShared.Data
{
    public static class SharedDbContextUtils
    {
        public static DbContextOptionsBuilder UseGridBlazorDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.UseNpgsql(new NpgsqlConnectionStringBuilder()
            {
                Host = "localhost",
                Database = "northwind",
                SearchPath = "northwind",
                Username = "postgres",
            }.ToString());
        }
        
        public static void ApplyToModelBuilder(DatabaseFacade databaseFacade, ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName()));
                }

                entity.SetTableName(mapper.TranslateTypeName(entity.GetTableName()));
            }
        }
    }
}