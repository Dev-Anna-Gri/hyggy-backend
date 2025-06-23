using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll() =>
           Ok(await DBService.GetAllWarehouseEntriesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> Get(int id)
        {
            var item = await DBService.GetWarehouseByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Warehouse wh)
        {
            var newId = await DBService.AddWarehouseEntryAsync(wh);
            wh.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = wh.Id }, wh);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Warehouse wh)
        {
            if (id != wh.Id) return BadRequest();
            var updated = await DBService.UpdateWarehouseAsync(wh);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteWarehouseAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
