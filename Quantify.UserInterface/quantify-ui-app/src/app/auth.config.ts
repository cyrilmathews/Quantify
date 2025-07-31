import { LogLevel, BrowserCacheLocation, PublicClientApplication, IPublicClientApplication, Configuration, InteractionType } from '@azure/msal-browser';
import { MsalInterceptorConfiguration, MsalGuardConfiguration } from '@azure/msal-angular';
import { environment } from '../environments/environment';

const isIE = (typeof window !== 'undefined') 
  && (window.navigator.userAgent.indexOf("MSIE ") > -1 || window.navigator.userAgent.indexOf("Trident/") > -1);

export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.msal.clientId, // The Application (client) ID
      authority: `https://login.microsoftonline.com/${environment.msal.tenantId}`, // The Directory (tenant) ID
      redirectUri: environment.msal.redirectUri // Your local redirect URI
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
      storeAuthStateInCookie: isIE
    },
    system: {
      loggerOptions: {
        loggerCallback: (level, message, containsPii) => {
          if (containsPii) {
            return;
          }
          switch (level) {
            case LogLevel.Error:
              console.error(message);
              return;
            case LogLevel.Info:
              console.info(message);
              return;
            case LogLevel.Verbose:
              console.debug(message);
              return;
            case LogLevel.Warning:
              console.warn(message);
              return;
          }
        }
      }
    }
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  // Map your backend API endpoints to the scopes you require
  protectedResourceMap.set(environment.apiBaseUrl, [environment.msal.jobsApiScope]);

  return {
    protectedResourceMap,
    interactionType: InteractionType.Redirect
  };
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: [environment.msal.jobsApiScope] // Scopes your app needs at login
    }
  };
}