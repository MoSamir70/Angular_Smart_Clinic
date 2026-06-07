export interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth: string;
  age: number;
  phone: string;
  email: string;
  address?: string;
  isVip: boolean;
  notes?: string;
  createdAt: string;
}

export interface CreatePatientDto {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  phone: string;
  email: string;
  address?: string;
  isVip: boolean;
  notes?: string;
}

export interface Doctor {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  specialty: string;
  licenseNumber: string;
  phone: string;
  email: string;
  isAvailable: boolean;
  estimatedConsultationMinutes: number;
  clinicId: number;
  clinicName: string;
  queueCount: number;
  currentTicketId?: number;
  createdAt: string;
}

export interface CreateDoctorDto {
  firstName: string;
  lastName: string;
  specialty: string;
  licenseNumber: string;
  phone: string;
  email: string;
  estimatedConsultationMinutes: number;
  clinicId: number;
}

export interface Clinic {
  id: number;
  name: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
  estimatedWaitTimePerPatient: number;
  workingHoursStart: string;
  workingHoursEnd: string;
  doctorCount: number;
  createdAt: string;
}

export interface CreateClinicDto {
  name: string;
  address: string;
  phone: string;
  email: string;
  estimatedWaitTimePerPatient: number;
  workingHoursStart: string;
  workingHoursEnd: string;
}

export interface Appointment {
  id: number;
  scheduledDateTime: string;
  startTime?: string;
  endTime?: string;
  status: string;
  statusText: string;
  reason?: string;
  notes?: string;
  queueTicketId?: number;
  patientId: number;
  patientName: string;
  patientPhone: string;
  doctorId: number;
  doctorName: string;
  doctorSpecialty: string;
  clinicId: number;
  clinicName: string;
  createdAt: string;
}

export interface CreateAppointmentDto {
  scheduledDateTime: string;
  patientId: number;
  doctorId: number;
  clinicId: number;
  reason?: string;
  notes?: string;
}

export interface QueueTicket {
  id: number;
  ticketNumber: number;
  status: string;
  statusText: string;
  position: number;
  checkInTime: string;
  calledTime?: string;
  startTime?: string;
  endTime?: string;
  estimatedStartTime?: string;
  estimatedWaitMinutes: number;
  calledByDoctorId?: number;
  calledByDoctorName?: string;
  isVip: boolean;
  notes?: string;
  patientId: number;
  patientName: string;
  patientPhone: string;
  doctorId: number;
  doctorName: string;
  doctorSpecialty: string;
  clinicId: number;
  clinicName: string;
}

export interface CreateQueueTicketDto {
  patientId: number;
  doctorId: number;
  clinicId: number;
  isVip: boolean;
  notes?: string;
  appointmentId?: number;
}

export interface QueueStatus {
  doctorId: number;
  doctorName: string;
  doctorSpecialty: string;
  queueCount: number;
  currentTicketId?: number;
  currentPatientName?: string;
  nextPatient?: QueueTicket;
  waitingList: QueueTicket[];
  averageWaitMinutes: number;
}

export interface PatientQueueStatus {
  ticketId: number;
  ticketNumber: number;
  status: string;
  position: number;
  estimatedWaitMinutes: number;
  estimatedStartTime?: string;
  doctorId: number;
  doctorName: string;
  clinicId: number;
  clinicName: string;
  isMyTurn: boolean;
}