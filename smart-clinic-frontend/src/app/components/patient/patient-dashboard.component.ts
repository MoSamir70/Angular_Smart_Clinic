import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { SignalRService } from '../../services/signalr.service';
import { Patient, Doctor, Clinic, QueueTicket, PatientQueueStatus, CreateQueueTicketDto } from '../../models/clinic.models';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatButtonModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatTableModule,
    MatListModule, MatIconModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  template: `
    <div class="patient-dashboard">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Patient Dashboard</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div *ngIf="!currentPatient">
            <h3>Register / Login</h3>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>First Name</mat-label>
              <input matInput [(ngModel)]="newPatient.firstName">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Last Name</mat-label>
              <input matInput [(ngModel)]="newPatient.lastName">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Phone</mat-label>
              <input matInput [(ngModel)]="newPatient.phone">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput [(ngModel)]="newPatient.email" type="email">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Date of Birth</mat-label>
              <input matInput [(ngModel)]="newPatient.dateOfBirth" type="date">
            </mat-form-field>
            <button mat-raised-button color="primary" (click)="registerPatient()">Register</button>
          </div>

          <div *ngIf="currentPatient" class="patient-info">
            <h3>Welcome, {{ currentPatient.fullName }}!</h3>
            
            <mat-card *ngIf="queueStatus" class="queue-status-card">
              <mat-card-header>
                <mat-card-title>Your Queue Status</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <div class="status-item">
                  <span class="label">Ticket #:</span>
                  <span class="value">{{ queueStatus.ticketNumber }}</span>
                </div>
                <div class="status-item">
                  <span class="label">Position:</span>
                  <span class="value">{{ queueStatus.position }}</span>
                </div>
                <div class="status-item">
                  <span class="label">Estimated Wait:</span>
                  <span class="value">{{ queueStatus.estimatedWaitMinutes }} minutes</span>
                </div>
                <div class="status-item">
                  <span class="label">Doctor:</span>
                  <span class="value">{{ queueStatus.doctorName }}</span>
                </div>
                <div class="status-item">
                  <span class="label">Status:</span>
                  <span class="value" [class.called]="queueStatus.isMyTurn">{{ queueStatus.status }}</span>
                </div>
                <div *ngIf="queueStatus.isMyTurn" class="call-alert">
                  <mat-icon>notifications_active</mat-icon>
                  <span>DRIVER IS CALLING YOU!</span>
                </div>
              </mat-card-content>
            </mat-card>

            <mat-card *ngIf="!queueStatus" class="join-queue-card">
              <mat-card-header>
                <mat-card-title>Join Queue</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Select Clinic</mat-label>
                  <mat-select [(ngModel)]="selectedClinicId" (selectionChange)="loadDoctors()">
                    <mat-option *ngFor="let clinic of clinics" [value]="clinic.id">
                      {{ clinic.name }}
                    </mat-option>
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width" *ngIf="selectedClinicId">
                  <mat-label>Select Doctor</mat-label>
                  <mat-select [(ngModel)]="selectedDoctorId">
                    <mat-option *ngFor="let doctor of doctors" [value]="doctor.id">
                      Dr. {{ doctor.fullName }} - {{ doctor.specialty }} (Queue: {{ doctor.queueCount }})
                    </mat-option>
                  </mat-select>
                </mat-form-field>

                <button mat-raised-button color="primary" 
                        [disabled]="!selectedDoctorId" 
                        (click)="joinQueue()">
                  Join Queue
                </button>
              </mat-card-content>
            </mat-card>

            <button mat-raised-button color="warn" *ngIf="queueStatus" (click)="leaveQueue()">
              Leave Queue
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .patient-dashboard { padding: 20px; }
    .full-width { width: 100%; margin-bottom: 10px; }
    .patient-info h3 { margin-bottom: 20px; }
    .queue-status-card { margin: 20px 0; background: #e3f2fd; }
    .status-item { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #ddd; }
    .status-item .label { font-weight: bold; }
    .status-item .value.called { color: green; font-weight: bold; font-size: 1.2em; }
    .call-alert { display: flex; align-items: center; gap: 10px; color: #d32f2f; font-weight: bold; margin-top: 15px; padding: 15px; background: #ffebee; border-radius: 5px; }
    .join-queue-card { margin: 20px 0; }
  `]
})
export class PatientDashboardComponent implements OnInit, OnDestroy {
  currentPatient: Patient | null = null;
  newPatient = { firstName: '', lastName: '', phone: '', email: '', dateOfBirth: '' };
  
  clinics: Clinic[] = [];
  doctors: Doctor[] = [];
  selectedClinicId: number | null = null;
  selectedDoctorId: number | null = null;
  
  queueStatus: PatientQueueStatus | null = null;
  pollingInterval: any;

  constructor(
    private api: ApiService,
    private signalR: SignalRService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadClinics();
    
    this.signalR.patientCalled$.subscribe(data => {
      if (data && this.currentPatient && this.queueStatus) {
        this.snackBar.open('Doctor is calling you!', 'OK', { duration: 5000 });
        this.refreshQueueStatus();
      }
    });

    this.signalR.positionChanged$.subscribe(data => {
      if (data && this.queueStatus && data.ticketId === this.queueStatus.ticketId) {
        this.queueStatus.position = data.newPosition;
        this.queueStatus.estimatedWaitMinutes = data.waitMinutes;
      }
    });
  }

  ngOnDestroy() {
    if (this.pollingInterval) clearInterval(this.pollingInterval);
  }

  loadClinics() {
    this.api.getClinics().subscribe(data => this.clinics = data);
  }

  loadDoctors() {
    if (this.selectedClinicId) {
      this.api.getDoctorsByClinic(this.selectedClinicId).subscribe(data => this.doctors = data);
    }
  }

  registerPatient() {
    this.api.createPatient(this.newPatient as any).subscribe({
      next: (patient) => {
        this.currentPatient = patient;
        this.snackBar.open('Registered successfully!', 'OK', { duration: 3000 });
      },
      error: () => this.snackBar.open('Registration failed', 'OK', { duration: 3000 })
    });
  }

  joinQueue() {
    if (!this.currentPatient || !this.selectedDoctorId || !this.selectedClinicId) return;
    
    const dto: CreateQueueTicketDto = {
      patientId: this.currentPatient.id,
      doctorId: this.selectedDoctorId,
      clinicId: this.selectedClinicId,
      isVip: this.currentPatient.isVip
    };

    this.api.joinQueue(this.currentPatient.id, dto).subscribe({
      next: () => {
        this.signalR.joinPatientNotifications(this.currentPatient!.id);
        this.startPolling();
        this.snackBar.open('Joined queue successfully!', 'OK', { duration: 3000 });
      },
      error: (err) => this.snackBar.open(err.error || 'Failed to join queue', 'OK', { duration: 3000 })
    });
  }

  startPolling() {
    this.refreshQueueStatus();
    this.pollingInterval = setInterval(() => this.refreshQueueStatus(), 10000);
  }

  refreshQueueStatus() {
    if (this.currentPatient) {
      this.api.getPatientQueueStatus(this.currentPatient.id).subscribe({
        next: (status) => this.queueStatus = status,
        error: () => this.queueStatus = null
      });
    }
  }

  leaveQueue() {
    if (this.queueStatus) {
      this.api.cancelFromQueue(this.queueStatus.ticketId).subscribe({
        next: () => {
          this.queueStatus = null;
          if (this.pollingInterval) clearInterval(this.pollingInterval);
          this.snackBar.open('Left queue', 'OK', { duration: 3000 });
        },
        error: () => this.snackBar.open('Failed to leave queue', 'OK', { duration: 3000 })
      });
    }
  }
}