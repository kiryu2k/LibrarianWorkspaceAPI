using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarianWorkspace.Models;

public class Book
{
    [Key] public int Id { get; set; }
    [Required] public string Name { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
    [Required] public string Article { get; set; } = null!;
    [Required] public int PublicationYear { get; set; }
    [Required] public int Quantity { get; set; }
    [JsonIgnore] public virtual ICollection<Reader> Readers { get; set; } = new List<Reader>();
}