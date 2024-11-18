using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace social_oc_api.Migrations
{
    /// <inheritdoc />
    public partial class changeRelationFollowerRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestFollowers_AspNetUsers_FollowerId",
                table: "RequestFollowers");

            migrationBuilder.DropIndex(
                name: "IX_RequestFollowers_FollowerId",
                table: "RequestFollowers");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "RequestFollowers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "RequestFollowers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFollowers_FollowingId",
                table: "RequestFollowers",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestFollowers_AspNetUsers_FollowingId",
                table: "RequestFollowers",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestFollowers_AspNetUsers_FollowingId",
                table: "RequestFollowers");

            migrationBuilder.DropIndex(
                name: "IX_RequestFollowers_FollowingId",
                table: "RequestFollowers");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "RequestFollowers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "RequestFollowers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFollowers_FollowerId",
                table: "RequestFollowers",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestFollowers_AspNetUsers_FollowerId",
                table: "RequestFollowers",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
