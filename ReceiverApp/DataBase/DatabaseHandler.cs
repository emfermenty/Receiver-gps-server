using System.Configuration;
using Npgsql;

namespace Receiver_server.Database;

public class DataBaseHandler
{
    private string connectionString = $"Host={ConfigurationManager.AppSettings.Get("db_host")};" +
                                  $"Username={ConfigurationManager.AppSettings.Get("db_user")};" +
                                  $"Password={ConfigurationManager.AppSettings.Get("db_password")};" +
                                  $"Database={ConfigurationManager.AppSettings.Get("db_name")}";

    public async Task CallProcedureOccupancyAsync(GeoData geoData)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            const string query = @"CALL zxc.occupancy(
                                @id,
                                @lat,
                                @lon,
                                @height,
                                @speed,
                                @dt,
                                @course,
                                @vBattery,
                                @satCount,
                                @UserId
                            );";

           await using var cmd = new NpgsqlCommand(query, conn);

           cmd.Parameters.AddWithValue("id", 1);
           cmd.Parameters.AddWithValue("lat", geoData.Latitude);
           cmd.Parameters.AddWithValue("lon", geoData.Longitude);
           cmd.Parameters.AddWithValue("height", geoData.Height);
           cmd.Parameters.AddWithValue("speed", geoData.Speed);
           
           cmd.Parameters.Add("dt", NpgsqlTypes.NpgsqlDbType.Timestamp)
               .Value = DateTime.SpecifyKind(geoData.DateTime, DateTimeKind.Unspecified);

           cmd.Parameters.AddWithValue("course", geoData.Course);
           cmd.Parameters.AddWithValue("vBattery", geoData.VBattery);
           cmd.Parameters.AddWithValue("satCount", geoData.SatCount);
           cmd.Parameters.AddWithValue("UserId", geoData.UserId);

           await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при вызове процедуры: {ex.Message}");
        }
    }

    public async Task<int> CheckAvailableImeiAsync(string imei)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            const string query = @"SELECT zxc.fnc_check_available_imei('@imei');";
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("imei", imei);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка проверки наличия imei в базе данных");
        }
    }
}