using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class eNotificationProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "Notifications",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Notifications");

        }
    }
}
