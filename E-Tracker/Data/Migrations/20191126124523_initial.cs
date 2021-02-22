using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetRoleClaims",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_UserId",
                table: "AspNetRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_UserId",
                table: "AspNetRoleClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetUsers_UserId",
                table: "AspNetRoleClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UserId",
                table: "AspNetRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetUsers_UserId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UserId",
                table: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_UserId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoleClaims_UserId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetRoleClaims");
        }
    }
}
