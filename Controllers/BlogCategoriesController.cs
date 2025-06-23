using hyggy_backend.Models.BlogModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogCategory>>> GetAll() =>
            Ok(await DBService.GetAllBlogCategoriesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogCategory>> Get(int id)
        {
            var item = await DBService.GetBlogCategoryByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BlogCategory cat)
        {
            var newId = await DBService.AddBlogCategoryAsync(cat);
            cat.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = cat.Id }, cat);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] BlogCategory cat)
        {
            if (id != cat.Id) return BadRequest();
            var updated = await DBService.UpdateBlogCategoryAsync(cat);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteBlogCategoryAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
