using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.NameTranslation;

namespace GridShared.Data
{
    public static class SharedDbContextUtils
    {
        public static DbContextOptionsBuilder UseGridBlazorDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            var northwindDb = "../northwind.db";
            return optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = northwindDb,
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