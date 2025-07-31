import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { MsalGuard, MsalInterceptor, MsalService, MSAL_INSTANCE, MSAL_GUARD_CONFIG, MSAL_INTERCEPTOR_CONFIG, MsalBroadcastService } from '@azure/msal-angular';
import { MSALGuardConfigFactory, MSALInterceptorConfigFactory, MSALInstanceFactory } from './auth.config';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideAnimations(),
    provideHttpClient(withInterceptorsFromDi()),
      // MSAL Configuration for Standalone App
      { provide: MSAL_INSTANCE, useFactory: MSALInstanceFactory },
      { provide: MSAL_GUARD_CONFIG, useFactory: MSALGuardConfigFactory },
      { provide: MSAL_INTERCEPTOR_CONFIG, useFactory: MSALInterceptorConfigFactory },
      { provide: HTTP_INTERCEPTORS, useClass: MsalInterceptor, multi: true },
    MsalService,
    MsalGuard,
    MsalBroadcastService
  ]
};
