using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace mapsence.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "zxc");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "device_model",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_model", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "event_type",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    pass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sensor",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imei = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    model_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor", x => x.id);
                    table.ForeignKey(
                        name: "FK_sensor_device_model_model_id",
                        column: x => x.model_id,
                        principalSchema: "zxc",
                        principalTable: "device_model",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sensor_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "zxc",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "geo_data",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_id = table.Column<long>(type: "bigint", nullable: false),
                    sensor_id = table.Column<int>(type: "integer", nullable: false),
                    event_type_id = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    height = table.Column<double>(type: "double precision", nullable: false),
                    speed = table.Column<double>(type: "double precision", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    course = table.Column<double>(type: "double precision", nullable: false),
                    battery_voltage = table.Column<double>(type: "double precision", nullable: false),
                    satellite_count = table.Column<int>(type: "integer", nullable: false),
                    geom = table.Column<Point>(type: "geometry (Point, 4326)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_geo_data_event_type_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "zxc",
                        principalTable: "event_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_geo_data_sensor_sensor_id",
                        column: x => x.sensor_id,
                        principalSchema: "zxc",
                        principalTable: "sensor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensor_session",
                schema: "zxc",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sensor_id = table.Column<int>(type: "integer", nullable: false),
                    ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    firmware = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_seen_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_sensor_session_sensor_sensor_id",
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
                    date_start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_stop = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                name: "IX_device_model_manufacturer_name",
                schema: "zxc",
                table: "device_model",
                columns: new[] { "manufacturer", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_type_code",
                schema: "zxc",
                table: "event_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geo_data_event_type_id",
                schema: "zxc",
                table: "geo_data",
                column: "event_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_geo_data_sensor_id",
                schema: "zxc",
                table: "geo_data",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_geo_data_source_id",
                schema: "zxc",
                table: "geo_data",
                column: "source_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sensor_imei",
                schema: "zxc",
                table: "sensor",
                column: "imei",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sensor_model_id",
                schema: "zxc",
                table: "sensor",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_sensor_user_id",
                schema: "zxc",
                table: "sensor",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_sensor_session_sensor_id_started_at",
                schema: "zxc",
                table: "sensor_session",
                columns: new[] { "sensor_id", "started_at" },
                unique: true);

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

            // Точка относится к треку, если она от того же сенсора и попадает в интервал date_start..date_stop
            migrationBuilder.Sql("""
                CREATE VIEW zxc.v_track_info AS
                SELECT g.id::bigint        AS id,
                       g.latitude,
                       g.longitude,
                       g.height,
                       g.speed,
                       g.date_time,
                       g.sensor_id::bigint AS sensor_id,
                       t.id::bigint        AS track_id
                FROM zxc.geo_data g
                JOIN zxc.track t
                  ON t.sensor_id = g.sensor_id
                 AND g.date_time >= t.date_start
                 AND (t.date_stop IS NULL OR g.date_time <= t.date_stop);
                """);

            // distance — длина трека в километрах
            migrationBuilder.Sql("""
                CREATE VIEW zxc.v_tracks_view AS
                SELECT t.id,
                       t.name,
                       t.date_start,
                       t.date_stop,
                       t.sensor_id,
                       t.user_id,
                       COALESCE(ST_Length(ST_MakeLine(g.geom ORDER BY g.date_time)::geography) / 1000, 0)::real AS distance,
                       COALESCE(AVG(g.speed), 0)::real AS avg_speed,
                       date_trunc('second', COALESCE(t.date_stop, now() AT TIME ZONE 'utc') - t.date_start)::text AS duration
                FROM zxc.track t
                LEFT JOIN zxc.geo_data g
                  ON g.sensor_id = t.sensor_id
                 AND g.date_time >= t.date_start
                 AND (t.date_stop IS NULL OR g.date_time <= t.date_stop)
                GROUP BY t.id;
                """);

            migrationBuilder.Sql("""
                CREATE FUNCTION zxc.fnc_track_action(p_date timestamp, p_sensor integer, p_user integer, p_name text)
                RETURNS integer
                LANGUAGE plpgsql AS $$
                DECLARE
                    v_id integer;
                BEGIN
                    INSERT INTO zxc.track (date_start, sensor_id, user_id, name)
                    VALUES (p_date, p_sensor, p_user, p_name)
                    RETURNING id INTO v_id;
                    RETURN v_id;
                END;
                $$;

                CREATE PROCEDURE zxc.prc_track_end(p_id integer, p_date_stop timestamp)
                LANGUAGE sql AS $$
                    UPDATE zxc.track SET date_stop = p_date_stop WHERE id = p_id;
                $$;

                CREATE PROCEDURE zxc.prc_track_comeback(p_id integer)
                LANGUAGE sql AS $$
                    UPDATE zxc.track SET date_stop = NULL WHERE id = p_id;
                $$;

                CREATE PROCEDURE zxc.prc_edit_track(p_id integer, p_name text)
                LANGUAGE sql AS $$
                    UPDATE zxc.track SET name = p_name WHERE id = p_id;
                $$;

                CREATE PROCEDURE zxc.prc_delete_track(p_id integer)
                LANGUAGE sql AS $$
                    DELETE FROM zxc.track WHERE id = p_id;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP PROCEDURE IF EXISTS zxc.prc_delete_track(integer);
                DROP PROCEDURE IF EXISTS zxc.prc_edit_track(integer, text);
                DROP PROCEDURE IF EXISTS zxc.prc_track_comeback(integer);
                DROP PROCEDURE IF EXISTS zxc.prc_track_end(integer, timestamp);
                DROP FUNCTION IF EXISTS zxc.fnc_track_action(timestamp, integer, integer, text);
                DROP VIEW IF EXISTS zxc.v_tracks_view;
                DROP VIEW IF EXISTS zxc.v_track_info;
                """);

            migrationBuilder.DropTable(
                name: "geo_data",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "sensor_session",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "track",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "event_type",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "sensor",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "device_model",
                schema: "zxc");

            migrationBuilder.DropTable(
                name: "user",
                schema: "zxc");
        }
    }
}
