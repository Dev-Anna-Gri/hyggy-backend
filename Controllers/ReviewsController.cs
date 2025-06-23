using hyggy_backend.Models.ProductModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAll() =>
            Ok(await DBService.GetAllReviewsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> Get(int id)
        {
            var item = await DBService.GetReviewByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Review rev)
        {
            var newId = await DBService.AddReviewAsync(rev);
            rev.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = rev.Id }, rev);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Review rev)
        {
            if (id != rev.Id) return BadRequest();
            var updated = await DBService.UpdateReviewAsync(rev);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteReviewAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
