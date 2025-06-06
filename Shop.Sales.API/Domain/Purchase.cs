namespace Shop.Sales.API.Domain
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
