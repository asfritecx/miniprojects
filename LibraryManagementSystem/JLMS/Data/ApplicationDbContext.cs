using JLMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JLMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BooksExtendedInformation> BooksExtendedInformation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.BookISBN13)
                .IsUnique();

            modelBuilder.Entity<BooksExtendedInformation>()
                .HasKey(b => b.BookISBN13);

            modelBuilder.Entity<BooksExtendedInformation>()
                .HasOne(b => b.Book)
                .WithOne(b => b.BooksExtendedInformation)
                .HasForeignKey<BooksExtendedInformation>(b => b.BookISBN13)
                .HasPrincipalKey<Book>(b => b.BookISBN13);
        }
    }
}
