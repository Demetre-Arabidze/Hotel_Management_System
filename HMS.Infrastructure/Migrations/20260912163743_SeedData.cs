using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "11111111-1111-1111-1111-111111111111", "Admin", "ADMIN" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "22222222-2222-2222-2222-222222222222", "Manager", "MANAGER" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "33333333-3333-3333-3333-333333333333", "Guest", "GUEST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), 0, "00000000-0000-0000-0000-000000000001", "manager@hms.com", true, false, null, "MANAGER@HMS.COM", "MANAGER@HMS.COM", "AQAAAAIAAYagAAAAEK2/S7m4L1KzP6/1Z8M2N8R2L1P3R5T7V9X1Z3A5B7C9D1E3F5G7H9I0==", null, false, "00000000-0000-0000-0000-000000000001", false, "manager@hms.com" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), 0, "00000000-0000-0000-0000-000000000002", "guest@hms.com", true, false, null, "GUEST@HMS.COM", "GUEST@HMS.COM", "AQAAAAIAAYagAAAAEH3/A9M2K4L6P8/2a9N3O9S3M2Q4S6U8W0Y2A4B6C8D0E2F4G6H8I9==", null, false, "00000000-0000-0000-0000-000000000002", false, "guest@hms.com" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "City", "Country", "Name", "Rating" },
                values: new object[] { new Guid("c0000000-0000-0000-0000-000000000001"), "Rustaveli Avenue 12", "Tbilisi", "Georgia", "Grand Plaza Hotel", 4.8m });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("a0000000-0000-0000-0000-000000000001") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("a0000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "FirstName", "LastName", "PersonalNumber", "PhoneNumber", "UserId" },
                values: new object[] { new Guid("b0000000-0000-0000-0000-000000000002"), "John", "Doe", "01001011111", "+995555123456", new Guid("a0000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "Email", "FirstName", "HotelId", "LastName", "PersonalNumber", "PhoneNumber", "UserId" },
                values: new object[] { new Guid("b0000000-0000-0000-0000-000000000001"), "manager@hms.com", "Alex", new Guid("c0000000-0000-0000-0000-000000000001"), "Manager", "01001022222", "+995555987654", new Guid("a0000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "HotelId", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("c0000000-0000-0000-0000-000000000001"), "Deluxe Suite 101", 150.00m },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("c0000000-0000-0000-0000-000000000001"), "Standard Room 102", 85.00m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "CheckInDate", "CheckOutDate", "GuestId" },
                values: new object[] { new Guid("e0000000-0000-0000-0000-000000000001"), new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 5), new Guid("b0000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "ReservationRooms",
                columns: new[] { "ReservationId", "RoomId" },
                values: new object[] { new Guid("e0000000-0000-0000-0000-000000000001"), new Guid("d0000000-0000-0000-0000-000000000001") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("a0000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("a0000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ReservationRooms",
                keyColumns: new[] { "ReservationId", "RoomId" },
                keyValues: new object[] { new Guid("e0000000-0000-0000-0000-000000000001"), new Guid("d0000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"));
        }
    }
}
