using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Practice01.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "LastModifiedBy", "LastModifiedDate", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), null, null, null, null, null, null, null, "Member", "MEMBER" },
                    { new Guid("dc8bcc55-8540-4bb3-b45c-719ea1bce0f2"), null, null, null, null, null, null, null, "Administrator", "ADMINISTRATOR" },
                    { new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"), null, null, null, null, null, null, null, "Manager", "MANAGER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DateOfBirth", "DeletedBy", "DeletedDate", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastModifiedBy", "LastModifiedDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("ba638760-5686-4e92-b4d5-59850381bd8b"), 0, "", null, null, null, null, null, "pA5eF@example.com", true, "", true, null, null, "", false, null, "pA5eF@example.com", "ADMIN", "AQAAAAIAAYagAAAAEGR8kPQPIwgponxoYf+nivAE9iBl4wU4/r7hGaATJsTzhvGSCFU0bepIDsLJNT+odg==", "+7(999)999-99-99", true, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") },
                    { new Guid("dc8bcc55-8540-4bb3-b45c-719ea1bce0f2"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") },
                    { new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("dc8bcc55-8540-4bb3-b45c-719ea1bce0f2"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"), new Guid("ba638760-5686-4e92-b4d5-59850381bd8b") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("dc8bcc55-8540-4bb3-b45c-719ea1bce0f2"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ba638760-5686-4e92-b4d5-59850381bd8b"));

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
    }
}
