import { environment } from '../environments/environment';

/**
 * Configuration for Google Sign-In
 * 
 * This configuration now uses environment variables for better security
 * 
 * For local development with real credentials:
 * 1. Copy environment.local.template.ts to environment.local.ts
 * 2. Add your real Google Client ID
 * 3. Import from environment.local instead
 * 
 * Steps to get your Google Client ID:
 * 1. Go to https://console.cloud.google.com/
 * 2. Create a new project (or select existing one)
 * 3. Go to "APIs & Services" > "Credentials"
 * 4. Click "Create Credentials" > "OAuth client ID"
 * 5. Application type: Web application
 * 6. Name: BingoAI Web Client
 * 7. Authorized JavaScript origins: https://localhost:59641
 * 8. Authorized redirect URIs: https://localhost:59641
 * 9. Click "Create"
 * 10. Copy the Client ID and paste it in environment.local.ts
 */

export const googleConfig = {
  clientId: environment.googleClientId,
};

/**
 * Configuration for Facebook Sign-In
 * 
 * Steps to get your Facebook App ID:
 * 1. Go to https://developers.facebook.com/
 * 2. Create a new app (Consumer type)
 * 3. Add "Facebook Login" product
 * 4. Go to Settings > Basic to get your App ID
 * 5. In Facebook Login > Settings:
 *    - Valid OAuth Redirect URIs: https://localhost:59641/
 * 6. Copy the App ID and paste it in environment.local.ts
 */
export const facebookConfig = {
  appId: environment.facebookAppId,
  version: 'v18.0'
};

/**
 * API configuration
 */
export const apiConfig = {
  uri: environment.apiUrl
};
