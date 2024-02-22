using Microsoft.EntityFrameworkCore.Migrations;

namespace TingStoreAPI.Migrations
{
    public partial class MySecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "proStatus",
                table: "products",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "imageStatus",
                table: "productImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "discountImage",
                table: "discountPercents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageStatus",
                table: "productImages");

            migrationBuilder.DropColumn(
                name: "discountImage",
                table: "discountPercents");

            migrationBuilder.AlterColumn<bool>(
                name: "proStatus",
                table: "products",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
