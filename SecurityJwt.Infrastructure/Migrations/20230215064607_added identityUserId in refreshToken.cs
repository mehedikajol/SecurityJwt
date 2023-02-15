using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityJwt.Infrastructure.Migrations
{
    public partial class addedidentityUserIdinrefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "RefreshTokens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "RefreshTokens");
        }
    }
}
