using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HQS.Migrations
{
    /// <inheritdoc />
    public partial class AddWheelchairColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWheelchairAccessible",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWheelchairAccessible",
                table: "Hospitals");
        }
    }
}
