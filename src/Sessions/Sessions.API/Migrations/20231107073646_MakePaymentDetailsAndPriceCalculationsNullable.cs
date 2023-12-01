using Microsoft.EntityFrameworkCore.Migrations;
using Sessions.API.Controllers;

#nullable disable

namespace Sessions.API.Migrations
{
    /// <inheritdoc />
    public partial class MakePaymentDetailsAndPriceCalculationsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PricingCalculation>(
                name: "PricingCalculation",
                table: "Sessions",
                type: "json",
                nullable: true,
                oldClrType: typeof(PricingCalculation),
                oldType: "json");

            migrationBuilder.AlterColumn<PaymentDetails>(
                name: "PaymentDetails",
                table: "Sessions",
                type: "json",
                nullable: true,
                oldClrType: typeof(PaymentDetails),
                oldType: "json");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PricingCalculation>(
                name: "PricingCalculation",
                table: "Sessions",
                type: "json",
                nullable: false,
                oldClrType: typeof(PricingCalculation),
                oldType: "json",
                oldNullable: true);

            migrationBuilder.AlterColumn<PaymentDetails>(
                name: "PaymentDetails",
                table: "Sessions",
                type: "json",
                nullable: false,
                oldClrType: typeof(PaymentDetails),
                oldType: "json",
                oldNullable: true);
        }
    }
}
