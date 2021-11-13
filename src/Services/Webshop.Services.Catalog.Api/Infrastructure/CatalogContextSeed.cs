using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webshop.Services.Catalog.Api.Models;

namespace Webshop.Services.Catalog.Api.Infrastructure
{
    public class CatalogContextSeed
    {
        public static async Task SeedAsync(CatalogContext context)
        {
            if (!await context.CatalogItem.AnyAsync())
            {
                await context.CatalogItem.AddRangeAsync(CatalogItems);
                await context.SaveChangesAsync();
            }
        }

        public static IEnumerable<CatalogItem> CatalogItems { get; set; } =
            new List<CatalogItem>
            {
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Raspberry PI",
                    Description = "Raspberry Pi 3 Modell B Plus with 32GB",
                    Price = 79.99,
                    AvailableStock = 5,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/A300/RASP_PI_4_B_01_ANW.png"
                },
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Arduino UNO",
                    Description = "Arduino UNO Rev 3",
                    Price = 22.15,
                    AvailableStock = 3,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/A300/ARDUINO_UNO_01_NEU.png"
                },
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Node MCU",
                    Description = "ESP8266 ESP-12F",
                    Price = 6.04,
                    AvailableStock = 10,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/A300/DEBO_JT_ESP8266_01.png"
                },
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Raspberry PI Case",
                    Description = "Raspberry PI Case 4 2U-BK, black",
                    Price = 10.49,
                    AvailableStock = 5,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/A300/RPI_CASE_2U-BK_01.png"
                },
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "External drive 2TB",
                    Description = "Thshiba Canvio black 2TB",
                    Price = 70.85,
                    AvailableStock = 3,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/E600/CANVIO_GAMING__01.png"
                },
                new CatalogItem
                {
                    Id = Guid.NewGuid(),
                    Name = "RAM DDR3 8GB",
                    Description = "8GB DDR3L 1600 CL11 Crucial",
                    Price = 47.90,
                    AvailableStock = 6,
                    PictureUri = "https://cdn-reichelt.de/bilder/web/xxl_ws/E201/CT25664BA1339_01.png"
                },
            };
    }
}
