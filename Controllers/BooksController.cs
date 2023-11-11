using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarianWorkspace.Data;
using LibrarianWorkspace.Dto;
using LibrarianWorkspace.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibrarianWorkspace.Controllers;

[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private const string IssuedFilter = "issued";
    private const string AvailableFilter = "available";
    
    private readonly LibrarianWorkspaceContext _ctx;

    public BooksController(LibrarianWorkspaceContext context)
    {
        _ctx = context;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetBooks([FromQuery] string? name, [FromQuery] string? filter)
    {
        var books = GetBooksByName(name);
        if (filter.IsNullOrEmpty())
        {
            return Ok(await books.ToListAsync());
        }
        return filter!.ToLower() switch
        {
            IssuedFilter =>  Ok(await books.Include(e => e.Readers).Where(e => e.Readers.Count > 0).ToListAsync()),
            AvailableFilter => Ok(await books.Where(e => e.Quantity > 0).ToListAsync()),
            _ => Ok(await books.ToListAsync())
        };
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBook([FromRoute]int id)
    {
        var book = await _ctx.Book.FindAsync(id);
        if (book == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find book with specified id"
            });
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBook([FromBody]BookDto dto)
    {
        if (!dto.IsValid())
        {
            return BadRequest(new ErrResponse()
            {
                Message = "invalid input data"
            });
        }

        var book = new Book()
        {
            Name = dto.Name!,
            Author = dto.Author!,
            Article = dto.Article!,
            PublicationYear = dto.PublicationYear,
            Quantity = dto.Quantity
        };
        await _ctx.Book.AddAsync(book);
        await _ctx.SaveChangesAsync();
        return Ok(new IdResponse() { Id = book.Id });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBook([FromBody]BookDto dto)
    {
        if (dto.Id == null)
        {
            return BadRequest(new ErrResponse()
            {
                Message = "book's id must be specified for updating"
            });
        }

        var book = await _ctx.Book.FindAsync(dto.Id);
        if (book == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find book with specified id"
            });
        }

        if (!dto.IsValid())
        {
            return BadRequest(new ErrResponse()
            {
                Message = "invalid input data"
            });
        }

        book.Name = dto.Name!;
        book.Author = dto.Author!;
        book.Article = dto.Article!;
        book.PublicationYear = dto.PublicationYear;
        book.Quantity = dto.Quantity;
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook([FromRoute]int id)
    {
        var book = await _ctx.Book.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _ctx.Book.Remove(book);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<Book> GetBooksByName(string? name)
    {
        return (name == null)
            ? _ctx.Book.AsQueryable()
            : _ctx.Book.Where(e => e.Name.ToLower().Contains(name!.ToLower()));
    }
}