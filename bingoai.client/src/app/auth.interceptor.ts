import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Authentication Interceptor (Angular 19+ Functional Style)
 * 
 * Automatically adds the Google ID token to all HTTP requests
 * and handles authentication errors (401 Unauthorized)
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  
  // Get the authentication token
  const token = authService.getIdToken();
  
  // Clone the request and add the authorization header if token exists
  // Only add to requests that start with '/' (local API calls)
  if (token && req.url.startsWith('/')) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  // Handle the request and catch authentication errors
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // If we get a 401 Unauthorized, log the user out
      if (error.status === 401) {
        console.warn('Authentication failed, logging out user');
        authService.logout();
      }
      
      // Re-throw the error so components can handle it
      return throwError(() => error);
    })
  );
};
