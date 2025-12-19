using BookLendingService.Application.DTOs;

namespace BookLendingService.Application.Interfaces;

public interface IBookService
{
    Task<BookResponse> AddAsync(BookCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken ct);
    Task<BookResponse> CheckoutAsync(Guid id, CancellationToken ct);
    Task<BookResponse> ReturnAsync(Guid id, CancellationToken ct);
}
