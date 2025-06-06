namespace Shop.Shared.Models.ServiceBus
{
    public record PurchaseAdded
    {
        public Guid BookId { get; set; }
        public int PurchaseCount { get; set; }
    }
}
