using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetAll() =>
            Ok(await DBService.GetAllStoresAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> Get(int id)
        {
            var item = await DBService.GetStoreByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Store store)
        {
            var newId = await DBService.AddStoreAsync(store);
            store.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Store store)
        {
            if (id != store.Id) return BadRequest();
            var updated = await DBService.UpdateStoreAsync(store);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteStoreAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
