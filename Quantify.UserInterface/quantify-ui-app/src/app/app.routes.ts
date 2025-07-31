import { Routes } from '@angular/router';
import { Landing } from './dashboard/landing/landing';
// Removed JobList import for lazy loading
import { JobForm } from './jobs/job-form/job-form';
import { ClientForm } from './clients/client-form/client-form';
import { MsalGuard } from '@azure/msal-angular';

export const routes: Routes = [
    { path: '', component: Landing },
    {
        path: 'jobs/list',
        canMatch: [MsalGuard], // changed from canActivate
        loadComponent: () => import('./jobs/job-list/job-list').then(m => m.JobList)
    },
    { path: 'jobs/:id', component: JobForm, canActivate: [MsalGuard] },
    {
        path: 'clients/list',
        canMatch: [MsalGuard], // changed from canActivate
        loadComponent: () => import('./clients/client-list/client-list').then(m => m.ClientList)
    },
    { path: 'clients/new', component: ClientForm, canActivate: [MsalGuard] },
    { path: 'clients/:id', component: ClientForm, canActivate: [MsalGuard] },
    // ... more routes
    { path: '**', redirectTo: '' }
];
