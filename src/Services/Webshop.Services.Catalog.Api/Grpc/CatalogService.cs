using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Webshop.Services.Catalog.Api.Infrastructure;
using static Catalog;

namespace Webshop.Services.Catalog.Api.Grpc
{
    public class CatalogService : CatalogBase
    {
        private readonly CatalogContext dbContext;
        private readonly ILogger<CatalogService> logger;

        public CatalogService(
            CatalogContext dbContext,
            ILogger<CatalogService> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CatalogItemResponse> GetItemById(
            CatalogItemByIdRequest request,
            ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Id) ||
                !Guid.TryParse(request.Id, out Guid itemId))
            {
                logger.LogError("No catalog item id received.");
                context.Status = new Status(StatusCode.FailedPrecondition, "No catalog item id received.");
                return null;
            }

            var item = await dbContext.CatalogItem.FindAsync(itemId);
            if (item == null)
            {
                logger.LogError($"Catalog item with id {request.Id} not found.");
                context.Status = new Status(StatusCode.NotFound, $"Catalog item with id {request.Id} not found.");
                return null;
            }

            return new CatalogItemResponse
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                AvailableStock = item.AvailableStock,
                PictureUri = item.PictureUri
            };
        }
    }
}
