import { Routes } from '@angular/router';
import { PatientDashboardComponent } from './components/patient/patient-dashboard.component';
import { DoctorDashboardComponent } from './components/doctor/doctor-dashboard.component';
import { AdminDashboardComponent } from './components/admin/admin-dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: '/patient', pathMatch: 'full' },
  { path: 'patient', component: PatientDashboardComponent },
  { path: 'doctor', component: DoctorDashboardComponent },
  { path: 'admin', component: AdminDashboardComponent }
];