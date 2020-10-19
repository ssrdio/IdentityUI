using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.Migrations
{
    public partial class AddAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionType = table.Column<int>(nullable: false),
                    ObjectIdentifier = table.Column<string>(nullable: true),
                    ObjectType = table.Column<string>(nullable: true),
                    ObjectMetadata = table.Column<string>(nullable: true),
                    SubjectType = table.Column<int>(nullable: false),
                    SubjectIdentifier = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    RemoteIp = table.Column<string>(nullable: true),
                    ResourceName = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    TraceIdentifier = table.Column<string>(nullable: true),
                    AppVersion = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit");
        }
    }
}
