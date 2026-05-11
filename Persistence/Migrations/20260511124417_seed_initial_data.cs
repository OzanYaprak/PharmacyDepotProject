using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seed_initial_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "CreatedDate", "DeletedDate", "Email", "LicenseNumber", "Name", "Phone", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-000000000001"), "Bağcılar Mah. 1. Sokak No:5, İstanbul", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "guven@eczane.com", "ECZ-100001", "Eczane Güven", "0212-111-1111", null },
                    { new Guid("cccccccc-cccc-cccc-cccc-000000000002"), "Çankaya Mah. 2. Cadde No:10, Ankara", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "saglik@eczane.com", "ECZ-100002", "Eczane Sağlık", "0312-222-2222", null }
                });

            migrationBuilder.InsertData(
                table: "Drugs",
                columns: new[] { "Id", "BN", "CreatedDate", "DeletedDate", "ExpireDate", "GTIN", "Name", "SN", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-000000000001"), "BN-ASP-2025", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "08690001000001", "Aspirin 500mg", "SN-ASP-001", null },
                    { new Guid("dddddddd-dddd-dddd-dddd-000000000002"), "BN-AMX-2025", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2027, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "08690002000002", "Amoksisilin 250mg", "SN-AMX-001", null },
                    { new Guid("dddddddd-dddd-dddd-dddd-000000000003"), "BN-PAR-2025", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2027, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "08690003000003", "Parol 500mg", "SN-PAR-001", null }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactPerson", "CreatedDate", "DeletedDate", "Email", "Name", "Phone", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000001"), "Boğaziçi Mah. Sanayi Cad. No:1, İstanbul", "Ahmet Yılmaz", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "tedarik@abdiibrahim.com", "Abdi İbrahim İlaç", "0212-333-3333", null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000002"), "Kartal Mah. İlaç Sokak No:7, İstanbul", "Mehmet Demir", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "tedarik@eczacibasi.com", "Eczacıbaşı İlaç", "0216-444-4444", null }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Capacity", "CreatedDate", "DeletedDate", "Location", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000001"), 50000, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "İkitelli OSB, İstanbul", "Ana Depo", null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000002"), 10000, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Esenyurt, İstanbul", "Soğuk Zincir Deposu", null }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "OrderDate", "Status", "SupplierId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ffff0001-0000-0000-0000-000000000001"), new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 3, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000001"), null },
                    { new Guid("ffff0001-0000-0000-0000-000000000002"), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000002"), null }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CreatedDate", "CustomerId", "DeletedDate", "SaleDate", "TotalAmount", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("00000022-0000-0000-0000-000000000001"), new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("cccccccc-cccc-cccc-cccc-000000000001"), null, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1250.00m, null },
                    { new Guid("00000022-0000-0000-0000-000000000002"), new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("cccccccc-cccc-cccc-cccc-000000000002"), null, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), 875.00m, null }
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "DrugId", "Quantity", "UnitPrice", "UpdatedDate", "WarehouseId" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-eeee-eeee-eeee-000000000001"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000001"), 5000, 12.50m, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000001") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-000000000002"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000002"), 3000, 45.00m, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000001") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000003"), 2000, 8.75m, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "DrugId", "OrderId", "Quantity", "UnitPrice", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("00000011-0000-0000-0000-000000000001"), new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000001"), new Guid("ffff0001-0000-0000-0000-000000000001"), 1000, 10.00m, null },
                    { new Guid("00000011-0000-0000-0000-000000000002"), new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000002"), new Guid("ffff0001-0000-0000-0000-000000000001"), 500, 40.00m, null },
                    { new Guid("00000011-0000-0000-0000-000000000003"), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000003"), new Guid("ffff0001-0000-0000-0000-000000000002"), 800, 7.50m, null }
                });

            migrationBuilder.InsertData(
                table: "SaleItems",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "DrugId", "Quantity", "SaleId", "UnitPrice", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("00000033-0000-0000-0000-000000000001"), new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000001"), 100, new Guid("00000022-0000-0000-0000-000000000001"), 12.50m, null },
                    { new Guid("00000033-0000-0000-0000-000000000002"), new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("dddddddd-dddd-dddd-dddd-000000000003"), 100, new Guid("00000022-0000-0000-0000-000000000002"), 8.75m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SaleItems",
                keyColumn: "Id",
                keyValue: new Guid("00000033-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SaleItems",
                keyColumn: "Id",
                keyValue: new Guid("00000033-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-000000000001"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-000000000002"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-000000000003"));

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-000000000001"));

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-000000000002"));

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-000000000003"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("ffff0001-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("ffff0001-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: new Guid("00000022-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: new Guid("00000022-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000001"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000002"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-000000000001"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-000000000002"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000001"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-000000000002"));
        }
    }
}
