using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentAccounting.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomRetrainingMonths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomRetrainingMonths",
                table: "TrainingPrograms",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomRetrainingMonths",
                table: "TrainingPrograms");
        }
    }
}
