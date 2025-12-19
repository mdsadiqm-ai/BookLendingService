using BookLendingService.Api.Middleware;
using BookLendingService.Application.Interfaces;
using BookLendingService.Application.Services;
using BookLendingService.Infrastructure.Interfaces;
using BookLendingService.Infrastructure.Repositories;

namespace BookLendingService.Api.ServiceExtensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, EfBookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<ExceptionHandlingMiddleware>();
        return services;
    }
}