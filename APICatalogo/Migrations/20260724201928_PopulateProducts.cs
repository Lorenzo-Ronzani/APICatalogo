using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulateProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
                   "VALUES ('Coca-Cola Zero', 'Refrigerante de Cola 350 ml', 5.45, 'cocacola.jpg', 50, GETDATE(), 1)");

            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
                   "VALUES ('Lanche de Pernil', 'Lanche de Pernil com barbecue', 15.50, 'lanchepernil.jpg', 23, GETDATE(), 2)");

            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
                   "VALUES ('Pudim', 'Pudim de leite condensado', 8.90, 'pudim.jpg', 35, GETDATE(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Products");
        }
    }
}