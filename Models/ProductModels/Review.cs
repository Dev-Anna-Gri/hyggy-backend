namespace hyggy_backend.Models.ProductModels
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }
    }
}
