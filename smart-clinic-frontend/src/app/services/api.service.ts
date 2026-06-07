import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Patient, CreatePatientDto, Doctor, CreateDoctorDto,
  Clinic, CreateClinicDto, Appointment, CreateAppointmentDto,
  QueueTicket, CreateQueueTicketDto, QueueStatus, PatientQueueStatus
} from '../models/clinic.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5250/api';

  constructor(private http: HttpClient) {}

  // Patients
  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.baseUrl}/patients`);
  }

  getPatient(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.baseUrl}/patients/${id}`);
  }

  createPatient(dto: CreatePatientDto): Observable<Patient> {
    return this.http.post<Patient>(`${this.baseUrl}/patients`, dto);
  }

  updatePatient(id: number, dto: CreatePatientDto): Observable<Patient> {
    return this.http.put<Patient>(`${this.baseUrl}/patients/${id}`, dto);
  }

  deletePatient(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/patients/${id}`);
  }

  getPatientQueueStatus(patientId: number): Observable<PatientQueueStatus> {
    return this.http.get<PatientQueueStatus>(`${this.baseUrl}/patients/${patientId}/queue-status`);
  }

  joinQueue(patientId: number, dto: CreateQueueTicketDto): Observable<QueueTicket> {
    return this.http.post<QueueTicket>(`${this.baseUrl}/patients/${patientId}/join-queue`, dto);
  }

  // Clinics
  getClinics(): Observable<Clinic[]> {
    return this.http.get<Clinic[]>(`${this.baseUrl}/clinics`);
  }

  getClinic(id: number): Observable<Clinic> {
    return this.http.get<Clinic>(`${this.baseUrl}/clinics/${id}`);
  }

  createClinic(dto: CreateClinicDto): Observable<Clinic> {
    return this.http.post<Clinic>(`${this.baseUrl}/clinics`, dto);
  }

  // Doctors
  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors`);
  }

  getDoctorsByClinic(clinicId: number): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors/clinic/${clinicId}`);
  }

  getDoctor(id: number): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.baseUrl}/doctors/${id}`);
  }

  createDoctor(dto: CreateDoctorDto): Observable<Doctor> {
    return this.http.post<Doctor>(`${this.baseUrl}/doctors`, dto);
  }

  getDoctorQueue(doctorId: number): Observable<QueueStatus> {
    return this.http.get<QueueStatus>(`${this.baseUrl}/doctors/${doctorId}/queue`);
  }

  callNextPatient(doctorId: number): Observable<QueueTicket> {
    return this.http.post<QueueTicket>(`${this.baseUrl}/doctors/${doctorId}/call-next`, {});
  }

  // Appointments
  getAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/appointments`);
  }

  getAppointment(id: number): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.baseUrl}/appointments/${id}`);
  }

  getAppointmentsByPatient(patientId: number): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/appointments/patient/${patientId}`);
  }

  getAppointmentsByDoctor(doctorId: number): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/appointments/doctor/${doctorId}`);
  }

  createAppointment(dto: CreateAppointmentDto): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.baseUrl}/appointments`, dto);
  }

  cancelAppointment(id: number, reason?: string): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.baseUrl}/appointments/${id}/cancel`, reason);
  }

  rescheduleAppointment(id: number, newDateTime: string): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.baseUrl}/appointments/${id}/reschedule`, newDateTime);
  }

  // Queue
  getQueueDoctor(doctorId: number): Observable<QueueStatus> {
    return this.http.get<QueueStatus>(`${this.baseUrl}/queue/doctor/${doctorId}`);
  }

  getClinicQueue(clinicId: number): Observable<QueueTicket[]> {
    return this.http.get<QueueTicket[]>(`${this.baseUrl}/queue/clinic/${clinicId}`);
  }

  getQueueTicket(ticketId: number): Observable<QueueTicket> {
    return this.http.get<QueueTicket>(`${this.baseUrl}/queue/ticket/${ticketId}`);
  }

  joinQueuePublic(dto: CreateQueueTicketDto): Observable<QueueTicket> {
    return this.http.post<QueueTicket>(`${this.baseUrl}/queue/join`, dto);
  }

  completeTicket(ticketId: number): Observable<QueueTicket> {
    return this.http.post<QueueTicket>(`${this.baseUrl}/queue/${ticketId}/complete`, {});
  }

  cancelFromQueue(ticketId: number, reason?: string): Observable<QueueTicket> {
    return this.http.post<QueueTicket>(`${this.baseUrl}/queue/${ticketId}/cancel`, reason);
  }
}