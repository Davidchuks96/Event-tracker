using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class AddedStatusAndMoreProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveOrRejectComments",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ApproveOrRejectComments",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Items");
        }
    }
}
