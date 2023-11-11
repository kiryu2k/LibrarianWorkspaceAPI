using LibrarianWorkspace.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarianWorkspace.Data;

public class LibrarianWorkspaceContext : DbContext
{
    public LibrarianWorkspaceContext(DbContextOptions<LibrarianWorkspaceContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Book { get; set; } = default!;
    public DbSet<Reader> Reader { get; set; } = default!;
}