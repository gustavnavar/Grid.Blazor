﻿using GridBlazorClientSide.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GridBlazorClientSide.Server.Models
{
    public class NorthwindDbContext : DbContext
    {

        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(c => new { c.OrderID, c.ProductID });

            modelBuilder.Entity<EmployeeTerritories>()
                .HasKey(r => new { r.EmployeeID, r.TerritoryID });

            modelBuilder.Entity<EmployeeTerritories>()
                .HasOne(r => r.Employee)
                .WithMany(s => s.Territories)
                .HasForeignKey(t => t.EmployeeID);

            modelBuilder.Entity<EmployeeTerritories>()
                .HasOne(r => r.Territory)
                .WithMany(s => s.Employees)
                .HasForeignKey(t => t.TerritoryID);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Territory> Territories { get; set; }

    }
}