using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sufi.Demo.PeopleDirectory.Libs.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddServerInfoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerInfos",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerInfos", x => x.Key);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerInfos");
        }
    }
}
