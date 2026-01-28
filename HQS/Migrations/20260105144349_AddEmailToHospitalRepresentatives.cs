using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HQS.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToHospitalRepresentatives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "HospitalRepresentatives",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "HospitalRepresentatives");
        }
    }
}
