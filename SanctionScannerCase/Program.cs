
using FluentValidation;
using SanctionScannerCase.Application.Books;
using SanctionScannerCase.Application.BorrowedBooks;
using SanctionScannerCase.Application.Contracts.Books;
using SanctionScannerCase.Application.Contracts.BorrowedBook;
using SanctionScannerCase.Application.Validations;
using SanctionScannerCase.Domain.Books;
using SanctionScannerCase.Domain.BorrowedBooks;
using SanctionScannerCase.EntityFrameworkCore;
using SanctionScannerCase.EntityFrameworkCore.Context;
using Serilog;
using Serilog.Core;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBookAppService, BookAppService>();
builder.Services.AddScoped<IBorrowedBookAppService, BorrowedBookAppService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowedBookRepository, BorrowedBookRepository>();
builder.Services.AddMvc();
builder.Services.AddScoped<IValidator<BooksCreateDto>, BookValidator>();
builder.Services.AddScoped<IValidator<BorrowedBookCreateDto>, BorrowedBookValidator>();
// Add services to the container.
builder.Services.AddDbContext<SanctionScannerDbContext>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

Logger log = new LoggerConfiguration()
.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
.MinimumLevel.Override("System", LogEventLevel.Warning)
.Enrich.FromLogContext()
.WriteTo.File("logs/log.log",
rollOnFileSizeLimit: true,
shared: true,
flushToDiskInterval: TimeSpan.FromSeconds(1))
.CreateLogger();

builder.Host.UseSerilog(log);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}");

app.Run();
