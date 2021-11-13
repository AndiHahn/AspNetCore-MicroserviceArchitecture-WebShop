using Microsoft.EntityFrameworkCore.Migrations;

namespace Webshop.Services.Catalog.Api.Infrastructure.Migrations
{
    public partial class AddPictureUri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUri",
                table: "CatalogItem",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUri",
                table: "CatalogItem");
        }
    }
}
