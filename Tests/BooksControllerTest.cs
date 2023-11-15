using LibrarianWorkspace.Data;
using LibrarianWorkspace.Models;
using LibrarianWorkspace.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using LibrarianWorkspace.Dto;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using MockQueryable.Moq;
using Newtonsoft.Json.Linq;

namespace LibrarianWorkspace.Tests;

public class BooksControllerTest
{
    private List<Book> _data;
    private Mock<DbSet<Book>> _setMock;
    private Mock<LibrarianWorkspaceContext> _ctxMock;
    private readonly ITestOutputHelper _testOutputHelper;
    
    // public BooksControllerTest()
    // {
    //     _data = GetFakeBookList();
    //     _setMock = _data.AsQueryable().BuildMockDbSet();
    //     _ctxMock = new Mock<LibrarianWorkspaceContext>();
    //     _setMock.Setup(x => x.FindAsync(_data[0].Id)).ReturnsAsync(_data[0]);
    //     _setMock.Setup(x => x.AddAsync(It.IsAny<Book>(),It.IsAny<CancellationToken>())).Callback((Book model, CancellationToken token)=> {_data.Add(model);});
    //     _ctxMock.Setup(x => x.Book).Returns(_setMock.Object);
    // }
    
    public BooksControllerTest(ITestOutputHelper testOutputHelper)
    {
        _data = GetFakeBookList();
        _setMock = _data.AsQueryable().BuildMockDbSet();
        _ctxMock = new Mock<LibrarianWorkspaceContext>();
        _setMock.Setup(x => x.FindAsync(_data[0].Id)).ReturnsAsync(_data[0]);
        _setMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .Callback((Book model, CancellationToken token) => { _data.Add(model); });
        _ctxMock.Setup(x => x.Book).Returns(_setMock.Object);
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void GetBook()
    {
        var controller = new BooksController(_ctxMock.Object);
        const int bookId = 1;
        var jsonData = (await controller.GetBook(bookId)).ToJson();
        Assert.NotNull(jsonData);
        var resp = JObject.Parse(jsonData!);
        var book = JsonConvert.DeserializeObject<Book>(resp.GetValue("Value")!.ToString());
        Assert.Equivalent(_data[0], book);
    }

    [Fact]
    public async void CreateBook()
    {
        var controller = new BooksController(_ctxMock.Object);
        var dto = new BookDto()
        {
            Name = "added book",
            Article = "AAAAAA",
            Author = "meowich",
            PublicationYear = 2017,
            Quantity = 0
        };
        await controller.CreateBook(dto);
        const int expBookId = 3;
        var jsonData = ( await controller.CreateBook(dto)).ToJson();
        Assert.NotNull(jsonData);
        Assert.Equal(expBookId, _data.Count);
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