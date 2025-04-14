using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace LibraryManagement.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=MOATSEMIMAM\\SQLEXPRESS01;Database=LMS02;Trusted_Connection=True;Encrypt=False;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Reservations)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookISBN);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.BorrowedBooks)
                .HasForeignKey(b => b.BorrowedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Book>()
                .Property(b => b.ISBN)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .HasDefaultValueSql("NEWID()");
        }
    }
}
