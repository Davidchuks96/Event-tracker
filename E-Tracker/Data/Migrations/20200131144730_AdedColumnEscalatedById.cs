using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Tracker.Data.Migrations
{
    public partial class AdedColumnEscalatedById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
                name: "EscalatedById",
                table: "Reminders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalation",
                table: "Reminders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalatedById",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsEscalation",
                table: "Reminders");

           
        }
    }
}
