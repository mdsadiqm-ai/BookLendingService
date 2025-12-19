using BookLendingService.Application.DTOs;
using BookLendingService.Application.Interfaces;
using BookLendingService.Domain.Entities;
using BookLendingService.Infrastructure.Interfaces;

namespace BookLendingService.Application.Services;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _repo;

    public BookService(IBookRepository repo)
    {
        _repo = repo;
    }

    public async Task<BookResponse> AddAsync(BookCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Author))
            throw new ArgumentException("Author is required", nameof(request));

        var book = new Book
        {
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            IsAvailable = true,
            CheckedOutAtUtc = null
        };

        await _repo.AddAsync(book, ct);
        await _repo.SaveChangesAsync(ct);

        return Map(book);
    }

    public async Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken ct)
    {
        var books = await _repo.ListAsync(ct);
        return books.Select(Map).ToArray();
    }

    public async Task<BookResponse> CheckoutAsync(Guid id, CancellationToken ct)
    {
        var book = await _repo.GetAsync(id, ct);
        if (book is null)
            throw new KeyNotFoundException("Book not found");
        if (!book.IsAvailable)
            throw new InvalidOperationException("Book is already checked out");

        lock (book) // this is needed if more than one request try to checkout same book at the same time to avoid concurrency exception
        {
            book.IsAvailable = false;
            book.CheckedOutAtUtc = DateTimeOffset.UtcNow;
            _repo.SaveChangesAsync(ct);
        }

        return Map(book);
    }

    public async Task<BookResponse> ReturnAsync(Guid id, CancellationToken ct)
    {
        var book = await _repo.GetAsync(id, ct);
        if (book is null)
            throw new KeyNotFoundException("Book not found");

        if (book.IsAvailable)
            throw new InvalidOperationException("Book is already available");

        book.IsAvailable = true;
        book.CheckedOutAtUtc = null;

        await _repo.SaveChangesAsync(ct);

        return Map(book);
    }

    private static BookResponse Map(Book b) =>
        new BookResponse(b.Id, b.Title, b.Author, b.IsAvailable, b.CheckedOutAtUtc);
}