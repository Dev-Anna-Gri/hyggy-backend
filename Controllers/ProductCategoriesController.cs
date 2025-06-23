using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetAll() =>
           Ok(await DBService.GetAllProductCategoriesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> Get(int id)
        {
            var item = await DBService.GetProductCategoryByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductCategory cat)
        {
            var newId = await DBService.AddProductCategoryAsync(cat);
            cat.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = cat.Id }, cat);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductCategory cat)
        {
            if (id != cat.Id) return BadRequest();
            var updated = await DBService.UpdateProductCategoryAsync(cat);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteProductCategoryAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
