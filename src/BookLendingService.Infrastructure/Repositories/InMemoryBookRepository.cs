using BookLendingService.Domain.Entities;
using BookLendingService.Infrastructure.Interfaces;

namespace BookLendingService.Infrastructure.Repositories;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly Dictionary<Guid, Book> _store = new();

    public Task AddAsync(Book book, CancellationToken ct)
    {
        _store[book.Id] = book;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Book>> ListAsync(CancellationToken ct)
    {
        IReadOnlyList<Book> result = _store.Values
            .OrderBy(x => x.Title)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<Book?> GetAsync(Guid id, CancellationToken ct)
    {
        _store.TryGetValue(id, out var book);
        return Task.FromResult(book);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public void SaveChanges()
    {
        return;
    }
}
