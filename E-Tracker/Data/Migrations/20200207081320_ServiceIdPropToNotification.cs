using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class ServiceIdPropToNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ServiceId",
                table: "Notifications",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Services_ServiceId",
                table: "Notifications",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Services_ServiceId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ServiceId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Notifications");
        }
    }
}
