/**
 * Production environment configuration
 * 
 * IMPORTANT: In production, these values should be replaced during build
 * using environment variables or CI/CD pipeline configuration
 * 
 * Example using Azure DevOps or GitHub Actions:
 * - Set environment variable: GOOGLE_CLIENT_ID
 * - Replace placeholder during build/deployment
 */
export const environment = {
  production: true,
  googleClientId: '${GOOGLE_CLIENT_ID}', // Will be replaced during build
  apiUrl: '/weatherforecast'
};
