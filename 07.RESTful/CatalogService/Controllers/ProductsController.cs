using CatalogService.Data;
using CatalogService.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _service;

        public ProductsController(IProductsService service)
        {
            _service = service;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public Task<IEnumerable<Product>> Get(int categoryId, int page, int pageSize, CancellationToken token)
        {
            return _service.GetCategoryProductsAsync(categoryId, page, pageSize, token);
        }

        // GET api/<ProductsController>/5
        [ProducesResponseType(200)]
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

        // POST api/<ProductsController>
        [ProducesResponseType(typeof(int), 200)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product value, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(value, token);
            return Ok(result);
        }

        // PUT api/<ProductsController>/5
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product value, CancellationToken token)
        {
            var item = await _service.GetAsync(id, token);
            if (item == null)
            {
                return NotFound(id);
            }

            await _service.UpdateAsync(item, value, token);
            return Ok(item);
        }

        // DELETE api/<ProductsController>/5
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

            return Ok(item);
        }
    }
}
