using BookLendingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLendingService.Infrastructure.Persistence;

public sealed class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBookEntity(modelBuilder);
    }

    private void ConfigureBookEntity(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Book>();
        b.HasKey(x => x.Id);
        b.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(256);
        b.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(256);
        b.Property(x => x.IsAvailable)
            .IsRequired();
        b.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}