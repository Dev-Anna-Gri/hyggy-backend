using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hyggy_backend.Models.BlogModels
{
    public class BlogSubcategory
    {
        public int Id { get; set; }
        public int BlogCategoryId { get; set; }
        public string Name { get; set; }
    }
}
