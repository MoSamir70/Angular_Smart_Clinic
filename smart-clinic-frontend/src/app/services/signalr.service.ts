import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { QueueTicket, QueueStatus } from '../models/clinic.models';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private queueUpdatedSubject = new BehaviorSubject<{clinicId: number, doctorId: number, data: any} | null>(null);
  private ticketCalledSubject = new BehaviorSubject<{ticketId: number, position: number, doctorName: string} | null>(null);
  private positionChangedSubject = new BehaviorSubject<{ticketId: number, newPosition: number, waitMinutes: number} | null>(null);
  private patientCalledSubject = new BehaviorSubject<{patientId: number, ticketId: number, doctorName: string} | null>(null);

  queueUpdated$ = this.queueUpdatedSubject.asObservable();
  ticketCalled$ = this.ticketCalledSubject.asObservable();
  positionChanged$ = this.positionChangedSubject.asObservable();
  patientCalled$ = this.patientCalledSubject.asObservable();

  constructor() {
    this.initConnection();
  }

  private initConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5250/hubs/queue')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('QueueUpdated', (clinicId: number, doctorId: number, data: any) => {
      this.queueUpdatedSubject.next({ clinicId, doctorId, data });
    });

    this.hubConnection.on('TicketCalled', (ticketId: number, queuePosition: number, doctorName: string) => {
      this.ticketCalledSubject.next({ ticketId, position: queuePosition, doctorName });
    });

    this.hubConnection.on('PositionChanged', (ticketId: number, newPosition: number, estimatedWaitMinutes: number) => {
      this.positionChangedSubject.next({ ticketId, newPosition, waitMinutes: estimatedWaitMinutes });
    });

    this.hubConnection.on('PatientCalled', (patientId: number, ticketId: number, doctorName: string) => {
      this.patientCalledSubject.next({ patientId, ticketId, doctorName });
    });

    this.startConnection();
  }

  private async startConnection() {
    try {
      await this.hubConnection?.start();
      console.log('SignalR connected');
    } catch (error) {
      console.error('SignalR connection failed:', error);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  joinDoctorQueue(doctorId: number): void {
    this.hubConnection?.invoke('JoinDoctorQueue', doctorId);
  }

  leaveDoctorQueue(doctorId: number): void {
    this.hubConnection?.invoke('LeaveDoctorQueue', doctorId);
  }

  joinClinicQueue(clinicId: number): void {
    this.hubConnection?.invoke('JoinClinicQueue', clinicId);
  }

  leaveClinicQueue(clinicId: number): void {
    this.hubConnection?.invoke('LeaveClinicQueue', clinicId);
  }

  joinPatientNotifications(patientId: number): void {
    this.hubConnection?.invoke('JoinPatientNotifications', patientId);
  }

  leavePatientNotifications(patientId: number): void {
    this.hubConnection?.invoke('LeavePatientNotifications', patientId);
  }

  disconnect(): void {
    this.hubConnection?.stop();
  }
}