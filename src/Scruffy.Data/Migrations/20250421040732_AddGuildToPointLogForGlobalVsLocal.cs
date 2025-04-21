using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scruffy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildToPointLogForGlobalVsLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuildId",
                table: "PointLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "PointLogs");
        }
    }
}
