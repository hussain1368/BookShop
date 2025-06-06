using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Library.API.Migrations
{
    /// <inheritdoc />
    public partial class bookPurchaseCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseCount",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCount",
                table: "Books");
        }
    }
}
