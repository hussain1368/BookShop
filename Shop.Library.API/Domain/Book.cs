﻿namespace Shop.Library.API.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Description { get; set; }
        public int PurchaseCount { get; set; }
    }
}
