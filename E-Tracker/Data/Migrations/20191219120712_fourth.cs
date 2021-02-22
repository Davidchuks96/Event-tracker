using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationUsers",
                columns: table => new
                {
                    NotificationId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUsers", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_NotificationUsers_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUsers_NotificationId",
                table: "NotificationUsers",
                column: "NotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationUsers");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "AspNetRoles");
        }
    }
}
