using Microsoft.EntityFrameworkCore;

namespace GridShared.Data
{
    public class SharedDbContext<TContext> : DbContext 
        where TContext : SharedDbContext<TContext>
    {

        public SharedDbContext(DbContextOptions<TContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SharedDbContextUtils.ApplyToModelBuilder(Database, modelBuilder);
        }
    }
}