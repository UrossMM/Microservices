using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // api/v1/Catalog
    public class CatalogController: ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            this._repository = repository;
            this.logger = logger;
        }

        [HttpGet] //   /api/v1/Catalog
        [ProducesResponseType(typeof(IEnumerable<Product>),(int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() 
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name ="GetProduct")] //   /api/v1/Catalog/{id}
        //moze da se desi da naredna fja vrati listu proizvoda i odgovor 200 od servera ili da ne nadje nijedan proizvod
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)] //ako nadje proizvode i onda vraca i 200
        [ProducesResponseType( (int) HttpStatusCode.NotFound)]//ako ne nadje nijedan proizvod
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if(product == null)
            {
                logger.LogError($"Product with id:{id}, not found.");
                return NotFound();  
            }
            return Ok(product); 
        }

        [Route("[action]/{category}", Name ="GetProductByCategory")] //   /api/v1/Catalog/GetProductByCategory/{category}
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products= await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost] //   /api/v1/Catalog
        [ProducesResponseType(typeof(Product),(int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product); // pozvace se fja koja ima u Name GetProduct i prosledice joj se id
            //a to je fja GetProductById(string id) ???
        }

        [HttpPut]  //   /api/v1/Catalog
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product) //IActionResult jer ne vracamo nikakav specifican odgovor, vraca samo 200 
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}",Name ="DeleteProduct")]  //    /api/v1/Catalog/{id}
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)] //da li ovde treba typeof Product ili typeof void ?
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}
