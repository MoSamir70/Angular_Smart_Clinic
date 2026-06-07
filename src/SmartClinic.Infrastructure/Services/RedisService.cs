//using System.Text.Json;
//using Microsoft.Extensions.Logging;
//using SmartClinic.Application.Interfaces.Services;
//using SmartClinic.Domain.Entities;
////using StackExchange.Redis;

//namespace SmartClinic.Infrastructure.Services;

//public class RedisService : IRedisService
//{
//    private readonly IConnectionMultiplexer _redis;
//    private readonly ILogger<RedisService> _logger;
//    private readonly IDatabase _db;

//    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
//    {
//        _redis = redis;
//        _logger = logger;
//        _db = redis.GetDatabase();
//    }

//    private string QueueTicketKey(int ticketId) => $"queueticket:{ticketId}";
//    private string DoctorQueueKey(int doctorId) => $"doctorqueue:{doctorId}";
//    private string NextPatientKey(int doctorId) => $"nextpatient:{doctorId}";
//    private string ClinicQueueKey(int clinicId) => $"clinicqueue:{clinicId}";
//    private string AppointmentKey(int appointmentId) => $"appointment:{appointmentId}";

//    public async Task SetQueueTicketAsync(QueueTicket ticket)
//    {
//        var key = QueueTicketKey(ticket.Id);
//        var json = JsonSerializer.Serialize(ticket);
//        await _db.StringSetAsync(key, json, TimeSpan.FromHours(2));
//    }

//    public async Task<QueueTicket?> GetQueueTicketAsync(int ticketId)
//    {
//        var key = QueueTicketKey(ticketId);
//        var json = await _db.StringGetAsync(key);
//        if (json.IsNullOrEmpty)
//            return null;

//        return JsonSerializer.Deserialize<QueueTicket>(json!);
//    }

//    public async Task RemoveQueueTicketAsync(int ticketId)
//    {
//        var key = QueueTicketKey(ticketId);
//        await _db.KeyDeleteAsync(key);
//    }

//    public async Task UpdateQueuePositionAsync(int doctorId, int ticketId, int newPosition)
//    {
//        var ticket = await GetQueueTicketAsync(ticketId);
//        if (ticket != null)
//        {
//            ticket.Position = newPosition;
//            await SetQueueTicketAsync(ticket);
//        }
//    }

//    public async Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId)
//    {
//        var key = DoctorQueueKey(doctorId);
//        var json = await _db.StringGetAsync(key);
//        if (json.IsNullOrEmpty)
//            return Enumerable.Empty<QueueTicket>();

//        return JsonSerializer.Deserialize<List<QueueTicket>>(json!) ?? new List<QueueTicket>();
//    }

//    public async Task SetNextPatientCacheAsync(int doctorId, int? ticketId)
//    {
//        var key = NextPatientKey(doctorId);
//        if (ticketId.HasValue)
//            await _db.StringSetAsync(key, ticketId.Value.ToString(), TimeSpan.FromHours(2));
//        else
//            await _db.StringSetAsync(key, "", TimeSpan.FromHours(2));
//    }

//    public async Task<int?> GetNextPatientCacheAsync(int doctorId)
//    {
//        var key = NextPatientKey(doctorId);
//        var value = await _db.StringGetAsync(key);
//        if (value.IsNullOrEmpty || string.IsNullOrEmpty(value))
//            return null;

//        return int.Parse(value!);
//    }

//    public async Task SetClinicQueueCacheAsync(int clinicId, IEnumerable<QueueTicket> tickets)
//    {
//        var key = ClinicQueueKey(clinicId);
//        var json = JsonSerializer.Serialize(tickets);
//        await _db.StringSetAsync(key, json, TimeSpan.FromHours(1));
//    }

//    public async Task<IEnumerable<QueueTicket>> GetClinicQueueCacheAsync(int clinicId)
//    {
//        var key = ClinicQueueKey(clinicId);
//        var json = await _db.StringGetAsync(key);
//        if (json.IsNullOrEmpty)
//            return Enumerable.Empty<QueueTicket>();

//        return JsonSerializer.Deserialize<List<QueueTicket>>(json!) ?? new List<QueueTicket>();
//    }

//    public async Task InvalidateClinicCacheAsync(int clinicId)
//    {
//        var key = ClinicQueueKey(clinicId);
//        await _db.KeyDeleteAsync(key);
//    }

//    public async Task InvalidateDoctorQueueCacheAsync(int doctorId)
//    {
//        var key = DoctorQueueKey(doctorId);
//        await _db.KeyDeleteAsync(key);
//    }

//    public async Task CacheAppointmentAsync(Appointment appointment)
//    {
//        var key = AppointmentKey(appointment.Id);
//        var json = JsonSerializer.Serialize(appointment);
//        await _db.StringSetAsync(key, json, TimeSpan.FromHours(2));
//    }

//    public async Task<Appointment?> GetCachedAppointmentAsync(int appointmentId)
//    {
//        var key = AppointmentKey(appointmentId);
//        var json = await _db.StringGetAsync(key);
//        if (json.IsNullOrEmpty)
//            return null;

//        return JsonSerializer.Deserialize<Appointment>(json!);
//    }

//    public async Task InvalidateAppointmentCacheAsync(int appointmentId)
//    {
//        var key = AppointmentKey(appointmentId);
//        await _db.KeyDeleteAsync(key);
//    }
//}


//---------------------------------------------------------------------

//using System.Text.Json;
//using Microsoft.Extensions.Logging;
//using SmartClinic.Application.Interfaces.Services;
//using SmartClinic.Domain.Entities;

//namespace SmartClinic.Infrastructure.Services;

//public class RedisService : IRedisService
//{
//    private readonly ILogger<RedisService> _logger;

//    // Constructor بدون Redis
//    public RedisService(ILogger<RedisService> logger)
//    {
//        _logger = logger;
//        _logger.LogWarning("RedisService is running in fallback mode (no Redis connection)");
//    }

//    private string QueueTicketKey(int ticketId) => $"queueticket:{ticketId}";
//    private string DoctorQueueKey(int doctorId) => $"doctorqueue:{doctorId}";
//    private string NextPatientKey(int doctorId) => $"nextpatient:{doctorId}";
//    private string ClinicQueueKey(int clinicId) => $"clinicqueue:{clinicId}";
//    private string AppointmentKey(int appointmentId) => $"appointment:{appointmentId}";

//    // جميع الـ Methods هترجع قيم فارغة أو تعمل Log فقط (Fallback)
//    public async Task SetQueueTicketAsync(QueueTicket ticket)
//    {
//        _logger.LogWarning("Redis is disabled. SetQueueTicketAsync skipped for ticket {TicketId}", ticket.Id);
//        await Task.CompletedTask;
//    }

//    public async Task<QueueTicket?> GetQueueTicketAsync(int ticketId)
//    {
//        _logger.LogWarning("Redis is disabled. GetQueueTicketAsync skipped for ticket {TicketId}", ticketId);
//        return null;
//    }

//    public async Task RemoveQueueTicketAsync(int ticketId)
//    {
//        _logger.LogWarning("Redis is disabled. RemoveQueueTicketAsync skipped for ticket {TicketId}", ticketId);
//        await Task.CompletedTask;
//    }

//    public async Task UpdateQueuePositionAsync(int doctorId, int ticketId, int newPosition)
//    {
//        _logger.LogWarning("Redis is disabled. UpdateQueuePositionAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId)
//    {
//        _logger.LogWarning("Redis is disabled. GetActiveQueueAsync skipped for doctor {DoctorId}", doctorId);
//        return Enumerable.Empty<QueueTicket>();
//    }

//    public async Task SetNextPatientCacheAsync(int doctorId, int? ticketId)
//    {
//        _logger.LogWarning("Redis is disabled. SetNextPatientCacheAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task<int?> GetNextPatientCacheAsync(int doctorId)
//    {
//        _logger.LogWarning("Redis is disabled. GetNextPatientCacheAsync skipped");
//        return null;
//    }

//    public async Task SetClinicQueueCacheAsync(int clinicId, IEnumerable<QueueTicket> tickets)
//    {
//        _logger.LogWarning("Redis is disabled. SetClinicQueueCacheAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task<IEnumerable<QueueTicket>> GetClinicQueueCacheAsync(int clinicId)
//    {
//        _logger.LogWarning("Redis is disabled. GetClinicQueueCacheAsync skipped");
//        return Enumerable.Empty<QueueTicket>();
//    }

//    public async Task InvalidateClinicCacheAsync(int clinicId)
//    {
//        _logger.LogWarning("Redis is disabled. InvalidateClinicCacheAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task InvalidateDoctorQueueCacheAsync(int doctorId)
//    {
//        _logger.LogWarning("Redis is disabled. InvalidateDoctorQueueCacheAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task CacheAppointmentAsync(Appointment appointment)
//    {
//        _logger.LogWarning("Redis is disabled. CacheAppointmentAsync skipped");
//        await Task.CompletedTask;
//    }

//    public async Task<Appointment?> GetCachedAppointmentAsync(int appointmentId)
//    {
//        _logger.LogWarning("Redis is disabled. GetCachedAppointmentAsync skipped");
//        return null;
//    }

//    public async Task InvalidateAppointmentCacheAsync(int appointmentId)
//    {
//        _logger.LogWarning("Redis is disabled. InvalidateAppointmentCacheAsync skipped");
//        await Task.CompletedTask;
//    }
//}


//-------------------------------

using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> _logger;

    public RedisService(ILogger<RedisService> logger)
    {
        _logger = logger;
        _logger.LogWarning("RedisService is running in FALLBACK MODE (Redis package is disabled)");
    }

    // ====================== Helper Keys ======================
    private string QueueTicketKey(int ticketId) => $"queueticket:{ticketId}";
    private string DoctorQueueKey(int doctorId) => $"doctorqueue:{doctorId}";
    private string NextPatientKey(int doctorId) => $"nextpatient:{doctorId}";
    private string ClinicQueueKey(int clinicId) => $"clinicqueue:{clinicId}";
    private string AppointmentKey(int appointmentId) => $"appointment:{appointmentId}";

    // ====================== QueueTicket Methods ======================
    public async Task SetQueueTicketAsync(QueueTicket ticket)
    {
        _logger.LogWarning("Redis disabled → SetQueueTicketAsync skipped for ticket {TicketId}", ticket?.Id);
        await Task.CompletedTask;
    }

    public async Task<QueueTicket?> GetQueueTicketAsync(int ticketId)
    {
        _logger.LogWarning("Redis disabled → GetQueueTicketAsync skipped for ticket {TicketId}", ticketId);
        return null;
    }

    public async Task RemoveQueueTicketAsync(int ticketId)
    {
        _logger.LogWarning("Redis disabled → RemoveQueueTicketAsync skipped for ticket {TicketId}", ticketId);
        await Task.CompletedTask;
    }

    public async Task UpdateQueuePositionAsync(int doctorId, int ticketId, int newPosition)
    {
        _logger.LogWarning("Redis disabled → UpdateQueuePositionAsync skipped");
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId)
    {
        _logger.LogWarning("Redis disabled → GetActiveQueueAsync skipped for doctor {DoctorId}", doctorId);
        return Enumerable.Empty<QueueTicket>();
    }

    // ====================== Next Patient ======================
    public async Task SetNextPatientCacheAsync(int doctorId, int? ticketId)
    {
        _logger.LogWarning("Redis disabled → SetNextPatientCacheAsync skipped for doctor {DoctorId}", doctorId);
        await Task.CompletedTask;
    }

    public async Task<int?> GetNextPatientCacheAsync(int doctorId)
    {
        _logger.LogWarning("Redis disabled → GetNextPatientCacheAsync skipped for doctor {DoctorId}", doctorId);
        return null;
    }

    // ====================== Clinic Queue ======================
    public async Task SetClinicQueueCacheAsync(int clinicId, IEnumerable<QueueTicket> tickets)
    {
        _logger.LogWarning("Redis disabled → SetClinicQueueCacheAsync skipped for clinic {ClinicId}", clinicId);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<QueueTicket>> GetClinicQueueCacheAsync(int clinicId)
    {
        _logger.LogWarning("Redis disabled → GetClinicQueueCacheAsync skipped for clinic {ClinicId}", clinicId);
        return Enumerable.Empty<QueueTicket>();
    }

    public async Task InvalidateClinicCacheAsync(int clinicId)
    {
        _logger.LogWarning("Redis disabled → InvalidateClinicCacheAsync skipped for clinic {ClinicId}", clinicId);
        await Task.CompletedTask;
    }

    public async Task InvalidateDoctorQueueCacheAsync(int doctorId)
    {
        _logger.LogWarning("Redis disabled → InvalidateDoctorQueueCacheAsync skipped for doctor {DoctorId}", doctorId);
        await Task.CompletedTask;
    }

    // ====================== Appointment ======================
    public async Task CacheAppointmentAsync(Appointment appointment)
    {
        _logger.LogWarning("Redis disabled → CacheAppointmentAsync skipped for appointment {AppointmentId}", appointment?.Id);
        await Task.CompletedTask;
    }

    public async Task<Appointment?> GetCachedAppointmentAsync(int appointmentId)
    {
        _logger.LogWarning("Redis disabled → GetCachedAppointmentAsync skipped for appointment {AppointmentId}", appointmentId);
        return null;
    }

    public async Task InvalidateAppointmentCacheAsync(int appointmentId)
    {
        _logger.LogWarning("Redis disabled → InvalidateAppointmentCacheAsync skipped for appointment {AppointmentId}", appointmentId);
        await Task.CompletedTask;
    }
}