using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HQS.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneAndWebsiteColumnsToHospitalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Hospitals");
        }
    }
}
