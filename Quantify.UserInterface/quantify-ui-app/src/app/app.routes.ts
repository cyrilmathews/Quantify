import { Routes } from '@angular/router';
import { Landing } from './dashboard/landing/landing';
// Removed JobList import for lazy loading
import { JobForm } from './jobs/job-form/job-form';
import { ClientForm } from './clients/client-form/client-form';

export const routes: Routes = [
    { path: '', component: Landing },
    {
        path: 'jobs/list',
        loadComponent: () => import('./jobs/job-list/job-list').then(m => m.JobList)
    },
    { path: 'jobs/:id', component: JobForm },
    {
        path: 'clients/list',
        loadComponent: () => import('./clients/client-list/client-list').then(m => m.ClientList)
    },
    { path: 'clients/new', component: ClientForm },
    { path: 'clients/:id', component: ClientForm },
    // ... more routes
    { path: '**', redirectTo: '' }
];
