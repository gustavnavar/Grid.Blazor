using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.NameTranslation;
using System.IO;

namespace GridShared.Data
{
    public static class SharedDbContextUtils
    {
        public static DbProvider DbProvider = DbProvider.SqlServer;

        public static string ConnectionString = "Server=.;Database=NorthWind;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        public static DbContextOptionsBuilder UseGridBlazorDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            switch (DbProvider)
            {
                case DbProvider.SqlServer:
                    return optionsBuilder.UseSqlServer(ConnectionString);
                case DbProvider.Sqlite:
                default:
                    var northwindDb = "../GridSharedData/northwind.db";
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
        }
        
        public static void ApplyToModelBuilder(DatabaseFacade databaseFacade, ModelBuilder modelBuilder)
        {
            switch (DbProvider)
            {
                case DbProvider.SqlServer:
                    break;
                case DbProvider.Sqlite:
                default:
                    var mapper = new NpgsqlSnakeCaseNameTranslator();
                    foreach (var entity in modelBuilder.Model.GetEntityTypes())
                    {
                        foreach (var property in entity.GetProperties())
                        {
                            var storeObjectIdentifier = StoreObjectIdentifier.Create(property.DeclaringEntityType, StoreObjectType.Table);
                            if(storeObjectIdentifier.HasValue)
                                property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName(storeObjectIdentifier.Value)));
                        }
                    
                        entity.SetTableName(mapper.TranslateTypeName(entity.GetTableName()));
                    }
                    break;
            }
        }
    }

    public enum DbProvider
    {
        SqlServer,
        Sqlite
    }
}