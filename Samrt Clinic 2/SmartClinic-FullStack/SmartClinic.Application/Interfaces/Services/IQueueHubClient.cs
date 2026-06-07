namespace SmartClinic.Application.Interfaces.Services;

public interface IQueueHubClient
{
    Task QueueUpdated(int clinicId, int doctorId, object queueData);
    Task TicketCalled(int ticketId, int queuePosition, string doctorName);
    Task PositionChanged(int ticketId, int newPosition, int estimatedWaitMinutes);
    Task PatientCalled(int patientId, int ticketId, string doctorName);
    Task QueueCancelled(int ticketId);
    Task NotificationReceived(int patientId, object notification);
}