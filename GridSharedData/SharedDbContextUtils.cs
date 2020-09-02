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
            if (!File.Exists(northwindDb))
            {
                using var resxStream = typeof(SharedDbContextUtils).Assembly.GetManifestResourceStream(
                    $"{typeof(SharedDbContextUtils).Namespace}.northwind.db");
                using var fs = new FileStream(northwindDb, FileMode.Create, FileAccess.ReadWrite);
                resxStream.CopyTo(fs);
            }
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