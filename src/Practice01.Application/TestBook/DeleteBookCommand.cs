using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;
using Practice01.Domain.Entities.Books;

namespace Practice01.Application.TestBook;

public class DeleteBookCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly ErrorCollector _errorCollector;

    public DeleteBookCommandHandler(IBookRepository bookRepository, ErrorCollector errorCollector)
    {
        _bookRepository = bookRepository;
        _errorCollector = errorCollector;
    }
    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var bookExists = await _bookRepository.ExistsAsync(request.Id);
        if (!bookExists)
        {
            _errorCollector.Error(HttpStatusCode.NotFound, "BOOK_NOT_FOUND", "Book not found");
            return;
        }
        await _bookRepository.DeleteAsync(request.Id);
        _errorCollector.Success(HttpStatusCode.NoContent, "DELETE_BOOK_SUCCESS", "Delete Book Success");
    }
}