using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice01.Application.TestBook;
using Practice01.Presentation.Common.ObjectResult;


namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestMongoDbController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CustomObjectResult _objectResult;

    public TestMongoDbController(IMediator mediator, CustomObjectResult objectResult)
    {
        _mediator = mediator;
        _objectResult = objectResult;
    }

    [HttpGet]
    public async Task<IResult> GetBooks()
    {
        var books = await _mediator.Send(new GetBooksQuery());
        return _objectResult.Return(books);
    }

    [HttpPost]
    public async Task<IResult> CreateBook([FromBody] CreateBookCommand createBookCommand)
    {
        var result = await _mediator.Send(createBookCommand);
        return _objectResult.Return(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateBook(Guid id, [FromBody] UpdateBookCommand updateBookCommand)
    {
        updateBookCommand.Id = id;
        
        await _mediator.Send(updateBookCommand);
        return _objectResult.Return();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteBook(Guid id)
    {
        await _mediator.Send(new DeleteBookCommand { Id = id });
        return _objectResult.Return();
    }
}