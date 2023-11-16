using LibrarianWorkspace.Dto;
using Xunit;

namespace LibrarianWorkspace.Tests;

public class BookDtoTest
{
    class TestCase
    {
        public string Name = null!;
        public BookDto Data = null!;
        public bool Exp;
    }
    
    [Fact]
    public void ValidateBook()
    {
        var testCases = new List<TestCase>()
        {
            new()
            {
                Name = "OK",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Article = "AAAAAA",
                    Author = "meowich",
                    PublicationYear = 2017,
                    Quantity = 2
                },
                Exp = true
            },
            new()
            {
                Name = "no name",
                Data = new BookDto()
                {
                    Article = "AAAAAA",
                    Author = "meowich",
                    PublicationYear = 2017,
                    Quantity = 2
                },
                Exp = false
            },
            new()
            {
                Name = "no article",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Author = "meowich",
                    PublicationYear = 2017,
                    Quantity = 2
                },
                Exp = false
            },
            new()
            {
                Name = "no author",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Article = "AAAAAA",
                    PublicationYear = 2017,
                    Quantity = 2
                },
                Exp = false
            },
            new()
            {
                Name = "publication year less than min",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Article = "AAAAAA",
                    Author = "meowich",
                    PublicationYear = 1499,
                    Quantity = 2
                },
                Exp = false
            },
            new()
            {
                Name = "publication year greater than max",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Article = "AAAAAA",
                    Author = "meowich",
                    PublicationYear = 3000,
                    Quantity = 2
                },
                Exp = false
            },
            new()
            {
                Name = "negative quantity",
                Data = new BookDto()
                {
                    Name = "book book book",
                    Article = "AAAAAA",
                    Author = "meowich",
                    PublicationYear = 2000,
                    Quantity = -1
                },
                Exp = false
            },
        };
        foreach (var test in testCases)
        {
            var actual = test.Data.IsValid();
            if (test.Exp) // only assert true/false allows adding test messages
            {
                Assert.True(actual, test.Name);
            }
            else
            {
                Assert.False(actual, test.Name);
            }
        }
    }
}