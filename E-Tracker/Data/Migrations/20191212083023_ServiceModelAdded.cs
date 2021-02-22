using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class ServiceModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_ServiceApprovedById",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ServiceApprovedById",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsServiceApproved",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ServiceApprovedById",
                table: "Items");

           

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ServiceDepartmentId = table.Column<string>(nullable: true),
                    ItemId = table.Column<string>(nullable: true),
                    IsServiceApproved = table.Column<bool>(nullable: false),
                    ServiceApprovedById = table.Column<string>(nullable: true),
                    DateServiced = table.Column<DateTime>(nullable: false),
                    NewExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Services_AspNetUsers_ServiceApprovedById",
                        column: x => x.ServiceApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Services_Departments_ServiceDepartmentId",
                        column: x => x.ServiceDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ItemId",
                table: "Services",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceApprovedById",
                table: "Services",
                column: "ServiceApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceDepartmentId",
                table: "Services",
                column: "ServiceDepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceApproved",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServiceApprovedById",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Departments",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ServiceApprovedById",
                table: "Items",
                column: "ServiceApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_ServiceApprovedById",
                table: "Items",
                column: "ServiceApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
