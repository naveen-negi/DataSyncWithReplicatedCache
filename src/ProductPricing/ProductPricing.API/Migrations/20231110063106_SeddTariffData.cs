using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductPricing.API.Migrations
{
    /// <inheritdoc />
    public partial class SeddTariffData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tariffs",
                columns: new[] { "Id", "LocationId", "PricePerHour", "TaxBasisPoints", "ValidFrom", "ValidTo" },
                values: new object[] { new Guid("2aa1e57e-36d0-4057-8403-745378cfdbff"), "1234", 2, 1900, new DateTime(2022, 11, 10, 6, 31, 6, 228, DateTimeKind.Utc).AddTicks(3140), new DateTime(2024, 11, 9, 6, 31, 6, 228, DateTimeKind.Utc).AddTicks(3200) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tariffs",
                keyColumn: "Id",
                keyValue: new Guid("2aa1e57e-36d0-4057-8403-745378cfdbff"));
        }
    }
}
