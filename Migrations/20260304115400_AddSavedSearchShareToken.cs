using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shortlist.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedSearchShareToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedSearches_UserProfile_UserProfileId",
                table: "SavedSearches");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "SavedSearches",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSearches_UserProfileId",
                table: "SavedSearches",
                newName: "IX_SavedSearches_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ShareToken",
                table: "SavedSearches",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSearches_Users_UserId",
                table: "SavedSearches",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedSearches_Users_UserId",
                table: "SavedSearches");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShareToken",
                table: "SavedSearches");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SavedSearches",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSearches_UserId",
                table: "SavedSearches",
                newName: "IX_SavedSearches_UserProfileId");

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSearches_UserProfile_UserProfileId",
                table: "SavedSearches",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
