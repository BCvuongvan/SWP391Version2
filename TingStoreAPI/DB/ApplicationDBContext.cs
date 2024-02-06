using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TingStoreAPI.Models;

namespace TingStoreAPI.DB
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Cart> carts { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<OrderStatus> orderStatuses { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Feedback> feedbacks { get; set; }
        public DbSet<DiscountPercent> discountPercents { get; set; }
        public DbSet<Category> categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure composite primary key using HasKey method            
            modelBuilder.Entity<OrderDetail>().HasKey(s => new
            { s.orderId, s.proId });
            modelBuilder.Entity<Feedback>().HasKey(s => new
            { s.userName, s.proId });
            modelBuilder.Entity<Cart>().HasKey(s => new
            { s.userName, s.proId });

            modelBuilder.Entity<Category>().HasData(
                new Category { cateId = 1, cateName = "Iphone", cateDescribe = "The best of phone", cateStatus = true },
                new Category { cateId = 2, cateName = "Samsung", cateDescribe = "The best of phone", cateStatus = true }
            );

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { orderStatusId = 1, statusName = "Spending" },
                new OrderStatus { orderStatusId = 2, statusName = "Successful" }
            );
            // Chỉ định kiểu cột và tỷ lệ cho TotalAmount
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)"); // Điều chỉnh tỷ lệ và tỷ lệ theo nhu cầu
            modelBuilder.Entity<Product>()
                .Property(o => o.proPrice)
                .HasColumnType("decimal(18, 2)"); // Điều chỉnh tỷ lệ và tỷ lệ theo nhu cầu
            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.subTotal)
                .HasColumnType("decimal(18, 2)"); // Điều chỉnh tỷ lệ và tỷ lệ theo nhu cầu

            // Thêm các cấu hình khác nếu cần thiết

            base.OnModelCreating(modelBuilder);
        }

    }
}