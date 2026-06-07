//import { Component, OnInit } from '@angular/core';
//import { CommonModule } from '@angular/common';
//import { FormsModule } from '@angular/forms';
//import { MatCardModule } from '@angular/material/card';
//import { MatButtonModule } from '@angular/material/button';
//import { MatTableModule } from '@angular/material/table';
//import { MatFormFieldModule } from '@angular/material/form-field';
//import { MatInputModule } from '@angular/material/input';
//import { MatSelectModule } from '@angular/material/select';
//import { MatTabsModule } from '@angular/material/tabs';
//import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
//import { ApiService } from '../../services/api.service';
//import { Clinic, Doctor, CreateClinicDto, CreateDoctorDto, CreatePatientDto, CreateAppointmentDto } from '../../models/clinic.models';

//@Component({
//  selector: 'app-admin-dashboard',
//  standalone: true,
//  imports: [CommonModule, FormsModule, MatCardModule, MatButtonModule, MatTableModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatTabsModule, MatSnackBarModule],
//  template: `
//    <div class="admin-dashboard">
//      <mat-card>
//        <mat-card-header>
//          <mat-card-title>Admin Dashboard</mat-card-title>
//        </mat-card-header>
//        <mat-card-content>
//          <mat-tab-group>
//            <mat-tab label="Clinics">
//              <div class="tab-content">
//                <h3>Add New Clinic</h3>
//                <mat-form-field appearance="outline">
//                  <mat-label>Name</mat-label>
//                  <input matInput [(ngModel)]="newClinic.name">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Address</mat-label>
//                  <input matInput [(ngModel)]="newClinic.address">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Phone</mat-label>
//                  <input matInput [(ngModel)]="newClinic.phone">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Email</mat-label>
//                  <input matInput [(ngModel)]="newClinic.email">
//                </mat-form-field>
//                <button mat-raised-button color="primary" (click)="addClinic()">Add Clinic</button>

//                <h3>All Clinics</h3>
//                <table mat-table [dataSource]="clinics" class="mat-elevation-z8">
//                  <ng-container matColumnDef="name">
//                    <th mat-header-cell *matHeaderCellDef>Name</th>
//                    <td mat-cell *matCellDef="let c">{{ c.name }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="address">
//                    <th mat-header-cell *matHeaderCellDef>Address</th>
//                    <td mat-cell *matCellDef="let c">{{ c.address }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="phone">
//                    <th mat-header-cell *matHeaderCellDef>Phone</th>
//                    <td mat-cell *matCellDef="let c">{{ c.phone }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="doctors">
//                    <th mat-header-cell *matHeaderCellDef>Doctors</th>
//                    <td mat-cell *matCellDef="let c">{{ c.doctorCount }}</td>
//                  </ng-container>
//                  <tr mat-header-row *matHeaderRowDef="['name', 'address', 'phone', 'doctors']"></tr>
//                  <tr mat-row *matRowDef="let row; columns: ['name', 'address', 'phone', 'doctors'];"></tr>
//                </table>
//              </div>
//            </mat-tab>

//            <mat-tab label="Doctors">
//              <div class="tab-content">
//                <h3>Add New Doctor</h3>
//                <mat-form-field appearance="outline">
//                  <mat-label>First Name</mat-label>
//                  <input matInput [(ngModel)]="newDoctor.firstName">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Last Name</mat-label>
//                  <input matInput [(ngModel)]="newDoctor.lastName">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Specialty</mat-label>
//                  <input matInput [(ngModel)]="newDoctor.specialty">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>License Number</mat-label>
//                  <input matInput [(ngModel)]="newDoctor.licenseNumber">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Clinic</mat-label>
//                  <mat-select [(ngModel)]="newDoctor.clinicId">
//                    <mat-option *ngFor="let c of clinics" [value]="c.id">{{ c.name }}</mat-option>
//                  </mat-select>
//                </mat-form-field>
//                <button mat-raised-button color="primary" (click)="addDoctor()">Add Doctor</button>

//                <h3>All Doctors</h3>
//                <table mat-table [dataSource]="doctors" class="mat-elevation-z8">
//                  <ng-container matColumnDef="name">
//                    <th mat-header-cell *matHeaderCellDef>Name</th>
//                    <td mat-cell *matCellDef="let d">Dr. {{ d.fullName }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="specialty">
//                    <th mat-header-cell *matHeaderCellDef>Specialty</th>
//                    <td mat-cell *matCellDef="let d">{{ d.specialty }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="clinic">
//                    <th mat-header-cell *matHeaderCellDef>Clinic</th>
//                    <td mat-cell *matCellDef="let d">{{ d.clinicName }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="queue">
//                    <th mat-header-cell *matHeaderCellDef>Queue</th>
//                    <td mat-cell *matCellDef="let d">{{ d.queueCount }}</td>
//                  </ng-container>
//                  <tr mat-header-row *matHeaderRowDef="['name', 'specialty', 'clinic', 'queue']"></tr>
//                  <tr mat-row *matRowDef="let row; columns: ['name', 'specialty', 'clinic', 'queue'];"></tr>
//                </table>
//              </div>
//            </mat-tab>

//            <mat-tab label="Patients">
//              <div class="tab-content">
//                <h3>Register Patient</h3>
//                <mat-form-field appearance="outline">
//                  <mat-label>First Name</mat-label>
//                  <input matInput [(ngModel)]="newPatient.firstName">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Last Name</mat-label>
//                  <input matInput [(ngModel)]="newPatient.lastName">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Phone</mat-label>
//                  <input matInput [(ngModel)]="newPatient.phone">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Email</mat-label>
//                  <input matInput [(ngModel)]="newPatient.email">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Date of Birth</mat-label>
//                  <input matInput [(ngModel)]="newPatient.dateOfBirth" type="date">
//                </mat-form-field>
//                <button mat-raised-button color="primary" (click)="addPatient()">Register Patient</button>

//                <h3>All Patients</h3>
//                <table mat-table [dataSource]="patients" class="mat-elevation-z8">
//                  <ng-container matColumnDef="name">
//                    <th mat-header-cell *matHeaderCellDef>Name</th>
//                    <td mat-cell *matCellDef="let p">{{ p.fullName }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="phone">
//                    <th mat-header-cell *matHeaderCellDef>Phone</th>
//                    <td mat-cell *matCellDef="let p">{{ p.phone }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="email">
//                    <th mat-header-cell *matHeaderCellDef>Email</th>
//                    <td mat-cell *matCellDef="let p">{{ p.email }}</td>
//                  </ng-container>
//                  <ng-container matColumnDef="vip">
//                    <th mat-header-cell *matHeaderCellDef>VIP</th>
//                    <td mat-cell *matCellDef="let p">{{ p.isVip ? 'Yes' : 'No' }}</td>
//                  </ng-container>
//                  <tr mat-header-row *matHeaderRowDef="['name', 'phone', 'email', 'vip']"></tr>
//                  <tr mat-row *matRowDef="let row; columns: ['name', 'phone', 'email', 'vip'];"></tr>
//                </table>
//              </div>
//            </mat-tab>

//            <mat-tab label="Appointments">
//              <div class="tab-content">
//                <h3>Create Appointment</h3>
//                <mat-form-field appearance="outline">
//                  <mat-label>Patient</mat-label>
//                  <mat-select [(ngModel)]="newAppointment.patientId">
//                    <mat-option *ngFor="let p of patients" [value]="p.id">{{ p.fullName }}</mat-option>
//                  </mat-select>
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Doctor</mat-label>
//                  <mat-select [(ngModel)]="newAppointment.doctorId">
//                    <mat-option *ngFor="let d of doctors" [value]="d.id">Dr. {{ d.fullName }}</mat-option>
//                  </mat-select>
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Clinic</mat-label>
//                  <mat-select [(ngModel)]="newAppointment.clinicId">
//                    <mat-option *ngFor="let c of clinics" [value]="c.id">{{ c.name }}</mat-option>
//                  </mat-select>
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Date & Time</mat-label>
//                  <input matInput [(ngModel)]="newAppointment.scheduledDateTime" type="datetime-local">
//                </mat-form-field>
//                <mat-form-field appearance="outline">
//                  <mat-label>Reason</mat-label>
//                  <input matInput [(ngModel)]="newAppointment.reason">
//                </mat-form-field>
//                <button mat-raised-button color="primary" (click)="createAppointment()">Create Appointment</button>
//              </div>
//            </mat-tab>
//          </mat-tab-group>
//        </mat-card-content>
//      </mat-card>
//    </div>
//  `,
//  styles: [`
//    .admin-dashboard { padding: 20px; }
//    .tab-content { padding: 20px; }
//    mat-form-field { margin-right: 10px; width: 200px; }
//    h3 { margin-top: 20px; margin-bottom: 10px; }
//    table { width: 100%; margin-top: 20px; }
//  `]
//})
//export class AdminDashboardComponent implements OnInit {
//  clinics: Clinic[] = [];
//  doctors: Doctor[] = [];
//  patients: any[] = [];

//  newClinic: CreateClinicDto = { name: '', address: '', phone: '', email: '', estimatedWaitTimePerPatient: 15, workingHoursStart: '09:00', workingHoursEnd: '18:00' };
//  newDoctor: CreateDoctorDto = { firstName: '', lastName: '', specialty: '', licenseNumber: '', phone: '', email: '', estimatedConsultationMinutes: 15, clinicId: 0 };
//  newPatient: CreatePatientDto = { firstName: '', lastName: '', phone: '', email: '', dateOfBirth: '', isVip: false };
//  newAppointment: CreateAppointmentDto = { patientId: 0, doctorId: 0, clinicId: 0, scheduledDateTime: '' };

//  constructor(private api: ApiService, private snackBar: MatSnackBar) {}

//  ngOnInit() {
//    this.loadData();
//  }

//  loadData() {
//    this.api.getClinics().subscribe(data => this.clinics = data);
//    this.api.getDoctors().subscribe(data => this.doctors = data);
//    this.api.getPatients().subscribe(data => this.patients = data);
//  }

//  addClinic() {
//    this.api.createClinic(this.newClinic).subscribe({
//      next: () => { this.snackBar.open('Clinic added', 'OK', { duration: 3000 }); this.loadData(); },
//      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
//    });
//  }

//  addDoctor() {
//    this.api.createDoctor(this.newDoctor).subscribe({
//      next: () => { this.snackBar.open('Doctor added', 'OK', { duration: 3000 }); this.loadData(); },
//      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
//    });
//  }

//  addPatient() {
//    this.api.createPatient(this.newPatient).subscribe({
//      next: () => { this.snackBar.open('Patient registered', 'OK', { duration: 3000 }); this.loadData(); },
//      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
//    });
//  }

//  createAppointment() {
//    this.api.createAppointment(this.newAppointment).subscribe({
//      next: () => { this.snackBar.open('Appointment created', 'OK', { duration: 3000 }); this.loadData(); },
//      error: () => this.snackBar.open('Failed', 'OK', { duration: 3000 })
//    });
//  }
//}


import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { ApiService } from '../../services/api.service';
import { Clinic, Doctor, CreateClinicDto, CreateDoctorDto, CreatePatientDto, CreateAppointmentDto } from '../../models/clinic.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTabsModule,
    MatSnackBarModule
  ],
  template: `
    <div class="admin-dashboard">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Admin Dashboard</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <mat-tab-group>
            <mat-tab label="Clinics">
              <div class="tab-content">
                <h3>Add New Clinic</h3>
                <mat-form-field appearance="outline">
                  <mat-label>Name</mat-label>
                  <input matInput [(ngModel)]="newClinic.name">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Address</mat-label>
                  <input matInput [(ngModel)]="newClinic.address">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Phone</mat-label>
                  <input matInput [(ngModel)]="newClinic.phone">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Email</mat-label>
                  <input matInput [(ngModel)]="newClinic.email">
                </mat-form-field>
                <button mat-raised-button color="primary" (click)="addClinic()">Add Clinic</button>

                <h3>All Clinics</h3>
                <table mat-table [dataSource]="clinics" class="mat-elevation-z8">
                  <ng-container matColumnDef="name">
                    <th mat-header-cell *matHeaderCellDef>Name</th>
                    <td mat-cell *matCellDef="let c">{{ c.name }}</td>
                  </ng-container>
                  <ng-container matColumnDef="address">
                    <th mat-header-cell *matHeaderCellDef>Address</th>
                    <td mat-cell *matCellDef="let c">{{ c.address }}</td>
                  </ng-container>
                  <ng-container matColumnDef="phone">
                    <th mat-header-cell *matHeaderCellDef>Phone</th>
                    <td mat-cell *matCellDef="let c">{{ c.phone }}</td>
                  </ng-container>
                  <ng-container matColumnDef="doctors">
                    <th mat-header-cell *matHeaderCellDef>Doctors</th>
                    <td mat-cell *matCellDef="let c">{{ c.doctorCount || 0 }}</td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="['name', 'address', 'phone', 'doctors']"></tr>
                  <tr mat-row *matRowDef="let row; columns: ['name', 'address', 'phone', 'doctors'];"></tr>
                </table>
              </div>
            </mat-tab>

            <mat-tab label="Doctors">
              <div class="tab-content">
                <h3>Add New Doctor</h3>
                <!-- باقي الـ Doctors tab كما هو -->
                <mat-form-field appearance="outline">
                  <mat-label>First Name</mat-label>
                  <input matInput [(ngModel)]="newDoctor.firstName">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Last Name</mat-label>
                  <input matInput [(ngModel)]="newDoctor.lastName">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Specialty</mat-label>
                  <input matInput [(ngModel)]="newDoctor.specialty">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>License Number</mat-label>
                  <input matInput [(ngModel)]="newDoctor.licenseNumber">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Clinic</mat-label>
                  <mat-select [(ngModel)]="newDoctor.clinicId">
                    <mat-option *ngFor="let c of clinics" [value]="c.id">{{ c.name }}</mat-option>
                  </mat-select>
                </mat-form-field>
                <button mat-raised-button color="primary" (click)="addDoctor()">Add Doctor</button>

                <h3>All Doctors</h3>
                <table mat-table [dataSource]="doctors" class="mat-elevation-z8">
                  <ng-container matColumnDef="name">
                    <th mat-header-cell *matHeaderCellDef>Name</th>
                    <td mat-cell *matCellDef="let d">Dr. {{ d.fullName }}</td>
                  </ng-container>
                  <ng-container matColumnDef="specialty">
                    <th mat-header-cell *matHeaderCellDef>Specialty</th>
                    <td mat-cell *matCellDef="let d">{{ d.specialty }}</td>
                  </ng-container>
                  <ng-container matColumnDef="clinic">
                    <th mat-header-cell *matHeaderCellDef>Clinic</th>
                    <td mat-cell *matCellDef="let d">{{ d.clinicName }}</td>
                  </ng-container>
                  <ng-container matColumnDef="queue">
                    <th mat-header-cell *matHeaderCellDef>Queue</th>
                    <td mat-cell *matCellDef="let d">{{ d.queueCount || 0 }}</td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="['name', 'specialty', 'clinic', 'queue']"></tr>
                  <tr mat-row *matRowDef="let row; columns: ['name', 'specialty', 'clinic', 'queue'];"></tr>
                </table>
              </div>
            </mat-tab>

            <!-- باقي الـ Tabs (Patients & Appointments) تبقى كما هي -->
            <mat-tab label="Patients">
              <div class="tab-content">
                <h3>Register Patient</h3>
                <!-- ... محتوى Patients tab كما هو ... -->
              </div>
            </mat-tab>

            <mat-tab label="Appointments">
              <div class="tab-content">
                <h3>Create Appointment</h3>
                <!-- ... محتوى Appointments tab كما هو ... -->
              </div>
            </mat-tab>
          </mat-tab-group>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .admin-dashboard { padding: 20px; }
    .tab-content { padding: 20px; }
    mat-form-field { margin-right: 10px; width: 200px; }
    h3 { margin-top: 20px; margin-bottom: 10px; }
    table { width: 100%; margin-top: 20px; }
  `]
})
export class AdminDashboardComponent implements OnInit {

  clinics: Clinic[] = [];
  doctors: Doctor[] = [];
  patients: any[] = [];

  newClinic: CreateClinicDto = {
    name: '',
    address: '',
    phone: '',
    email: '',
    estimatedWaitTimePerPatient: 15,
    workingHoursStart: '09:00',
    workingHoursEnd: '18:00'
  };

  newDoctor: CreateDoctorDto = {
    firstName: '',
    lastName: '',
    specialty: '',
    licenseNumber: '',
    phone: '',
    email: '',
    estimatedConsultationMinutes: 15,
    clinicId: 0
  };

  newPatient: CreatePatientDto = {
    firstName: '',
    lastName: '',
    phone: '',
    email: '',
    dateOfBirth: '',
    isVip: false
  };

  newAppointment: CreateAppointmentDto = {
    patientId: 0,
    doctorId: 0,
    clinicId: 0,
    scheduledDateTime: ''
  };

  constructor(
    private api: ApiService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef   // ← أضفنا ده
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.api.getClinics().subscribe({
      next: (data) => {
        this.clinics = data || [];
        this.cdr.markForCheck();        // ← الحل الرئيسي
      }
    });

    this.api.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data || [];
        this.cdr.markForCheck();
      }
    });

    this.api.getPatients().subscribe({
      next: (data) => {
        this.patients = data || [];
        this.cdr.markForCheck();
      }
    });
  }

  addClinic() {
    this.api.createClinic(this.newClinic).subscribe({
      next: () => {
        this.snackBar.open('Clinic added successfully', 'OK', { duration: 3000 });
        this.loadData();
      },
      error: () => this.snackBar.open('Failed to add clinic', 'OK', { duration: 3000 })
    });
  }

  addDoctor() {
    this.api.createDoctor(this.newDoctor).subscribe({
      next: () => {
        this.snackBar.open('Doctor added successfully', 'OK', { duration: 3000 });
        this.loadData();
      },
      error: () => this.snackBar.open('Failed to add doctor', 'OK', { duration: 3000 })
    });
  }

  addPatient() {
    this.api.createPatient(this.newPatient).subscribe({
      next: () => {
        this.snackBar.open('Patient registered successfully', 'OK', { duration: 3000 });
        this.loadData();
      },
      error: () => this.snackBar.open('Failed to register patient', 'OK', { duration: 3000 })
    });
  }

  createAppointment() {
    this.api.createAppointment(this.newAppointment).subscribe({
      next: () => {
        this.snackBar.open('Appointment created successfully', 'OK', { duration: 3000 });
        this.loadData();
      },
      error: () => this.snackBar.open('Failed to create appointment', 'OK', { duration: 3000 })
    });
  }
}
