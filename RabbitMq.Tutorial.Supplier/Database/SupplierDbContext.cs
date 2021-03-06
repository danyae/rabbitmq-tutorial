using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RabbitMq.Tutorial.Supplier.Database
{
    public class SupplierDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OrderLine> OrderLine { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<WarehouseStockBalance> WarehouseStockBalance { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=supplier.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new List<Product>()
            {
                new Product {Id = Guid.Parse("E90581B2-3A0A-4DEA-9F4F-2DA6B2A60938"), Name = "Mars", Price = 10},
                new Product {Id = Guid.Parse("A7ADEDC9-7E43-4CE9-A932-FA10A42ECA5C"), Name = "Snikers", Price = 11},
                new Product {Id = Guid.Parse("49A226E0-3991-4B09-BF11-AF3DFAE6D0D2"), Name = "Twix", Price = 12},
                new Product {Id = Guid.Parse("1CD5A63F-392E-4429-91B2-7CA57DAA11DA"), Name = "Milky Way", Price = 9},
                new Product {Id = Guid.Parse("D5CFC5F7-2034-4430-8A42-34BD1660491F"), Name = "Alenka", Price = 9},
            });

            modelBuilder.Entity<Store>().HasData(new List<Store>()
            {
                new Store {Id = Guid.Parse("82B06D3E-90FC-4409-820B-B5383B5173E4"), Name = "Hrusha's Goods"},
                new Store {Id = Guid.Parse("531317C9-5457-40C5-8EF3-6358A38587DF"), Name = "Stepasha's Store"},
                new Store {Id = Guid.Parse("321B19B5-BB14-419D-B429-DB8EA0C4CD50"), Name = "Karkushas's Marketplace"},
            });

            modelBuilder.Entity<WarehouseStockBalance>().HasData(new List<WarehouseStockBalance>()
            {
                new WarehouseStockBalance(){Id = Guid.Parse("C910765C-0CA5-475C-BB78-E190765A696F"), ProductId = Guid.Parse("E90581B2-3A0A-4DEA-9F4F-2DA6B2A60938"), Quantity = 1000},
                new WarehouseStockBalance(){Id = Guid.Parse("D3F6CDC3-BDBF-4FB9-A1B8-88D09B96A4BF"), ProductId = Guid.Parse("A7ADEDC9-7E43-4CE9-A932-FA10A42ECA5C"), Quantity = 1000},
                new WarehouseStockBalance(){Id = Guid.Parse("8D393AED-098D-4754-9184-EE91F0CD9336"), ProductId = Guid.Parse("49A226E0-3991-4B09-BF11-AF3DFAE6D0D2"), Quantity = 1000},
                new WarehouseStockBalance(){Id = Guid.Parse("49B85379-2342-4F54-B79E-590481CF4143"), ProductId = Guid.Parse("1CD5A63F-392E-4429-91B2-7CA57DAA11DA"), Quantity = 1000},
                new WarehouseStockBalance(){Id = Guid.Parse("3481DD5E-9B1F-4CDC-99D8-48AD89E8654D"), ProductId = Guid.Parse("D5CFC5F7-2034-4430-8A42-34BD1660491F"), Quantity = 1000},
            });
        }
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderLine
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }

    public class Order
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }

        public Store Store { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }

    public class WarehouseStockBalance
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public Product Product { get; set; }
    }
}
