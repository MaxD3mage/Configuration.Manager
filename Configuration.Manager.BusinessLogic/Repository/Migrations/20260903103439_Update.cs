using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Configuration.Manager.BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Configurations_CurrentVersionId",
                table: "Configurations",
                column: "CurrentVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Configurations_ConfigurationVersions_CurrentVersionId",
                table: "Configurations",
                column: "CurrentVersionId",
                principalTable: "ConfigurationVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configurations_ConfigurationVersions_CurrentVersionId",
                table: "Configurations");

            migrationBuilder.DropIndex(
                name: "IX_Configurations_CurrentVersionId",
                table: "Configurations");
        }
    }
}
