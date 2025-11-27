/**
 * Development environment configuration
 * This file is used during local development
 * 
 * IMPORTANT: Never commit real credentials to version control
 * Use environment.local.ts (git-ignored) for local development with real credentials
 */
export const environment = {
  production: false,
  googleClientId: 'YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com',
  apiUrl: '/weatherforecast'
};
