using MongoDB.Driver;

namespace Practice01.Infrastructure.Data.MongoDb.Books;

public abstract class MongoRepository<T>
{
    public IMongoCollection<T> Collection { get; set; }
    public IMongoDatabase MongoDatabase { get; set; }

    protected MongoRepository(IMongoDatabase mongoDatabase, string collectionName)
    {
        MongoDatabase = mongoDatabase;
        Collection = mongoDatabase.GetCollection<T>(collectionName);
    }
}