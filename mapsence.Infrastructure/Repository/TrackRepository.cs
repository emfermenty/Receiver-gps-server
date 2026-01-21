using AutoMapper;
using mapsence.Application.DTO;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using mapsenceto.Application.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace mapsence.Infrastructure.Repositories;
public class TrackRepository(ApplicationContext context, IMapper _mapper) : ITrackRepository
{
    public async Task<Track?> FindTrackByIdAsync(int id)
    {
        var entity = await context.Tracks.Where(t => t.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<Track>(entity);
    }
    public async Task<Sensor?> FindSensorByUserIdAsync(int id)
    {
        var entity = await context.Sensors.Where(s => s.UserId == id).FirstOrDefaultAsync();
        return _mapper.Map<Sensor>(entity);
    }
    
    public async Task<List<Track>> FindTrackBySensorIdAsync(int sensorId)
    {
        var entities = await context.Tracks.Where(s => s.SensorId == sensorId).ToListAsync();
        return _mapper.Map<List<Track>>(entities);
    }

    public async Task<List<Track>> FindTrackByUserIdAsync(int userId)
    {
        var entities =  await context.Tracks.Where(s => s.UserId == userId).ToListAsync();
        return _mapper.Map<List<Track>>(entities);
    }

    public async Task PrcDeleteTrackAsync(int id)
    {
        await context.Database.ExecuteSqlRawAsync("CALL zxc.prc_delete_track({0})", id);
    }

    public async Task PrcEditTrackAsync(int id, string name)
    {
        await context.Database.ExecuteSqlRawAsync("CALL zxc.prc_edit_track({0}, {1})", id, name);
    }

    public async Task PrcStopTrackAsync(int id, DateTime dateStop)
    {
        var idParam = new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id };
        var dateParam = new NpgsqlParameter("dateStop", NpgsqlTypes.NpgsqlDbType.Timestamp)
        {
            Value = DateTime.SpecifyKind(dateStop, DateTimeKind.Unspecified) 
        };

        await context.Database.ExecuteSqlRawAsync(
            "CALL zxc.prc_track_end(@id, @dateStop)",
            idParam, dateParam
        );
    }

    public async Task<int> PrcStartTrackAsync(StartTrackRequest request)
    {
        await using var conn = (NpgsqlConnection)context.Database.GetDbConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT zxc.fnc_track_action(@p_date, @p_sensor, @p_user, @p_name);", conn);

        cmd.Parameters.AddWithValue("p_date", NpgsqlTypes.NpgsqlDbType.Timestamp)
            .Value = DateTime.SpecifyKind(request.DateStart, DateTimeKind.Unspecified);
        cmd.Parameters.AddWithValue("p_sensor", NpgsqlTypes.NpgsqlDbType.Integer)
            .Value = request.SensorId;
        cmd.Parameters.AddWithValue("p_user", NpgsqlTypes.NpgsqlDbType.Integer)
            .Value = request.UserId;
        cmd.Parameters.AddWithValue("p_name", NpgsqlTypes.NpgsqlDbType.Text)
            .Value = request.Name;

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    public async Task PrcComebackTrackAsync(int id)
    {
        await context.Database.ExecuteSqlRawAsync("CALL zxc.prc_track_comeback({0})", id);
    }
}