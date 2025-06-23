using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll() =>
            Ok(await DBService.GetAllProductsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var item = await DBService.GetProductByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Product prod)
        {
            var newId = await DBService.AddProductAsync(prod);
            prod.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = prod.Id }, prod);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Product prod)
        {
            if (id != prod.Id) return BadRequest();
            var updated = await DBService.UpdateProductAsync(prod);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteProductAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
