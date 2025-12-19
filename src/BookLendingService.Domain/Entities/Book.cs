namespace BookLendingService.Domain.Entities;

public sealed class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Title { get; set; }

    public required string Author { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTimeOffset? CheckedOutAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public byte[]? RowVersion { get; set; }
}
