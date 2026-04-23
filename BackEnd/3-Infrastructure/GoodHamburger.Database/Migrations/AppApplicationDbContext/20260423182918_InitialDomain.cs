using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburger.Database.Migrations.AppApplicationDbContext
{
    /// <inheritdoc />
    public partial class InitialDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cat_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category_type = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cat_ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sale_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    reference_cost_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    role_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    identity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    display_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    profile_picture_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geo_countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    iso_code = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geo_street_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_street_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ord_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    cancel_reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    delivery_street_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_street_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    delivery_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    delivery_complement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    delivery_zip_code = table.Column<string>(type: "char(8)", nullable: false),
                    delivery_neighborhood_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_fee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ord_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sales_coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_percentage = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    usage_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    min_order_value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales_coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cat_menu_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    calories = table.Column<int>(type: "integer", nullable: true),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_menu_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cat_menu_items_cat_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "cat_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "geo_states",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uf = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_states", x => x.Id);
                    table.ForeignKey(
                        name: "FK_geo_states_geo_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "geo_countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ord_order_discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    coupon_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    applied_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ord_order_discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ord_order_discounts_ord_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "ord_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCoupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCoupons_ord_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ord_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderCoupons_sales_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "sales_coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cat_menu_item_ingredients",
                columns: table => new
                {
                    MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    is_removable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_menu_item_ingredients", x => new { x.MenuItemId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_cat_menu_item_ingredients_cat_ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "cat_ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cat_menu_item_ingredients_cat_menu_items_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "cat_menu_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ord_order_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    menu_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ord_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ord_order_items_cat_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "cat_menu_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ord_order_items_ord_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "ord_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "geo_cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_geo_cities_geo_states_state_id",
                        column: x => x.state_id,
                        principalTable: "geo_states",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ord_order_item_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ord_order_item_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ord_order_item_details_ord_order_items_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "ord_order_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "geo_neighborhoods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_neighborhoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_geo_neighborhoods_geo_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "geo_cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    document_type = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    address_street_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_street = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    address_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address_complement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_zip_code = table.Column<string>(type: "char(8)", maxLength: 8, nullable: false),
                    address_neighborhood_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    identity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    display_name = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    profile_picture_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_profiles_geo_neighborhoods_address_neighborhood_id",
                        column: x => x.address_neighborhood_id,
                        principalTable: "geo_neighborhoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_profiles_geo_street_types_address_street_type_id",
                        column: x => x.address_street_type_id,
                        principalTable: "geo_street_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cat_category_display_order",
                table: "cat_categories",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "ix_cat_category_slug",
                table: "cat_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cat_ingredient_active",
                table: "cat_ingredients",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_cat_ingredient_name",
                table: "cat_ingredients",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_cat_menu_item_ingredients_IngredientId",
                table: "cat_menu_item_ingredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "ix_cat_menu_item_category",
                table: "cat_menu_items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "ix_cat_menu_item_sku",
                table: "cat_menu_items",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cat_menu_item_slug",
                table: "cat_menu_items",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_profile_full_name",
                table: "customer_profiles",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "ix_customer_profile_identity_id",
                table: "customer_profiles",
                column: "identity_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_profiles_address_neighborhood_id",
                table: "customer_profiles",
                column: "address_neighborhood_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_profiles_address_street_type_id",
                table: "customer_profiles",
                column: "address_street_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_profile_code",
                table: "employee_profiles",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_profile_identity_id",
                table: "employee_profiles",
                column: "identity_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geo_cities_state_id",
                table: "geo_cities",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "IX_geo_countries_iso_code",
                table: "geo_countries",
                column: "iso_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geo_neighborhoods_city_id",
                table: "geo_neighborhoods",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_geo_states_country_id",
                table: "geo_states",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_geo_states_uf_country_id",
                table: "geo_states",
                columns: new[] { "uf", "country_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ord_discount_coupon_code",
                table: "ord_order_discounts",
                column: "coupon_code",
                filter: "coupon_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_ord_discount_order_id",
                table: "ord_order_discounts",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_ord_detail_is_removed",
                table: "ord_order_item_details",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_ord_order_item_details_OrderItemId",
                table: "ord_order_item_details",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ord_order_items_menu_item_id",
                table: "ord_order_items",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_ord_order_items_order_id",
                table: "ord_order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_ord_order_customer",
                table: "ord_orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_ord_order_date",
                table: "ord_orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "ix_ord_order_status",
                table: "ord_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCoupons_CouponId",
                table: "OrderCoupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCoupons_OrderId",
                table: "OrderCoupons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "ix_sal_coupon_active",
                table: "sales_coupons",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_sal_coupon_code",
                table: "sales_coupons",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cat_menu_item_ingredients");

            migrationBuilder.DropTable(
                name: "customer_profiles");

            migrationBuilder.DropTable(
                name: "employee_profiles");

            migrationBuilder.DropTable(
                name: "ord_order_discounts");

            migrationBuilder.DropTable(
                name: "ord_order_item_details");

            migrationBuilder.DropTable(
                name: "OrderCoupons");

            migrationBuilder.DropTable(
                name: "cat_ingredients");

            migrationBuilder.DropTable(
                name: "geo_neighborhoods");

            migrationBuilder.DropTable(
                name: "geo_street_types");

            migrationBuilder.DropTable(
                name: "ord_order_items");

            migrationBuilder.DropTable(
                name: "sales_coupons");

            migrationBuilder.DropTable(
                name: "geo_cities");

            migrationBuilder.DropTable(
                name: "cat_menu_items");

            migrationBuilder.DropTable(
                name: "ord_orders");

            migrationBuilder.DropTable(
                name: "geo_states");

            migrationBuilder.DropTable(
                name: "cat_categories");

            migrationBuilder.DropTable(
                name: "geo_countries");
        }
    }
}
