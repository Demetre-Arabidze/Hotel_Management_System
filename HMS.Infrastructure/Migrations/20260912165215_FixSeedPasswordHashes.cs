using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAEBAQEBAQEBAQEBAQEBAQFgQSgkAKOAlM4nL+ruW1lW8cX7LQFBLSFMhGfkRpgdtA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAICAgICAgICAgICAgICAgL2rbbBkaJpmUNkK8uDDH5I10HuF26q4gmKLVeIu9jHBg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEK2/S7m4L1KzP6/1Z8M2N8R2L1P3R5T7V9X1Z3A5B7C9D1E3F5G7H9I0==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEH3/A9M2K4L6P8/2a9N3O9S3M2Q4S6U8W0Y2A4B6C8D0E2F4G6H8I9==");
        }
    }
}
