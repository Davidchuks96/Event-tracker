using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class HasDataItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: "0855f548-6fca-40f7-ac60-a6de7aea4d78");

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: "4cf4e6d8-2bb7-4b98-944e-6c28eee4b11a");

         

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "CreatedById", "DateCreated", "DateDeleted", "DateUpdated", "DeletedById", "IsActive", "Name", "UpdatedById" },
                values: new object[] { "d421e6d6-fd6f-4c2e-8646-15ca24945787", null, new DateTime(2020, 1, 24, 12, 36, 31, 415, DateTimeKind.Local).AddTicks(4881), null, null, null, true, "Renewal", null });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "CreatedById", "DateCreated", "DateDeleted", "DateUpdated", "DeletedById", "IsActive", "Name", "UpdatedById" },
                values: new object[] { "38dbc747-6243-4a9b-9fc7-e22d6544ed78", null, new DateTime(2020, 1, 24, 12, 36, 31, 416, DateTimeKind.Local).AddTicks(9969), null, null, null, true, "Servicing", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: "38dbc747-6243-4a9b-9fc7-e22d6544ed78");

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: "d421e6d6-fd6f-4c2e-8646-15ca24945787");


            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "CreatedById", "DateCreated", "DateDeleted", "DateUpdated", "DeletedById", "IsActive", "Name", "UpdatedById" },
                values: new object[] { "4cf4e6d8-2bb7-4b98-944e-6c28eee4b11a", null, new DateTime(2020, 1, 24, 12, 35, 2, 165, DateTimeKind.Local).AddTicks(3905), null, null, null, true, "Renewal", null });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "CreatedById", "DateCreated", "DateDeleted", "DateUpdated", "DeletedById", "IsActive", "Name", "UpdatedById" },
                values: new object[] { "0855f548-6fca-40f7-ac60-a6de7aea4d78", null, new DateTime(2020, 1, 24, 12, 35, 2, 165, DateTimeKind.Local).AddTicks(5995), null, null, null, true, "Servicing", null });
        }
    }
}
