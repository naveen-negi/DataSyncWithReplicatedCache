using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangePaymentIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Payments""
      ALTER COLUMN ""PaymentId"" TYPE uuid
      USING (""PaymentId""::text::uuid);");
        }
    }
}
