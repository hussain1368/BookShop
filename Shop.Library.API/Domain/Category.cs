namespace Shop.Library.API.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid ParentCategoryId { get; set; }
        public string Name { get; set; }
    }
}
