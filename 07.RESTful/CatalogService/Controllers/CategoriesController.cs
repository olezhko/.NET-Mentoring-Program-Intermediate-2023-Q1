using CatalogService.Data;
using CatalogService.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _service;

        public CategoriesController(ICategoriesService service)
        {
            _service = service;
        }

        // GET: api/<CategoriesController>
        [HttpGet]
        public Task<IEnumerable<Category>> Get(CancellationToken token)
        {
            return _service.GetCategoriesAsync(token);
        }

        // GET api/<CategoriesController>/5
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken token)
        {
            var item = await _service.GetAsync(id, token);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // POST api/<CategoriesController>
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category value, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _service.CreateAsync(value, token));
        }

        // PUT api/<CategoriesController>/5
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Category value, CancellationToken token)
        {
            var item = await _service.GetAsync(id, token);
            if (item == null)
            {
                return NotFound(id);
            }

            if (id != value.CategoryID)
            {
                return BadRequest();
            }

            await _service.UpdateAsync(item, value, token);
            return Ok(item);
        }

        // DELETE api/<CategoriesController>/5
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken token)
        {
            var item = await _service.GetAsync(id, token);
            if (item == null)
            {
                return NotFound(id);
            }

            await _service.DeleteAsync(item, token);
            return Ok();
        }
    }
}