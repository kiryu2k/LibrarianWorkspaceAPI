using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibrarianWorkspace.Data;
var builder = WebApplication.CreateBuilder(args);

// "LibrarianWorkspaceContext": "Server=(localdb)\\mssqllocaldb;Database=LibrarianWorkspaceContext-389b4e76-2c06-44cf-baca-f8bb174e47e9;Trusted_Connection=True;MultipleActiveResultSets=true"

builder.Services.AddDbContext<LibrarianWorkspaceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LibrarianWorkspaceContext") ?? throw new InvalidOperationException("Connection string 'LibrarianWorkspaceContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();