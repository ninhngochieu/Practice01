using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;
using Practice01.Domain.Entities.Books;

namespace Practice01.Application.TestBook;

public class CreateBookCommand : IRequest<Guid>
{
    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!; 
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly ErrorCollector _errorCollector;

    public CreateBookCommandHandler(IBookRepository bookRepository, ErrorCollector errorCollector)
    {
        _bookRepository = bookRepository;
        _errorCollector = errorCollector;
    }
    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(request.BookName, request.Price, request.Category, request.Author);
        var id = await _bookRepository.Add(book);
        
        _errorCollector.Success(HttpStatusCode.Created, "CREATE_BOOK_SUCCESS", "Create Book Success");
        return id;
    }
}