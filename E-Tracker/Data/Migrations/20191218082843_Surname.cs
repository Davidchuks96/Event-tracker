using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class Surname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherNames",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailLogs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    UpdatedById = table.Column<string>(nullable: true),
                    DeletedById = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    To = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    DateSent = table.Column<DateTime>(nullable: false),
                    IsSent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailLogs");

            migrationBuilder.DropColumn(
                name: "OtherNames",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");          
        }
    }
}
