using Microsoft.IdentityModel.Tokens;

namespace LibrarianWorkspace.Dto;

public class BookDto
{
    private const int MinPublicationYear = 1500;
    private const int MaxPublicationYear = 2023;
    
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public string? Article { get; set; }
    public int PublicationYear { get; set; }
    public int Quantity { get; set; }

    public bool IsValid()
    {
        if (Name.IsNullOrEmpty() || Author.IsNullOrEmpty() || Article.IsNullOrEmpty())
        {
            return false;
        }
        if (PublicationYear < MinPublicationYear || PublicationYear > MaxPublicationYear)
        {
            return false;
        }
        return Quantity >= 0;
    }
}