using hyggy_backend.Models.BlogModels;
using hyggy_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hyggy_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogSubcategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogSubcategory>>> GetAll() =>
            Ok(await DBService.GetAllBlogSubcategoriesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogSubcategory>> Get(int id)
        {
            var item = await DBService.GetBlogSubcategoryByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BlogSubcategory sub)
        {
            var newId = await DBService.AddBlogSubcategoryAsync(sub);
            sub.Id = newId;
            return CreatedAtAction(nameof(Get), new { id = sub.Id }, sub);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] BlogSubcategory sub)
        {
            if (id != sub.Id) return BadRequest();
            var updated = await DBService.UpdateBlogSubcategoryAsync(sub);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await DBService.DeleteBlogSubcategoryAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }
    }
}
