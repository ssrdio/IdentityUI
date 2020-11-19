using Microsoft.EntityFrameworkCore.Migrations;

namespace SSRD.IdentityUI.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class AuditMetadataGroupIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupIdentifier",
                table: "Audit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Audit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectMetadata",
                table: "Audit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupIdentifier",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "SubjectMetadata",
                table: "Audit");
        }
    }
}
