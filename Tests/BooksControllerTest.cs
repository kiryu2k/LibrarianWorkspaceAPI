using LibrarianWorkspace.Data;
using LibrarianWorkspace.Models;
using LibrarianWorkspace.Controllers;
using Moq;
using Moq.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using LibrarianWorkspace.Dto;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Xunit;
using MockQueryable.Moq;
using Newtonsoft.Json.Linq;

namespace LibrarianWorkspace.Tests;

public class BooksControllerTest
{
    private readonly List<Book> _data;
    private readonly Mock<LibrarianWorkspaceContext> _ctxMock;
    
    public BooksControllerTest()
    {
        _data = GetFakeBookList();
        var setMock = _data.AsQueryable().BuildMockDbSet();
        _ctxMock = new Mock<LibrarianWorkspaceContext>();
        setMock.Setup(x => x.FindAsync(_data[0].Id)).ReturnsAsync(_data[0]);
        setMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .Callback((Book model, CancellationToken token) => { _data.Add(model); })
            .ReturnsAsync((Book model, CancellationToken token) => null);
        setMock.Setup(x => x.Remove(It.IsAny<Book>())).Callback<Book>((entity) => _data.Remove(entity));
        _ctxMock.Setup(x => x.Book).Returns(setMock.Object);
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
        Assert.Equal(2, _data.Count);
        var controller = new BooksController(_ctxMock.Object);
        var dto = new BookDto()
        {
            Name = "added book",
            Article = "AAAAAA",
            Author = "meowich",
            PublicationYear = 2017,
            Quantity = 2
        };
        await controller.CreateBook(dto);
        const int expBookCount = 3;
        Assert.Equal(expBookCount, _data.Count);
        Assert.Equal(dto.Name, _data[2].Name);
        Assert.Equal(dto.Article, _data[2].Article);
        Assert.Equal(dto.Author, _data[2].Author);
        Assert.Equal(dto.PublicationYear, _data[2].PublicationYear);
        Assert.Equal(dto.Quantity, _data[2].Quantity);
    }
    
    [Fact]
    public async void DeleteBook()
    {
        var controller = new BooksController(_ctxMock.Object);
        const int bookId = 1;
        await controller.DeleteBook(bookId);
        Assert.Single(_data);
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