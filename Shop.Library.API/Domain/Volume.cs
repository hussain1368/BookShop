namespace Shop.Library.API.Domain
{
    public class Volume
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public int Number { get; set; }
        public int PageCount { get; set; }
    }
}
