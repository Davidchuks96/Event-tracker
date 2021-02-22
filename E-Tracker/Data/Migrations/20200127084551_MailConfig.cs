using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class MailConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "MailConfigs",
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
                    MailProvider = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ProviderType = table.Column<int>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    FromName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailConfigs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

        }
    }
}
