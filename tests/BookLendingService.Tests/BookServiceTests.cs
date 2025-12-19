using BookLendingService.Application.DTOs;
using BookLendingService.Application.Services;
using BookLendingService.Infrastructure.Repositories;
using Xunit;

namespace BookLendingService.Tests;

public sealed class BookServiceTests
{
    [Fact]
    public async Task Add_then_list_returns_book()
    {
        var repo = new InMemoryBookRepository();
        var service = new BookService(repo);

        var created = await service.AddAsync(new BookCreateRequest("Domain Driven Design", "Eric Evans"), CancellationToken.None);
        var list = await service.GetAllAsync(CancellationToken.None);

        Assert.Single(list);
        Assert.Equal(created.Id, list[0].Id);
        Assert.True(list[0].IsAvailable);
    }

    [Fact]
    public async Task Checkout_then_return_changes_availability()
    {
        var repo = new InMemoryBookRepository();
        var service = new BookService(repo);

        var created = await service.AddAsync(new BookCreateRequest("Clean Architecture", "Robert C Martin"), CancellationToken.None);

        var checkedOut = await service.CheckoutAsync(created.Id, CancellationToken.None);
        Assert.False(checkedOut.IsAvailable);

        var returned = await service.ReturnAsync(created.Id, CancellationToken.None);
        Assert.True(returned.IsAvailable);
    }

    [Fact]
    public async Task Checkout_twice_returns_conflict()
    {
        var repo = new InMemoryBookRepository();
        var service = new BookService(repo);

        var created = await service.AddAsync(new BookCreateRequest("Refactoring", "Martin Fowler"), CancellationToken.None);

        await service.CheckoutAsync(created.Id, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CheckoutAsync(created.Id, CancellationToken.None));
    }
}
