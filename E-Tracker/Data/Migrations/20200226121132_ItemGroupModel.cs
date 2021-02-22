using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class ItemGroupModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Departments_DepartmentId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_DepartmentId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Services",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Items",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Items",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemGroupId",
                table: "Items",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemGroups",
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
                    Name = table.Column<string>(nullable: true),
                    TagNo = table.Column<string>(nullable: true),
                    CategoryId = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    DateApproved = table.Column<DateTime>(nullable: false),
                    ApprovedById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemGroups_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroups_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemGroupId",
                table: "Items",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroups_ApprovedById",
                table: "ItemGroups",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroups_CategoryId",
                table: "ItemGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroups_DepartmentId",
                table: "ItemGroups",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupId",
                table: "Items",
                column: "ItemGroupId",
                principalTable: "ItemGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ItemGroups");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemGroupId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemGroupId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(120)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Services",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Items",
                type: "nvarchar(120)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproveOrRejectComments",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DepartmentId",
                table: "Items",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Departments_DepartmentId",
                table: "Items",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
