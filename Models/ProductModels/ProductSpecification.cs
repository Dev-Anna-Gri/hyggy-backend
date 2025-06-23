namespace hyggy_backend.Models.ProductModels
{
    public class ProductSpecification
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Product Product { get; set; }
    }
}
