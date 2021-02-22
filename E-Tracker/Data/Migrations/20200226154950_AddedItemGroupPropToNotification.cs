using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class AddedItemGroupPropToNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ItemGroupId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ItemGroupId",
                table: "Notifications",
                column: "ItemGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ItemGroups_ItemGroupId",
                table: "Notifications",
                column: "ItemGroupId",
                principalTable: "ItemGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ItemGroups_ItemGroupId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ItemGroupId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ItemGroupId",
                table: "Notifications");
        }
    }
}
