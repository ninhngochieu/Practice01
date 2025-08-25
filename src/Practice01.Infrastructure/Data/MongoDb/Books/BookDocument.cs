using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Practice01.Domain.Entities;
using Practice01.Domain.Entities.Books;

namespace Practice01.Infrastructure.Data.MongoDb.Books;

public class BookDocument : IBaseEntity
{
    public BookDocument(Book book)
    {
        Id = book.Id;
        BookName = book.BookName;
        Price = book.Price;
        Category = book.Category;
        Author = book.Author;
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? DeletedBy { get; set; }

    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;

    public Book ToEntity()
    {
        return Book.Create(Id,BookName, Price, Category, Author);
    }
}