using LibrarianWorkspace.Data;
using LibrarianWorkspace.Models;
using LibrarianWorkspace.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using MockQueryable.Moq;
using Newtonsoft.Json.Linq;

namespace LibrarianWorkspace.Tests;

public class BooksControllerTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BooksControllerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void GetBook()
    {
        var stub = GetFakeBookList();
        var data = stub.AsQueryable().BuildMockDbSet();
        var ctxMock = new Mock<LibrarianWorkspaceContext>();
        // var setMock = new Mock<DbSet<Book>>();
        // setMock.As<IQueryable<Book>>().Setup(x=>x.Provider).Returns(data.Provider);
        // setMock.As<IQueryable<Book>>().Setup(x=>x.ElementType).Returns(data.ElementType);
        // setMock.As<IQueryable<Book>>().Setup(x=>x.Expression).Returns(data.Expression);
        // setMock.As<IQueryable<Book>>().Setup(x=>x.GetEnumerator()).Returns(()=>data.GetEnumerator());
        // ctxMock.Setup(x => x.Book).Returns(setMock.Object);
        data.Setup(x => x.FindAsync(stub[0].Id)).ReturnsAsync(stub[0]);
        ctxMock.Setup(x => x.Book).Returns(data.Object);
        var controller = new BooksController(ctxMock.Object);
        const int bookId = 1;
        var jsonData = (await controller.GetBook(bookId)).ToJson();
        Assert.NotNull(jsonData);
        var resp = JObject.Parse(jsonData!);
        var book = JsonConvert.DeserializeObject<Book>(resp.GetValue("Value")!.ToString());
        Assert.Equivalent(stub[0], book);
    }
    
    private static List<Book> GetFakeBookList()
    {
        return new List<Book>()
        {
            new()
            {
                Id = 1,
                Name = "some book",
                Article = "some article",
                Author = "some author",
                PublicationYear = 2023,
                Quantity = 8
            },
            new()
            {
                Id = 2,
                Name = "book book book",
                Article = "#^#@^#@^",
                Author = "me",
                PublicationYear = 1989,
                Quantity = 0
            },
        };
    }
}