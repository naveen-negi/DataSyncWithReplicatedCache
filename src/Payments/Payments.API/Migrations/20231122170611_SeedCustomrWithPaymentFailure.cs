using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomrWithPaymentFailure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserPaymentDetails",
                columns: new[] { "Id", "StripeCustomerId", "UserId" },
                values: new object[] { "2", "cus_123", "9999" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserPaymentDetails",
                keyColumn: "Id",
                keyValue: "2");
        }
    }
}
