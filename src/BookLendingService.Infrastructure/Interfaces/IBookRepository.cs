using BookLendingService.Domain.Entities;

namespace BookLendingService.Infrastructure.Interfaces;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken ct);
    Task<IReadOnlyList<Book>> ListAsync(CancellationToken ct);
    Task<Book?> GetAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);

    void SaveChanges();
}