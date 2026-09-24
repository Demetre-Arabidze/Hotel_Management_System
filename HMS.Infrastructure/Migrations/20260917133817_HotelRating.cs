using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HotelRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Hotels_Rating",
                table: "Hotels");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hotels_Rating",
                table: "Hotels",
                sql: "[Rating] >= 0.0 AND [Rating] <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Hotels_Rating",
                table: "Hotels");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hotels_Rating",
                table: "Hotels",
                sql: "[Rating] >= 1 AND [Rating] <= 5");
        }
    }
}
