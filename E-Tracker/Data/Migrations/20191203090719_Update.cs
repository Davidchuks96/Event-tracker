using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Departments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Departments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ItemTypeId = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<string>(nullable: true),
                    ExpiredDate = table.Column<DateTime>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    TagNo = table.Column<string>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    ReoccurenceValue = table.Column<int>(nullable: false),
                    ReoccurenceFrequency = table.Column<int>(nullable: false),
                    NotificationPeriod = table.Column<int>(nullable: false),
                    NotificationFrequency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ApprovedById",
                table: "Items",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DepartmentId",
                table: "Items",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetUsers");
        }
    }
}
