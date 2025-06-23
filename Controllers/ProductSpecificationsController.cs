using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSpecificationsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSpecification>>> GetAll() =>
           Ok(await DBService.GetAllProductSpecificationsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSpecification>> Get(int id)
        {
            var item = await DBService.GetProductSpecificationByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductSpecification spec)
        {
            var newId = await DBService.AddProductSpecificationAsync(spec);
            spec.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = spec.Id }, spec);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductSpecification spec)
        {
            if (id != spec.Id) return BadRequest();
            var updated = await DBService.UpdateProductSpecificationAsync(spec);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteProductSpecificationAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
