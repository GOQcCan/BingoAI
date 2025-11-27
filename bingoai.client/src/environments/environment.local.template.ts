/**
 * Local development environment with REAL credentials
 * 
 * Copy this file to environment.local.ts and add your real Google Client ID
 * environment.local.ts is git-ignored and will not be committed
 * 
 * Steps to use:
 * 1. Copy this file: cp environment.local.template.ts environment.local.ts
 * 2. Replace the googleClientId with your actual Client ID from Google Cloud Console
 * 3. Never commit environment.local.ts to git
 */
export const environment = {
  production: false,
  googleClientId: 'YOUR-REAL-GOOGLE-CLIENT-ID.apps.googleusercontent.com', // TODO: Replace with your real Client ID
  apiUrl: '/weatherforecast'
};
