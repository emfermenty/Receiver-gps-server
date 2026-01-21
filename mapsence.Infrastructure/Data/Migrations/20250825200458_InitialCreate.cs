using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace mapsencetoWebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "zxc");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "login", nullable: false),
                    pass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "v_track_info",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    height = table.Column<double>(type: "double precision", nullable: false),
                    speed = table.Column<double>(type: "double precision", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sensor_id = table.Column<long>(type: "bigint", nullable: false),
                    track_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_v_track_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "v_tracks_view",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_stop = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sensorId = table.Column<int>(type: "integer", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    distance = table.Column<float>(type: "real", nullable: false),
                    avgSpeed = table.Column<float>(type: "real", nullable: false),
                    duration = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_v_tracks_view", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sensor",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imei = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor", x => x.id);
                    table.ForeignKey(
                        name: "FK_sensor_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "zxc",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "geo_data",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sensor_id = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    height = table.Column<double>(type: "double precision", nullable: false),
                    speed = table.Column<double>(type: "double precision", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    course = table.Column<double>(type: "double precision", nullable: false),
                    battery_voltage = table.Column<double>(type: "double precision", nullable: false),
                    satellite_count = table.Column<int>(type: "integer", nullable: false),
                    geom = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_geo_data_sensor_sensor_id",
                        column: x => x.sensor_id,
                        principalSchema: "zxc",
                        principalTable: "sensor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "track",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_stop = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sensor_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_track", x => x.id);
                    table.ForeignKey(
                        name: "FK_track_sensor_sensor_id",
                        column: x => x.sensor_id,
                        principalSchema: "zxc",
                        principalTable: "sensor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_track_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "zxc",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_geo_data_sensor_id",
                schema: "zxc",
                table: "geo_data",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_sensor_user_id",
                schema: "zxc",
                table: "sensor",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_track_sensor_id",
                schema: "zxc",
                table: "track",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_track_user_id",
                schema: "zxc",
                table: "track",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "geo_data",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "track",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "v_track_info",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "v_tracks_view",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "sensor",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "user",
                schema: "zxc");
        }
    }
}
