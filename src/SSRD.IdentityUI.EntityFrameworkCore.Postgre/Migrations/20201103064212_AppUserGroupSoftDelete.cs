using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.Migrations
{
    public partial class AppUserGroupSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "_DeletedDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "_DeletedDate",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_DeletedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "_DeletedDate",
                table: "Groups");
        }
    }
}
