using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scruffy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuildId",
                table: "Roles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Roles");
        }
    }
}
