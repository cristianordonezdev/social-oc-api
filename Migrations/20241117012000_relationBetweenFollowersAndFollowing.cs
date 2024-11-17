using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace social_oc_api.Migrations
{
    /// <inheritdoc />
    public partial class relationBetweenFollowersAndFollowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_UserId",
                table: "Followers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Followers",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Followers_UserId",
                table: "Followers",
                newName: "IX_Followers_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "Followers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "Followers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowingId",
                table: "Followers",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_ApplicationUserId",
                table: "Followers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowingId",
                table: "Followers",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_ApplicationUserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowingId",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowingId",
                table: "Followers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Followers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Followers_ApplicationUserId",
                table: "Followers",
                newName: "IX_Followers_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_UserId",
                table: "Followers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
