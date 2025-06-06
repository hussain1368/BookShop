namespace Shop.Library.API.Domain
{
    public class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookCount { get; set; }
    }
}
