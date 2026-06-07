import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ApiService } from '../../services/api.service';
import { SignalRService } from '../../services/signalr.service';
import { Doctor, QueueStatus, QueueTicket } from '../../models/clinic.models';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatButtonModule, MatListModule, MatIconModule, MatBadgeModule, MatSnackBarModule, MatSelectModule, MatFormFieldModule],
  template: `
    <div class="doctor-dashboard">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Doctor Dashboard</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <mat-form-field appearance="outline" style="width: 100%;">
            <mat-label>Select Doctor</mat-label>
            <mat-select [(ngModel)]="selectedDoctorId" (selectionChange)="loadDoctorQueue()">
              <mat-option *ngFor="let doctor of doctors" [value]="doctor.id">
                Dr. {{ doctor.fullName }} - {{ doctor.specialty }}
              </mat-option>
            </mat-select>
          </mat-form-field>

          <div *ngIf="queueStatus" class="queue-info">
            <div class="stats-row">
              <mat-card class="stat-card">
                <mat-card-content>
                  <div class="stat-value">{{ queueStatus.queueCount }}</div>
                  <div class="stat-label">Patients in Queue</div>
                </mat-card-content>
              </mat-card>
              <mat-card class="stat-card">
                <mat-card-content>
                  <div class="stat-value">{{ queueStatus.averageWaitMinutes }}</div>
                  <div class="stat-label">Avg Wait (min)</div>
                </mat-card-content>
              </mat-card>
            </div>

            <mat-card *ngIf="queueStatus.currentTicketId" class="current-patient-card">
              <mat-card-header>
                <mat-card-title>Currently with Patient</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <div class="current-patient">
                  <mat-icon>person</mat-icon>
                  <span>{{ queueStatus.currentPatientName }}</span>
                </div>
              </mat-card-content>
            </mat-card>

            <mat-card class="waiting-list-card">
              <mat-card-header>
                <mat-card-title>Waiting List</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <mat-list>
                  <mat-list-item *ngFor="let ticket of queueStatus.waitingList; let i = index" 
                                 [class.current]="ticket.status === 'Called'"
                                 [class.vip]="ticket.isVip">
                    <span matListItemTitle>
                      <mat-icon *ngIf="ticket.isVip">star</mat-icon>
                      Ticket #{{ ticket.ticketNumber }} - {{ ticket.patientName }}
                    </span>
                    <span matListItemLine>
                      Position: {{ ticket.position }} | Wait: {{ ticket.estimatedWaitMinutes }} min
                    </span>
                    <button mat-icon-button *ngIf="ticket.status === 'Waiting'" (click)="markPatientCalled(ticket.id)" color="primary">
                      <mat-icon>notifications</mat-icon>
                    </button>
                    <button mat-icon-button *ngIf="ticket.status === 'Called'" (click)="completePatient(ticket.id)" color="accent">
                      <mat-icon>check_circle</mat-icon>
                    </button>
                  </mat-list-item>
                  <p *ngIf="queueStatus.waitingList.length === 0" class="no-patients">No patients waiting</p>
                </mat-list>
              </mat-card-content>
            </mat-card>

            <button mat-raised-button color="primary" (click)="callNextPatient()" [disabled]="queueStatus.queueCount === 0" class="call-next-btn">
              <mat-icon>arrow_forward</mat-icon> Call Next Patient
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .doctor-dashboard { padding: 20px; }
    .queue-info { margin-top: 20px; }
    .stats-row { display: flex; gap: 20px; margin-bottom: 20px; }
    .stat-card { flex: 1; text-align: center; }
    .stat-value { font-size: 2.5em; font-weight: bold; color: #3f51b5; }
    .stat-label { font-size: 0.9em; color: #666; }
    .current-patient-card { background: #e8f5e9; margin-bottom: 20px; }
    .current-patient { display: flex; align-items: center; gap: 10px; font-size: 1.2em; }
    .waiting-list-card { margin-bottom: 20px; }
    mat-list-item.current { background: #fff3e0; }
    mat-list-item.vip { border-left: 3px solid gold; }
    .no-patients { text-align: center; color: #999; padding: 20px; }
    .call-next-btn { width: 100%; padding: 15px; font-size: 1.1em; }
  `]
})
export class DoctorDashboardComponent implements OnInit, OnDestroy {
  doctors: Doctor[] = [];
  selectedDoctorId: number | null = null;
  queueStatus: QueueStatus | null = null;
  pollingInterval: any;

  constructor(
    private api: ApiService,
    private signalR: SignalRService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.api.getDoctors().subscribe(data => this.doctors = data);
    
    this.signalR.queueUpdated$.subscribe(data => {
      if (data && this.selectedDoctorId && data.doctorId === this.selectedDoctorId) {
        this.loadDoctorQueue();
      }
    });
  }

  ngOnDestroy() {
    if (this.pollingInterval) clearInterval(this.pollingInterval);
    if (this.selectedDoctorId) this.signalR.leaveDoctorQueue(this.selectedDoctorId);
  }

  loadDoctors() {
    this.api.getDoctors().subscribe(data => this.doctors = data);
  }

  loadDoctorQueue() {
    if (!this.selectedDoctorId) return;
    
    this.signalR.joinDoctorQueue(this.selectedDoctorId);
    this.api.getDoctorQueue(this.selectedDoctorId).subscribe(data => this.queueStatus = data);
    
    if (this.pollingInterval) clearInterval(this.pollingInterval);
    this.pollingInterval = setInterval(() => {
      if (this.selectedDoctorId) {
        this.api.getDoctorQueue(this.selectedDoctorId).subscribe(data => this.queueStatus = data);
      }
    }, 5000);
  }

  callNextPatient() {
    if (!this.selectedDoctorId) return;
    
    this.api.callNextPatient(this.selectedDoctorId).subscribe({
      next: (ticket) => {
        this.snackBar.open(`Called patient #${ticket.ticketNumber}`, 'OK', { duration: 3000 });
        this.loadDoctorQueue();
      },
      error: () => this.snackBar.open('No patients in queue', 'OK', { duration: 3000 })
    });
  }

  markPatientCalled(ticketId: number) {
    this.api.completeTicket(ticketId).subscribe({
      next: () => this.loadDoctorQueue(),
      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
    });
  }

  completePatient(ticketId: number) {
    this.api.completeTicket(ticketId).subscribe({
      next: () => {
        this.snackBar.open('Patient completed', 'OK', { duration: 3000 });
        this.loadDoctorQueue();
      },
      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
    });
  }
}