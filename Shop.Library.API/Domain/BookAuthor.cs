namespace Shop.Library.API.Domain
{
    public class BookAuthor
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
