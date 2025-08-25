namespace Practice01.Domain.Entities.Books;

public class Book : IBaseEntity
{
    public Guid Id { get; set; }

    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? DeletedBy { get; set; }

    public static Book Create(string requestBookName, decimal requestPrice, string requestCategory, string requestAuthor)
    {
        return new Book
        {
            BookName = requestBookName,
            Price = requestPrice,
            Category = requestCategory,
            Author = requestAuthor
        };
    }

    public static Book Create(Guid requestId ,string requestBookName, decimal requestPrice, string requestCategory, string requestAuthor)
    {
        return new Book
        {
            Id = requestId,
            BookName = requestBookName,
            Price = requestPrice,
            Category = requestCategory,
            Author = requestAuthor
        };
    }

    public void Update(string requestBookName, decimal requestPrice, string requestCategory, string requestAuthor)
    {
        BookName = requestBookName;
        Price = requestPrice;
        Category = requestCategory;
        Author = requestAuthor;
    }
}