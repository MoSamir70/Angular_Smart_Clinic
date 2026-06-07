using Microsoft.AspNetCore.SignalR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces.Services;

namespace SmartClinic.API.Hubs;

public class QueueHub : Hub<IQueueHubClient>
{
    public async Task JoinDoctorQueue(int doctorId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"doctor-{doctorId}");
    }

    public async Task LeaveDoctorQueue(int doctorId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"doctor-{doctorId}");
    }

    public async Task JoinClinicQueue(int clinicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"clinic-{clinicId}");
    }

    public async Task LeaveClinicQueue(int clinicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"clinic-{clinicId}");
    }

    public async Task JoinPatientNotifications(int patientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"patient-{patientId}");
    }

    public async Task LeavePatientNotifications(int patientId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patient-{patientId}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}