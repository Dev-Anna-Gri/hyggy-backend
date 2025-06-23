using hyggy_backend.Models.BlogModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAll() =>
           Ok(await DBService.GetAllBlogsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> Get(int id)
        {
            var item = await DBService.GetBlogByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Blog blog)
        {
            var newId = await DBService.AddBlogAsync(blog);
            blog.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = blog.Id }, blog);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Blog blog)
        {
            if (id != blog.Id) return BadRequest();
            var updated = await DBService.UpdateBlogAsync(blog);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteBlogAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
