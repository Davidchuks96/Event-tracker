using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class AddedUpdatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Permissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Permissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ItemTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ItemTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Departments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Departments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Departments");

            
        }
    }
}
