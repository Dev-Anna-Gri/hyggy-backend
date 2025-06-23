namespace hyggy_backend.Models.ProductModels
{
    public class Warehouse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Amount { get; set; }

        public Product Product { get; set; }
        public Store Store { get; set; }
    }
}
