using LibrarianWorkspace.Data;
using LibrarianWorkspace.Models;
using LibrarianWorkspace.Controllers;
using Moq;
using NuGet.Protocol;
using LibrarianWorkspace.Dto;
using Newtonsoft.Json;
using Xunit;
using MockQueryable.Moq;
using Newtonsoft.Json.Linq;

namespace LibrarianWorkspace.Tests;

public class ReaderControllerTest
{
    private readonly List<Reader> _data;
    private readonly Mock<LibrarianWorkspaceContext> _ctxMock;

    public ReaderControllerTest()
    {
        _data = GetFakeReaderList();
        var setMock = _data.AsQueryable().BuildMockDbSet();
        _ctxMock = new Mock<LibrarianWorkspaceContext>();
        setMock.Setup(x => x.FindAsync(_data[0].Id)).ReturnsAsync(_data[0]);
        setMock.Setup(x => x.AddAsync(It.IsAny<Reader>(), It.IsAny<CancellationToken>()))
            .Callback((Reader model, CancellationToken token) => { _data.Add(model); })
            .ReturnsAsync((Reader model, CancellationToken token) => null);
        setMock.Setup(x => x.Remove(It.IsAny<Reader>())).Callback<Reader>((entity) => _data.Remove(entity));
        _ctxMock.Setup(x => x.Reader).Returns(setMock.Object);
    }
    
    [Fact]
    public async void GetReader()
    {
        var controller = new ReadersController(_ctxMock.Object);
        const int readerId = 2;
        var jsonData = (await controller.GetReader(readerId)).ToJson();
        Assert.NotNull(jsonData);
        var resp = JObject.Parse(jsonData!);
        var reader = JsonConvert.DeserializeObject<Reader>(resp.GetValue("Value")!.ToString());
        Assert.Equal(_data[1].Id, reader!.Id);
        Assert.Equal(_data[1].FullName, reader.FullName);
    }

    [Fact]
    public async void CreateReader()
    {
        Assert.Equal(3, _data.Count);
        var controller = new ReadersController(_ctxMock.Object);
        var dto = new ReaderDto()
        {
            FullName = "added book",
            Birthday = new DateOnly(2005, 12, 1),
        };
        await controller.CreateReader(dto);
        const int expReaderCount = 4;
        Assert.Equal(expReaderCount, _data.Count);
        Assert.Equal(dto.FullName, _data[3].FullName);
        Assert.Equal(dto.Birthday.ToDateTime(TimeOnly.MinValue), _data[3].Birthday);
    }
    
    [Fact]
    public async void DeleteReader()
    {
        var controller = new ReadersController(_ctxMock.Object);
        await controller.DeleteReader(1);
        await controller.DeleteReader(3);
        Assert.Single(_data);
    }
    
    private static List<Reader> GetFakeReaderList()
    {
        return new List<Reader>()
        {
            new()
            {
                Id = 1,
                FullName = "A.A. Brrrr",
                Birthday = DateTime.Parse("10.12.2000")
            },
            new()
            {
                Id = 2,
                FullName = "Z.I. Rooooah",
                Birthday = DateTime.Parse("31.10.2009")
            },
            new()
            {
                Id = 3,
                FullName = "S.S. Absol",
                Birthday = DateTime.Parse("01.01.2019")
            },
        };
    }
}