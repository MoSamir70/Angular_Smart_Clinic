import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <span>🏥 Smart Clinic Queue System</span>
      <span class="spacer"></span>
      <nav>
        <a mat-button routerLink="/patient" routerLinkActive="active">
          <mat-icon>person</mat-icon> Patient
        </a>
        <a mat-button routerLink="/doctor" routerLinkActive="active">
          <mat-icon>medical_services</mat-icon> Doctor
        </a>
        <a mat-button routerLink="/admin" routerLinkActive="active">
          <mat-icon>admin_panel_settings</mat-icon> Admin
        </a>
      </nav>
    </mat-toolbar>
    <main>
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .spacer { flex: 1; }
    nav a { margin-left: 10px; }
    a.active { background: rgba(255,255,255,0.2); }
    main { padding: 20px; }
    mat-toolbar { margin-bottom: 20px; }
  `]
})


/*export class App { }*/
export class AppComponent { }
