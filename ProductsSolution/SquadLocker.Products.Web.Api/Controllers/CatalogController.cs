using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SquadLocker.Products.Services;
using SquadLocker.Products.Services.Models;
using SquadLocker.Products.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using SquadLocker.Common.Constants;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SquadLocker.Products.Web.Api.Controllers
{
    [Authorize(IdentityConstants.ProductsApiPolicy)]
    [Area(ProductConstants.ControllerAreaProductsApi)]
    [Route("[area]/[controller]")]
    public class CatalogController : BaseApiController
    {
        private readonly ICatalogService _productsService;

        public CatalogController(ICatalogService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("product/{sku}")]
        [ProducesResponseType(typeof(ProductModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var user = User;
            var product = await _productsService.GetBySkuAsync(WebUtility.UrlDecode(sku));

            if (product == null)
            {
                return NotFound($"Product with {nameof(sku)}: {sku} not found");
            }

            return Ok(product);
        }

        [HttpGet("search/{sku}")]
        [ProducesResponseType(typeof(ICollection<ProductModel>), 200)]
        public IActionResult SearchBySku(string sku)
        {
            var products = _productsService.SearchBySku(WebUtility.UrlDecode(sku));

            return Ok(products);
        }
    }
}
