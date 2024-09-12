using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace social_oc_api.Migrations
{
    /// <inheritdoc />
    public partial class seedDataRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3b2d71d6-cb9e-419f-a805-5be22fc395fe", "3b2d71d6-cb9e-419f-a805-5be22fc395fe", "Writer", "WRITER" },
                    { "d5223cbc-9bdc-4088-ae3b-345e281c571b", "d5223cbc-9bdc-4088-ae3b-345e281c571b", "Reader", "READER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b2d71d6-cb9e-419f-a805-5be22fc395fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d5223cbc-9bdc-4088-ae3b-345e281c571b");
        }
    }
}
