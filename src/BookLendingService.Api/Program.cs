using BookLendingService.Api.Middleware;
using BookLendingService.Api.ServiceExtensions;
using BookLendingService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// we can delete these 3 lines when we setup proper production database ex: in dynamoDb
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "books.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Lending Service API",
        Version = "v1"
    });
});

builder.Services.AddDbContext<BooksDbContext>(opt => opt.UseSqlite(connectionString));
builder.Services.AddApiServices();

builder.Services.AddHealthChecks();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Lending Service API v1");
        c.RoutePrefix = "swagger";
    });
}



app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.MapHealthChecks("/healthz", new HealthCheckOptions());

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
