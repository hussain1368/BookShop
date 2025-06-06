namespace Shop.Library.API.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string Name { get; set; }
    }
}
