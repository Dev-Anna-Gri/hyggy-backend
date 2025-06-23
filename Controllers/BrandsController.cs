using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAll() =>
           Ok(await DBService.GetAllBrandsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> Get(int id)
        {
            var item = await DBService.GetBrandByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Brand brand)
        {
            var newId = await DBService.AddBrandAsync(brand);
            brand.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Brand brand)
        {
            if (id != brand.Id) return BadRequest();
            var updated = await DBService.UpdateBrandAsync(brand);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteBrandAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
