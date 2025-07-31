import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';
import { Client } from '../models/client.interface';
import { ClientService } from '../client-service';

@Component({
  selector: 'app-client-list',
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './client-list.html',
  styleUrl: './client-list.scss'
})
export class ClientList implements OnInit {
  clients$: Observable<Client[]>;
  displayedColumns: string[] = ['code', 'name', 'createdOn', 'actions'];
  isLoading = true;

  constructor(
    private clientService: ClientService,
    private router: Router
  ) {
    this.clients$ = new Observable<Client[]>();
  }

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.isLoading = true;
    this.clients$ = this.clientService.getClients();
    this.clients$.subscribe({
      next: () => this.isLoading = false,
      error: () => this.isLoading = false
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  onEditClient(client: Client): void {
    this.router.navigate(['/clients', client.id]);
  }

  onDeleteClient(client: Client): void {
    // TODO: Implement delete functionality with confirmation dialog
    console.log('Delete client:', client);
  }

  onViewClient(client: Client): void {
    this.router.navigate(['/clients', client.id]);
  }
}
