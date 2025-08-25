using MongoDB.Driver;
using Practice01.Domain.Entities.Books;

namespace Practice01.Infrastructure.Data.MongoDb.Books;

public class BookRepository : MongoRepository<BookDocument>, IBookRepository
{
    public BookRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase, "books")
    {
    }

    public async Task<Guid> Add(Book book)
    {
        var bookDocument = new BookDocument(book);
        await Collection.InsertOneAsync(bookDocument);
        
        return bookDocument.Id;
    }

    public async Task<Book?> GetByIdAsync(Guid requestId)
    {
        var filter = Builders<BookDocument>.Filter.Eq(x => x.Id, requestId);
        var result = await Collection.Find(filter).FirstOrDefaultAsync();
        
        return result.ToEntity();
    }

    public Task UpdateAsync(Book book)
    {
        var filter = Builders<BookDocument>.Filter.Eq(x => x.Id, book.Id);
        var update = Builders<BookDocument>.Update
            .Set(x => x.BookName, book.BookName)
            .Set(x => x.Price, book.Price)
            .Set(x => x.Category, book.Category)
            .Set(x => x.Author, book.Author);
        
        return Collection.UpdateOneAsync(filter, update);
    }

    public async Task<List<Book>> GetAllAsync(string requestBookName, decimal requestPrice, string requestCategory,
        string requestAuthor)
    {
        var filter = Builders<BookDocument>.Filter.Empty;
        if (!string.IsNullOrEmpty(requestBookName))
        {
            filter &= Builders<BookDocument>.Filter.Eq(x => x.BookName, requestBookName);
        }
        if (requestPrice != 0)
        {
            filter &= Builders<BookDocument>.Filter.Eq(x => x.Price, requestPrice);
        }
        if (!string.IsNullOrEmpty(requestCategory))
        {
            filter &= Builders<BookDocument>.Filter.Eq(x => x.Category, requestCategory);
        }
        if (!string.IsNullOrEmpty(requestAuthor))
        {
            filter &= Builders<BookDocument>.Filter.Eq(x => x.Author, requestAuthor);
        }
        
        var result = await Collection.Find(filter).ToListAsync();
        return result.Select(x => x.ToEntity()).ToList();
    }

    public Task DeleteAsync(Guid bookId)
    {
        var filter = Builders<BookDocument>.Filter.Eq(x => x.Id, bookId);
        return Collection.DeleteOneAsync(filter);
    }

    public Task<bool> ExistsAsync(Guid requestId)
    {
        var filter = Builders<BookDocument>.Filter.Eq(x => x.Id, requestId);
        return Collection.Find(filter).AnyAsync();
    }
}