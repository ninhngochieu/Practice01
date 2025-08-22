using MongoDB.Driver;
using Practice01.Domain.Entities.Books;

namespace Practice01.Infrastructure.Data.MongoDb.Books;

public class BookRepository : MongoRepository<Book>, IBookRepository
{
    public BookRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase, "books")
    {
    }
}