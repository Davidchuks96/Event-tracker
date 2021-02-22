using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class MoreServiceProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsANewCycle",
                table: "Services",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsANewReoccurenceFrequency",
                table: "Services",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewReoccurenceFrequency",
                table: "Services",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewReoccurenceValue",
                table: "Services",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AutoGenServicePeriods",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ItemId = table.Column<string>(nullable: true),
                    PresentExpiryDate = table.Column<DateTime>(nullable: false),
                    ReoccurenceValue = table.Column<int>(nullable: false),
                    ReoccurenceFrequency = table.Column<int>(nullable: false),
                    NextExpiryDate = table.Column<DateTime>(nullable: false),
                    IsServiceDateMet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoGenServicePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoGenServicePeriods_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoGenServicePeriods_ItemId",
                table: "AutoGenServicePeriods",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoGenServicePeriods");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsANewCycle",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsANewReoccurenceFrequency",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NewReoccurenceFrequency",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NewReoccurenceValue",
                table: "Services");

        }
    }
}
