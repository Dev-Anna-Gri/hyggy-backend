using hyggy_backend.Models.ProductModels.Enums;

namespace hyggy_backend.Models.ProductModels
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageAssetId { get; set; }
        public string SmallDescription { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int SubcategoryId { get; set; }
        public bool IsDiscount { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? DiscountTime { get; set; }
        public string FullDescriptionHTML { get; set; }
        public DateTime AddedAt { get; set; }

        public Asset ImageAsset { get; set; }
        public Brand Brand { get; set; }
        public ProductSubcategory Subcategory { get; set; }
    }
}
