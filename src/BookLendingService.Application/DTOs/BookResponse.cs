namespace BookLendingService.Application.DTOs;

public sealed record BookResponse(Guid Id, string Title, string Author, bool IsAvailable, DateTimeOffset? CheckedOutAtUtc);
