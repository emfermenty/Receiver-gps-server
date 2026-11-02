using Gps.Transport;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Receiver_server.Database;

/// <summary>
/// Ненормализованная БД экспорта: одна таблица raw_points в SQLite.
/// </summary>
public class RawPointStore(IConfiguration configuration, IHostEnvironment environment)
{
    private readonly string _connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(environment.ContentRootPath, configuration["Sqlite:Path"]!)
    }.ToString();

    public async Task InitializeAsync()
    {
        await using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            PRAGMA journal_mode = WAL;
            CREATE TABLE IF NOT EXISTS raw_points (
                id                 INTEGER PRIMARY KEY AUTOINCREMENT,
                imei               TEXT    NOT NULL,
                manufacturer       TEXT    NOT NULL,
                model              TEXT    NOT NULL,
                firmware           TEXT    NOT NULL,
                session_ip         TEXT    NOT NULL,
                session_started_at TEXT    NOT NULL,
                event_code         INTEGER NOT NULL,
                event_name         TEXT    NOT NULL,
                device_time        TEXT    NOT NULL,
                received_at        TEXT    NOT NULL,
                latitude           REAL    NOT NULL,
                longitude          REAL    NOT NULL,
                height             INTEGER NOT NULL,
                speed              REAL    NOT NULL,
                course             REAL    NOT NULL,
                satellite_count    INTEGER NOT NULL,
                battery_mv         INTEGER NOT NULL,
                sent               INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS ix_raw_points_sent_id ON raw_points (sent, id);
            """;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<RawPointDto>> GetUnsentAsync(int limit)
    {
        await using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, imei, manufacturer, model, firmware,
                   session_ip, session_started_at,
                   event_code, event_name,
                   device_time, received_at,
                   latitude, longitude, height, speed, course, satellite_count, battery_mv
            FROM raw_points
            WHERE sent = 0
            ORDER BY id
            LIMIT $limit;
            """;
        cmd.Parameters.AddWithValue("$limit", limit);

        var points = new List<RawPointDto>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            points.Add(new RawPointDto
            {
                SourceId = reader.GetInt64(0),
                Imei = reader.GetString(1),
                Manufacturer = reader.GetString(2),
                Model = reader.GetString(3),
                Firmware = reader.GetString(4),
                SessionIp = reader.GetString(5),
                SessionStartedAt = AsUtc(reader.GetDateTime(6)),
                EventCode = reader.GetInt32(7),
                EventName = reader.GetString(8),
                DeviceTime = AsUtc(reader.GetDateTime(9)),
                ReceivedAt = AsUtc(reader.GetDateTime(10)),
                Latitude = reader.GetDouble(11),
                Longitude = reader.GetDouble(12),
                Height = reader.GetInt32(13),
                Speed = reader.GetDouble(14),
                Course = reader.GetDouble(15),
                SatelliteCount = reader.GetInt32(16),
                BatteryMv = reader.GetInt32(17)
            });
        }
        return points;
    }

    public async Task MarkSentAsync(long id)
    {
        await using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE raw_points SET sent = 1 WHERE id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    // SQLite хранит время без часового пояса, все значения в таблице записаны в UTC
    private static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);

    public async Task<long> InsertAsync(RawPointDto point)
    {
        await using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO raw_points (
                imei, manufacturer, model, firmware,
                session_ip, session_started_at,
                event_code, event_name,
                device_time, received_at,
                latitude, longitude, height, speed, course, satellite_count, battery_mv)
            VALUES (
                $imei, $manufacturer, $model, $firmware,
                $sessionIp, $sessionStartedAt,
                $eventCode, $eventName,
                $deviceTime, $receivedAt,
                $lat, $lon, $height, $speed, $course, $satCount, $batteryMv)
            RETURNING id;
            """;

        cmd.Parameters.AddWithValue("$imei", point.Imei);
        cmd.Parameters.AddWithValue("$manufacturer", point.Manufacturer);
        cmd.Parameters.AddWithValue("$model", point.Model);
        cmd.Parameters.AddWithValue("$firmware", point.Firmware);
        cmd.Parameters.AddWithValue("$sessionIp", point.SessionIp);
        cmd.Parameters.AddWithValue("$sessionStartedAt", point.SessionStartedAt);
        cmd.Parameters.AddWithValue("$eventCode", point.EventCode);
        cmd.Parameters.AddWithValue("$eventName", point.EventName);
        cmd.Parameters.AddWithValue("$deviceTime", point.DeviceTime);
        cmd.Parameters.AddWithValue("$receivedAt", point.ReceivedAt);
        cmd.Parameters.AddWithValue("$lat", point.Latitude);
        cmd.Parameters.AddWithValue("$lon", point.Longitude);
        cmd.Parameters.AddWithValue("$height", point.Height);
        cmd.Parameters.AddWithValue("$speed", point.Speed);
        cmd.Parameters.AddWithValue("$course", point.Course);
        cmd.Parameters.AddWithValue("$satCount", point.SatelliteCount);
        cmd.Parameters.AddWithValue("$batteryMv", point.BatteryMv);

        return (long)(await cmd.ExecuteScalarAsync())!;
    }
}
