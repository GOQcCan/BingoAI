/**
 * Configuration for Google Sign-In
 * 
 * IMPORTANT: Replace with your actual Google Client ID
 * Get it from: https://console.cloud.google.com/
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
 * 10. Copy the Client ID and paste it below
 */

export const googleConfig = {
  clientId: 'YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com', // TODO: Replace with your Google Client ID
};

/**
 * API configuration
 */
export const apiConfig = {
  uri: '/weatherforecast'
};
