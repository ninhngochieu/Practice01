using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;
using Practice01.Domain.Entities.Books;

namespace Practice01.Application.TestBook;

public class GetBooksQuery : IRequest<List<Book>>
{
    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!; 
}

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<Book>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ErrorCollector _errorCollector;

    public GetBooksQueryHandler(IBookRepository bookRepository, ErrorCollector errorCollector)
    {
        _bookRepository = bookRepository;
        _errorCollector = errorCollector;
    }
    public async Task<List<Book>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(request.BookName, request.Price, request.Category, request.Author);
        _errorCollector.Success(HttpStatusCode.OK, "GET_BOOKS_SUCCESS", "Get Books Success");
        return books;
    }
}