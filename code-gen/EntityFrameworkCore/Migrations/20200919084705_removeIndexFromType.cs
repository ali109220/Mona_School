using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkCore.Migrations
{
    public partial class removeIndexFromType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_Brands_BrandId",
                table: "ProductTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_LicenceTypes_LicenceTypeId",
                table: "ProductTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_Platforms_PlatformId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_BrandId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_LicenceTypeId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_PlatformId",
                table: "ProductTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_BrandId",
                table: "ProductTypes",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_LicenceTypeId",
                table: "ProductTypes",
                column: "LicenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_PlatformId",
                table: "ProductTypes",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_Brands_BrandId",
                table: "ProductTypes",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_LicenceTypes_LicenceTypeId",
                table: "ProductTypes",
                column: "LicenceTypeId",
                principalTable: "LicenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_Platforms_PlatformId",
                table: "ProductTypes",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_Brands_BrandId",
                table: "ProductTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_LicenceTypes_LicenceTypeId",
                table: "ProductTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_Platforms_PlatformId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_BrandId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_LicenceTypeId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_PlatformId",
                table: "ProductTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_BrandId",
                table: "ProductTypes",
                column: "BrandId",
                unique: true,
                filter: "[BrandId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_LicenceTypeId",
                table: "ProductTypes",
                column: "LicenceTypeId",
                unique: true,
                filter: "[LicenceTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_PlatformId",
                table: "ProductTypes",
                column: "PlatformId",
                unique: true,
                filter: "[PlatformId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_Brands_BrandId",
                table: "ProductTypes",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_LicenceTypes_LicenceTypeId",
                table: "ProductTypes",
                column: "LicenceTypeId",
                principalTable: "LicenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_Platforms_PlatformId",
                table: "ProductTypes",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
