using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarianWorkspace.Data;
using LibrarianWorkspace.Dto;
using LibrarianWorkspace.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibrarianWorkspace.Controllers;

[Route("api/readers")]
[ApiController]
public class ReadersController : ControllerBase
{
    private readonly LibrarianWorkspaceContext _ctx;

    public ReadersController(LibrarianWorkspaceContext context)
    {
        _ctx = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetReaders([FromQuery] string? name)
    {
        if (name == null)
        {
            return Ok(await _ctx.Reader.Include(e => e.Books).ToListAsync());
        }

        var readers = await _ctx.Reader.Where(e => e.FullName.ToLower().Contains(name.ToLower())).Include(e => e.Books)
            .ToListAsync();
        return Ok(readers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetReader([FromRoute] int id)
    {
        var reader = await _ctx.Reader.Include(e => e.Books).FirstOrDefaultAsync(e => e.Id == id);
        if (reader == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find reader with specified id"
            });
        }

        return Ok(reader);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReader([FromBody] ReaderDto dto)
    {
        if (dto.FullName.IsNullOrEmpty())
        {
            return BadRequest(new ErrResponse()
            {
                Message = "reader's name cannot be empty"
            });
        }

        var reader = new Reader()
        {
            FullName = dto.FullName!,
            Birthday = dto.Birthday.ToDateTime(TimeOnly.MinValue)
        };
        await _ctx.Reader.AddAsync(reader);
        await _ctx.SaveChangesAsync();
        return Ok(new IdResponse() { Id = reader.Id });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReader([FromBody] ReaderDto dto)
    {
        if (dto.Id == null)
        {
            return BadRequest(new ErrResponse()
            {
                Message = "reader's id must be specified for updating"
            });
        }

        var reader = await _ctx.Reader.FindAsync(dto.Id);
        if (reader == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find reader with specified id"
            });
        }

        if (dto.FullName.IsNullOrEmpty())
        {
            return BadRequest(new ErrResponse()
            {
                Message = "reader's name cannot be empty"
            });
        }

        reader.FullName = dto.FullName!;
        reader.Birthday = dto.Birthday.ToDateTime(TimeOnly.MinValue);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReader([FromRoute] int id)
    {
        var reader = await _ctx.Reader.Include(e=>e.Books).FirstOrDefaultAsync(e=>e.Id == id);
        if (reader == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find reader with specified id"
            });
        }
        foreach (var book in reader.Books)
        {
            book.Quantity++;
        }
        _ctx.Reader.Remove(reader);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("addBook")]
    public async Task<IActionResult> AddBook([FromBody] BookReaderDto dto)
    {
        var reader = await _ctx.Reader.Include(e => e.Books).FirstOrDefaultAsync(e => e.Id == dto.ReaderId);
        if (reader == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find reader with specified id"
            });
        }

        if (reader.Books.FirstOrDefault(e => e.Id == dto.BookId) != null)
        {
            return BadRequest(new ErrResponse()
            {
                Message = "this reader already has specified book"
            });
        }

        var book = await _ctx.Book.FindAsync(dto.BookId);
        if (book == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find book with specified id"
            });
        }

        if (book.Quantity == 0)
        {
            return BadRequest(new ErrResponse()
            {
                Message = "specified book is already out of stock"
            });
        }

        reader.Books.Add(book);
        book.Quantity--;
        await _ctx.SaveChangesAsync();
        return Ok(reader);
    }

    [HttpPost("returnBook")]
    public async Task<IActionResult> ReturnBook([FromBody] BookReaderDto dto)
    {
        var reader = await _ctx.Reader.Include(e => e.Books).FirstOrDefaultAsync(e => e.Id == dto.ReaderId);
        if (reader == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find reader with specified id"
            });
        }

        var book = reader.Books.FirstOrDefault(e => e.Id == dto.BookId);
        if (book == null)
        {
            return NotFound(new ErrResponse()
            {
                Message = "cannot find book with specified id from this reader"
            });
        }

        reader.Books.Remove(book);
        book.Quantity++;
        await _ctx.SaveChangesAsync();
        return Ok(reader);
    }
}