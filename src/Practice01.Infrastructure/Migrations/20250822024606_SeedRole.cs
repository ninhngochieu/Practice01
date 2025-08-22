using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Practice01.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "LastModifiedBy", "LastModifiedDate", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("21b6d6f3-e0cd-4aba-ae80-b41edea87b86"), null, null, null, null, null, null, null, "Manager", "MANAGER" },
                    { new Guid("53dd8b32-fbb2-40bb-88c9-819de4865740"), null, null, null, null, null, null, null, "Member", "MEMBER" },
                    { new Guid("5c7f0163-27a7-4de8-a448-ed7e77577442"), null, null, null, null, null, null, null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("21b6d6f3-e0cd-4aba-ae80-b41edea87b86"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("53dd8b32-fbb2-40bb-88c9-819de4865740"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5c7f0163-27a7-4de8-a448-ed7e77577442"));
        }
    }
}
