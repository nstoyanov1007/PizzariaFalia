using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PizzariaFalia.Data.Migrations
{
    /// <inheritdoc />
    public partial class M4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPickup",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Name", "ParentCategoryId", "isDeleted" },
                values: new object[,]
                {
                    { 1, "Pizzas", "Pizzas", null, false },
                    { 4, "Salads", "Salads", null, false },
                    { 5, "Desserts", "Desserts", null, false },
                    { 2, "With Meat", "Pizzas with meat", 1, false },
                    { 3, "No Meat", "Pizzas with no meat", 1, false }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "CategoryId", "Description", "GramsBig", "GramsSmall", "Name", "PriceBig", "PriceSmall", "isDeleted" },
                values: new object[,]
                {
                    { 5, 4, "Iceberg lettuce, grilled chicken, croutons, parmesan and Caesar dressing.", null, 350m, "Caesar Salad", null, 7.80m, false },
                    { 6, 4, "Tomatoes, cucumbers, olives, feta cheese and olive oil.", null, 330m, "Greek Salad", null, 6.90m, false },
                    { 7, 5, "Traditional Italian dessert with mascarpone, espresso and cocoa.", null, 180m, "Tiramisu", null, 5.50m, false },
                    { 1, 3, "Classic pizza with tomato sauce, mozzarella and fresh basil.", 750m, 400m, "Margherita", 10.90m, 7.50m, false },
                    { 2, 2, "Tomato sauce, mozzarella and spicy pepperoni.", 800m, 450m, "Pepperoni", 12.50m, 8.90m, false },
                    { 3, 2, "Tomato sauce, mozzarella, ham and fresh mushrooms.", 820m, 480m, "Prosciutto Funghi", 13.20m, 9.20m, false },
                    { 4, 3, "Tomato sauce, mozzarella, peppers, olives, onions and mushrooms.", 780m, 460m, "Vegetarian", 12.00m, 8.70m, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsPickup",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
