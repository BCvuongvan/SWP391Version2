using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TingStoreAPI.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    cateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cateDescribe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cateStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.cateId);
                });

            migrationBuilder.CreateTable(
                name: "orderStatuses",
                columns: table => new
                {
                    orderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderStatuses", x => x.orderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userName);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    proId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    proDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    proQuantity = table.Column<int>(type: "int", nullable: false),
                    proImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    proStatus = table.Column<bool>(type: "bit", nullable: false),
                    cateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.proId);
                    table.ForeignKey(
                        name: "FK_products_categories_cateId",
                        column: x => x.cateId,
                        principalTable: "categories",
                        principalColumn: "cateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    orderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "Money", nullable: false),
                    orderStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_orders_orderStatuses_orderStatusId",
                        column: x => x.orderStatusId,
                        principalTable: "orderStatuses",
                        principalColumn: "orderStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_users_userName",
                        column: x => x.userName,
                        principalTable: "users",
                        principalColumn: "userName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    proId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => new { x.userName, x.proId });
                    table.ForeignKey(
                        name: "FK_carts_products_proId",
                        column: x => x.proId,
                        principalTable: "products",
                        principalColumn: "proId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_carts_users_userName",
                        column: x => x.userName,
                        principalTable: "users",
                        principalColumn: "userName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discountPercents",
                columns: table => new
                {
                    discountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proId = table.Column<int>(type: "int", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discountPercents", x => x.discountId);
                    table.ForeignKey(
                        name: "FK_discountPercents_products_proId",
                        column: x => x.proId,
                        principalTable: "products",
                        principalColumn: "proId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    proId = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => new { x.userName, x.proId });
                    table.ForeignKey(
                        name: "FK_feedbacks_products_proId",
                        column: x => x.proId,
                        principalTable: "products",
                        principalColumn: "proId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_userName",
                        column: x => x.userName,
                        principalTable: "users",
                        principalColumn: "userName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productImages",
                columns: table => new
                {
                    imageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proId = table.Column<int>(type: "int", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productImages", x => x.imageId);
                    table.ForeignKey(
                        name: "FK_productImages_products_proId",
                        column: x => x.proId,
                        principalTable: "products",
                        principalColumn: "proId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orderDetails",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false),
                    proId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    subTotal = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderDetails", x => new { x.orderId, x.proId });
                    table.ForeignKey(
                        name: "FK_orderDetails_orders_orderId",
                        column: x => x.orderId,
                        principalTable: "orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orderDetails_products_proId",
                        column: x => x.proId,
                        principalTable: "products",
                        principalColumn: "proId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "cateId", "cateDescribe", "cateName", "cateStatus" },
                values: new object[,]
                {
                    { 1, "The best of phone", "Iphone", true },
                    { 2, "The best of phone", "Samsung", true }
                });

            migrationBuilder.InsertData(
                table: "orderStatuses",
                columns: new[] { "orderStatusId", "statusName" },
                values: new object[,]
                {
                    { 1, "Spending" },
                    { 2, "Successful" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_carts_proId",
                table: "carts",
                column: "proId");

            migrationBuilder.CreateIndex(
                name: "IX_discountPercents_proId",
                table: "discountPercents",
                column: "proId");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_proId",
                table: "feedbacks",
                column: "proId");

            migrationBuilder.CreateIndex(
                name: "IX_orderDetails_proId",
                table: "orderDetails",
                column: "proId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_orderStatusId",
                table: "orders",
                column: "orderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_userName",
                table: "orders",
                column: "userName");

            migrationBuilder.CreateIndex(
                name: "IX_productImages_proId",
                table: "productImages",
                column: "proId");

            migrationBuilder.CreateIndex(
                name: "IX_products_cateId",
                table: "products",
                column: "cateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.DropTable(
                name: "discountPercents");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "orderDetails");

            migrationBuilder.DropTable(
                name: "productImages");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "orderStatuses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
