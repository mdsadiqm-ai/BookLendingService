using BookLendingService.Domain.Entities;
using BookLendingService.Infrastructure.Interfaces;
using BookLendingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLendingService.Infrastructure.Repositories;

public sealed class EfBookRepository : IBookRepository
{
    private readonly BooksDbContext _db;

    public EfBookRepository(BooksDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Book book, CancellationToken ct)
    {
        await _db.Books.AddAsync(book, ct);
    }

    public async Task<IReadOnlyList<Book>> ListAsync(CancellationToken ct)
    {
        return await _db.Books
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync(ct);
    }

    public async Task<Book?> GetAsync(Guid id, CancellationToken ct)
    {
        return await _db.Books.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }

    public void SaveChanges()
    {
        _db.SaveChanges();
    }
}
