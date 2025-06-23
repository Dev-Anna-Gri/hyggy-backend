using hyggy_backend.Models;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAll() =>
            Ok(await DBService.GetAllAssetsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> Get(string id)
        {
            var item = await DBService.GetAssetByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Asset asset)
        {
            await DBService.AddAssetAsync(asset);
            return CreatedAtAction(nameof(Get), new { id = asset.Id }, asset);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Asset asset)
        {
            if (id != asset.Id) return BadRequest();
            var updated = await DBService.UpdateAssetAsync(asset);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await DBService.DeleteAssetAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
