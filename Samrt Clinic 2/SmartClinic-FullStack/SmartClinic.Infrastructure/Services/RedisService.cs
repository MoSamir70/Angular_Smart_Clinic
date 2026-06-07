using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;
using StackExchange.Redis;

namespace SmartClinic.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisService> _logger;
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
    {
        _redis = redis;
        _logger = logger;
        _db = redis.GetDatabase();
    }

    private string QueueTicketKey(int ticketId) => $"queueticket:{ticketId}";
    private string DoctorQueueKey(int doctorId) => $"doctorqueue:{doctorId}";
    private string NextPatientKey(int doctorId) => $"nextpatient:{doctorId}";
    private string ClinicQueueKey(int clinicId) => $"clinicqueue:{clinicId}";
    private string AppointmentKey(int appointmentId) => $"appointment:{appointmentId}";

    public async Task SetQueueTicketAsync(QueueTicket ticket)
    {
        var key = QueueTicketKey(ticket.Id);
        var json = JsonSerializer.Serialize(ticket);
        await _db.StringSetAsync(key, json, TimeSpan.FromHours(2));
    }

    public async Task<QueueTicket?> GetQueueTicketAsync(int ticketId)
    {
        var key = QueueTicketKey(ticketId);
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<QueueTicket>(json!);
    }

    public async Task RemoveQueueTicketAsync(int ticketId)
    {
        var key = QueueTicketKey(ticketId);
        await _db.KeyDeleteAsync(key);
    }

    public async Task UpdateQueuePositionAsync(int doctorId, int ticketId, int newPosition)
    {
        var ticket = await GetQueueTicketAsync(ticketId);
        if (ticket != null)
        {
            ticket.Position = newPosition;
            await SetQueueTicketAsync(ticket);
        }
    }

    public async Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId)
    {
        var key = DoctorQueueKey(doctorId);
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return Enumerable.Empty<QueueTicket>();

        return JsonSerializer.Deserialize<List<QueueTicket>>(json!) ?? new List<QueueTicket>();
    }

    public async Task SetNextPatientCacheAsync(int doctorId, int? ticketId)
    {
        var key = NextPatientKey(doctorId);
        if (ticketId.HasValue)
            await _db.StringSetAsync(key, ticketId.Value.ToString(), TimeSpan.FromHours(2));
        else
            await _db.StringSetAsync(key, "", TimeSpan.FromHours(2));
    }

    public async Task<int?> GetNextPatientCacheAsync(int doctorId)
    {
        var key = NextPatientKey(doctorId);
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty || string.IsNullOrEmpty(value))
            return null;

        return int.Parse(value!);
    }

    public async Task SetClinicQueueCacheAsync(int clinicId, IEnumerable<QueueTicket> tickets)
    {
        var key = ClinicQueueKey(clinicId);
        var json = JsonSerializer.Serialize(tickets);
        await _db.StringSetAsync(key, json, TimeSpan.FromHours(1));
    }

    public async Task<IEnumerable<QueueTicket>> GetClinicQueueCacheAsync(int clinicId)
    {
        var key = ClinicQueueKey(clinicId);
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return Enumerable.Empty<QueueTicket>();

        return JsonSerializer.Deserialize<List<QueueTicket>>(json!) ?? new List<QueueTicket>();
    }

    public async Task InvalidateClinicCacheAsync(int clinicId)
    {
        var key = ClinicQueueKey(clinicId);
        await _db.KeyDeleteAsync(key);
    }

    public async Task InvalidateDoctorQueueCacheAsync(int doctorId)
    {
        var key = DoctorQueueKey(doctorId);
        await _db.KeyDeleteAsync(key);
    }

    public async Task CacheAppointmentAsync(Appointment appointment)
    {
        var key = AppointmentKey(appointment.Id);
        var json = JsonSerializer.Serialize(appointment);
        await _db.StringSetAsync(key, json, TimeSpan.FromHours(2));
    }

    public async Task<Appointment?> GetCachedAppointmentAsync(int appointmentId)
    {
        var key = AppointmentKey(appointmentId);
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<Appointment>(json!);
    }

    public async Task InvalidateAppointmentCacheAsync(int appointmentId)
    {
        var key = AppointmentKey(appointmentId);
        await _db.KeyDeleteAsync(key);
    }
}