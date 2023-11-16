using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarianWorkspace.Models;

public class Reader
{
    [Key] public int Id { get; set; }
    [Required] public string FullName { get; set; } = null!;
    [Required, DataType(DataType.Date)] public DateTime Birthday { get; set; }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}