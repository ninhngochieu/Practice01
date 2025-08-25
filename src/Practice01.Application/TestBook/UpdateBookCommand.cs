using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;
using Practice01.Domain.Entities.Books;

namespace Practice01.Application.TestBook;

public class UpdateBookCommand : IRequest
{
    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;
    public Guid Id { get; set; }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly ErrorCollector _errorCollector;

    public UpdateBookCommandHandler(IBookRepository bookRepository, ErrorCollector errorCollector)
    {
        _bookRepository = bookRepository;
        _errorCollector = errorCollector;
    }

    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id);
        if (book == null)
        {
            _errorCollector.Error(HttpStatusCode.NotFound, "BOOK_NOT_FOUND", "Book not found");
            return;
        }
        
        book.Update(request.BookName, request.Price, request.Category, request.Author);
        await _bookRepository.UpdateAsync(book);
        
        _errorCollector.Success(HttpStatusCode.NoContent, "UPDATE_BOOK_SUCCESS", "Update Book Success");
    }
}