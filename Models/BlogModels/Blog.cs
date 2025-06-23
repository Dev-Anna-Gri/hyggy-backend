using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hyggy_backend.Models.BlogModels
{
    public class Blog
    {
        public int Id { get; set; }
        public string ImageAssetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubcategoryId { get; set; }
        public string FullTextHTML { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }

        public Asset ImageAsset { get; set; }
        public BlogSubcategory Subcategory { get; set; }
    }
}
