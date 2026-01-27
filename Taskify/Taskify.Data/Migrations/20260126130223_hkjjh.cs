using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    /// <inheritdoc />
    public partial class hkjjh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_ManagerIdId",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "ManagerIdId",
                table: "projects",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_ManagerIdId",
                table: "projects",
                newName: "IX_projects_ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_ManagerId",
                table: "projects",
                column: "ManagerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_ManagerId",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "projects",
                newName: "ManagerIdId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_ManagerId",
                table: "projects",
                newName: "IX_projects_ManagerIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_ManagerIdId",
                table: "projects",
                column: "ManagerIdId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
