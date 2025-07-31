import { Component, OnInit, signal, ViewChild, } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MsalService } from '@azure/msal-angular';
import { Subject } from 'rxjs';
import { EventMessage, EventType, AuthenticationResult } from '@azure/msal-browser';
import { CommonModule } from '@angular/common';
// Removed JobList import for lazy loading

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    RouterModule,
    CommonModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {

  protected readonly title = signal('quantify-ui-app');
  @ViewChild('sidenav') sidenav!: MatSidenav;
  isLoggedIn: boolean = false;
  userName: string = '';
  private readonly _destroying$ = new Subject<void>();

  constructor(private authService: MsalService) { }

  ngOnInit(): void {
    // Read isLoggedIn from localStorage on init
    const storedLogin = localStorage.getItem('isLoggedIn');
    const storedUserName = localStorage.getItem('userName');
    this.isLoggedIn = storedLogin === 'true';
    this.userName = storedUserName || '';

    this.authService.handleRedirectObservable().subscribe();

    this.authService.instance.handleRedirectPromise().then(authResult => {
      this.isLoggedIn = !!authResult || this.authService.instance.getAllAccounts().length > 0;
      localStorage.setItem('isLoggedIn', this.isLoggedIn.toString());
      const account = authResult?.account;
      this.userName = account?.name || '';
      localStorage.setItem('userName', this.userName);
      console.log('Auth result:', authResult);
      console.log('Accounts:', this.authService.instance.getAllAccounts());
      console.log('isLoggedIn:', this.isLoggedIn);
    });

    // Use MsalBroadcastService to react to login events in a non-module app
    this.authService.instance.addEventCallback((event: EventMessage) => {
      if (event.eventType === EventType.LOGIN_SUCCESS) {
        this.isLoggedIn = true;
        localStorage.setItem('isLoggedIn', 'true');
        const account = (event.payload as AuthenticationResult)?.account;
        this.userName = account?.name || '';
        localStorage.setItem('userName', this.userName);
        console.log('Login successful');
      }
      if (event.eventType === EventType.LOGOUT_SUCCESS) {
        this.isLoggedIn = false;
        localStorage.setItem('isLoggedIn', 'false');
        console.log('Logout successful');
      }
    });
  }

  login() {
    this.authService.loginRedirect(); // Redirects to the Entra ID login page
  }

  logout() {
    this.authService.logoutRedirect(); // Logs out the user
  }
}
