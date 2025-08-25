namespace Practice01.Domain.Entities.Books;

public interface IBookRepository : IRepository<Book>
{
    Task<Guid> Add(Book book);
    Task<Book?> GetByIdAsync(Guid requestId);
    Task UpdateAsync(Book book);
    Task<List<Book>> GetAllAsync(string requestBookName, decimal requestPrice, string requestCategory,
        string requestAuthor);

    Task DeleteAsync(Guid bookId);
    Task<bool> ExistsAsync(Guid requestId);
}