namespace Shop.Shared.Models.ServiceBus
{
    public record BookUpdated
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
