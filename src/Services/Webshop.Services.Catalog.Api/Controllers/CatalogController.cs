using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Services.Catalog.Api.Infrastructure;
using Webshop.Services.Catalog.Api.Models;

namespace Webshop.Services.Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext context;

        public CatalogController(CatalogContext context)
        {
            this.context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCatalogItems()
        {
            var items = await context.CatalogItem.ToListAsync();
            return Ok(items);
        }
    }
}
