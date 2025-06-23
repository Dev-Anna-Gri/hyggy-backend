using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSubcategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSubcategory>>> GetAll() =>
            Ok(await DBService.GetAllProductSubcategoriesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSubcategory>> Get(int id)
        {
            var item = await DBService.GetProductSubcategoryByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductSubcategory sub)
        {
            var newId = await DBService.AddProductSubcategoryAsync(sub);
            sub.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = sub.Id }, sub);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductSubcategory sub)
        {
            if (id != sub.Id) return BadRequest();
            var updated = await DBService.UpdateProductSubcategoryAsync(sub);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteProductSubcategoryAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
