namespace Shop.Library.API.Domain
{
    public class BookCategory
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
