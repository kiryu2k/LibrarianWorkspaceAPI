using LibrarianWorkspace.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarianWorkspace.Data;

public class LibrarianWorkspaceContext : DbContext
{
    public LibrarianWorkspaceContext(DbContextOptions<LibrarianWorkspaceContext> options)
        : base(options)
    {
    }
    
    public LibrarianWorkspaceContext()
    {
    }

    public virtual DbSet<Book> Book { get; set; }
    public virtual DbSet<Reader> Reader { get; set; }
}