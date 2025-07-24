import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';
import { Job } from '../models/job.interface';
import { JobService } from '../job-service';

@Component({
  selector: 'app-job-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './job-list.html',
  styleUrl: './job-list.scss'
})
export class JobList implements OnInit {
  jobs$: Observable<Job[]>;
  displayedColumns: string[] = ['code', 'name', 'client', 'createdOn', 'actions'];
  isLoading = true;

  constructor(private jobService: JobService) {
    this.jobs$ = new Observable<Job[]>();
  }

  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(): void {
    this.isLoading = true;
    this.jobs$ = this.jobService.getJobs();
    this.jobs$.subscribe({
      next: () => this.isLoading = false,
      error: () => this.isLoading = false
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  onEditJob(job: Job): void {
    // TODO: Implement edit functionality
    console.log('Edit job:', job);
  }

  onDeleteJob(job: Job): void {
    // TODO: Implement delete functionality
    console.log('Delete job:', job);
  }

  onViewJob(job: Job): void {
    // TODO: Implement view functionality
    console.log('View job:', job);
  }
}
