using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserPaymentDetails",
                keyColumn: "Id",
                keyValue: "2",
                column: "StripeCustomerId",
                value: "cus_999");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserPaymentDetails",
                keyColumn: "Id",
                keyValue: "2",
                column: "StripeCustomerId",
                value: "cus_123");
        }
    }
}
