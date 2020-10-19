using Microsoft.EntityFrameworkCore.Migrations;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Migrations
{
    public partial class UserActiveTwoFactorAuthenticationColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TwoFactor",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactor",
                table: "Users");
        }
    }
}
