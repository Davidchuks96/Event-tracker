using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class IsFirstLoginProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstLogin",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.DropColumn(
                name: "IsFirstLogin",
                table: "AspNetUsers");
            
        }
    }
}
