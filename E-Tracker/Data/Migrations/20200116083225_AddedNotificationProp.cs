using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class AddedNotificationProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotificationTriggerId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ItemId",
                table: "Notifications",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTriggerId",
                table: "Notifications",
                column: "NotificationTriggerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Items_ItemId",
                table: "Notifications",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_NotificationTriggerId",
                table: "Notifications",
                column: "NotificationTriggerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Items_ItemId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_NotificationTriggerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ItemId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTriggerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationTriggerId",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

           
        }
    }
}
