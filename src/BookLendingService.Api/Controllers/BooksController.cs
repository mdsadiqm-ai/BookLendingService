using BookLendingService.Application.DTOs;
using BookLendingService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookLendingService.Api.Controllers;

[ApiController]
[Route("books")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService service)
    {
        _bookService = service;
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Add(
        [FromBody] BookCreateRequest request, 
        CancellationToken ct)
    {
        var created = await _bookService.AddAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookResponse>>> GetAll(CancellationToken ct)
    {
        var books = await _bookService.GetAllAsync(ct);
        return Ok(books);
    }

    [HttpPost("{id:guid}/checkout")]
    public async Task<ActionResult<BookResponse>> Checkout(
        Guid id,
        CancellationToken ct)
    {
        var updated = await _bookService.CheckoutAsync(id, ct);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<BookResponse>> Return(
        Guid id,
        CancellationToken ct)
    {
        var updated = await _bookService.ReturnAsync(id, ct);
        return Ok(updated);
    }
}