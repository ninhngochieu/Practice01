using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Practice01.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ba638760-5686-4e92-b4d5-59850381bd8b"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKLc4wCc9E6qB9WKS5zp6IBMxPx7iAhHc398oPDnk1v1751JjgT5300w+MxWkUAphw==");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DateOfBirth", "DeletedBy", "DeletedDate", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastModifiedBy", "LastModifiedDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9"), 0, "", null, null, null, null, null, "9D2L6@example.com", true, "", true, null, null, "", false, null, "9D2L6@example.com", "MANAGER", "AQAAAAIAAYagAAAAENnMpMWlSK/4b3LV2ljXfum7Hqf012Gx6XCnW1at2E3OW5XM/BNskjBMX4DgnRLSNw==", "+7(999)999-99-99", true, "", false, "manager" },
                    { new Guid("dbd9b6f3-12d6-4755-824c-2933ecce4c4a"), 0, "", null, null, null, null, null, "6tMf6@example.com", true, "", true, null, null, "", false, null, "6tMf6@example.com", "MEMBER", "AQAAAAIAAYagAAAAEF/PjJMK0cUicohKoBzoZW2FkR4lEs1QFtqsAQDjDSBdlZ//SDaqT4cXbGi2QZjZOA==", "+7(999)999-99-99", true, "", false, "member" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9") },
                    { new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"), new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9") },
                    { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("dbd9b6f3-12d6-4755-824c-2933ecce4c4a") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ddad094e-f7b4-446c-9639-9f7a695a4db8"), new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0517ce8f-9d05-40ae-8c42-d93c8b5da363"), new Guid("dbd9b6f3-12d6-4755-824c-2933ecce4c4a") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b745d2be-dc7c-46ec-b4b3-da2c83099fd9"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("dbd9b6f3-12d6-4755-824c-2933ecce4c4a"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ba638760-5686-4e92-b4d5-59850381bd8b"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGR8kPQPIwgponxoYf+nivAE9iBl4wU4/r7hGaATJsTzhvGSCFU0bepIDsLJNT+odg==");
        }
    }
}
