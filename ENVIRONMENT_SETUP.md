# Environment Configuration Guide

This guide explains how to configure environment variables for the BingoAI application.

## 🔐 Security Best Practices

**NEVER** commit real credentials to version control. This project uses environment files that are git-ignored to protect sensitive data.

## 📁 Environment Files Structure

```
bingoai.client/src/environments/
├── environment.ts                      # Development (template, committed)
├── environment.prod.ts                 # Production (template, committed)
├── environment.local.template.ts       # Local template (committed)
└── environment.local.ts               # Your local config (GIT-IGNORED)
```

### Required Credentials

| Provider | Frontend | Backend |
|----------|----------|--------|
| Google | Client ID | Client ID |
| Facebook | App ID | App ID + App Secret |

## 🚀 Setup for Local Development

### Step 1: Get Your Google Client ID

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Navigate to **APIs & Services** > **Credentials**
4. Click **Create Credentials** > **OAuth client ID**
5. Configure the OAuth consent screen if prompted
6. Select **Web application** as the application type
7. Set the following:
   - **Name**: BingoAI Web Client
   - **Authorized JavaScript origins**: `https://localhost:59641`
   - **Authorized redirect URIs**: `https://localhost:59641`
8. Click **Create**
9. Copy the **Client ID** (format: `xxxxx.apps.googleusercontent.com`)

### Step 2: Get Your Facebook App ID and Secret

1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Click **My Apps** > **Create App**
3. Select **Consumer** as the app type
4. Fill in your app details and click **Create App**
5. In the app dashboard, click **Add Product** and select **Facebook Login**
6. Choose **Web** as the platform
7. Enter your site URL: `https://localhost:59641`
8. Go to **Facebook Login** > **Settings** and add:
   - **Valid OAuth Redirect URIs**: `https://localhost:59641/`
9. Go to **Settings** > **Basic** to find:
   - **App ID**: Copy this value
   - **App Secret**: Click "Show" and copy this value
10. Make sure your app is in **Development** mode for testing

### Step 3: Create Local Environment File

```bash
# Navigate to environments folder
cd bingoai.client/src/environments

# Copy the template
cp environment.local.template.ts environment.local.ts
```

### Step 4: Configure Your Credentials

Edit `environment.local.ts`:

```typescript
export const environment = {
  production: false,
  googleClientId: 'YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com', // ← Google Client ID
  facebookAppId: 'YOUR-FACEBOOK-APP-ID',                               // ← Facebook App ID
  apiUrl: '/weatherforecast'
};
```

### Step 5: Update Import (Optional for Local Dev)

If you want to use your local credentials, update `auth-config.ts`:

```typescript
// Use this for local development with real credentials
import { environment } from '../environments/environment.local';

// OR keep the default (uses environment.ts)
import { environment } from '../environments/environment';
```

## 🏗️ Backend Configuration

### .NET Server Configuration

The backend needs credentials for both Google and Facebook token validation.

#### Option 1: User Secrets (Recommended for Development)

```bash
cd BingoAI.Server

# Initialize user secrets (if not already done)
dotnet user-secrets init

# Set Google credentials
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com"

# Set Facebook credentials
dotnet user-secrets set "Authentication:Facebook:AppId" "YOUR-FACEBOOK-APP-ID"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "YOUR-FACEBOOK-APP-SECRET"
```

#### Option 2: Environment Variables

```bash
# Windows PowerShell
$env:Authentication__Google__ClientId = "YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com"
$env:Authentication__Facebook__AppId = "YOUR-FACEBOOK-APP-ID"
$env:Authentication__Facebook__AppSecret = "YOUR-FACEBOOK-APP-SECRET"

# Linux/macOS
export Authentication__Google__ClientId="YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com"
export Authentication__Facebook__AppId="YOUR-FACEBOOK-APP-ID"
export Authentication__Facebook__AppSecret="YOUR-FACEBOOK-APP-SECRET"
```

#### Option 3: appsettings.Development.json (Git-Ignored)

Create `BingoAI.Server/appsettings.Development.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com"
    },
    "Facebook": {
      "AppId": "YOUR-FACEBOOK-APP-ID",
      "AppSecret": "YOUR-FACEBOOK-APP-SECRET"
    }
  }
}
```

## 🚢 Production Deployment

### Angular Production Build

The production build uses `environment.prod.ts`, which contains placeholders:

```typescript
export const environment = {
  production: true,
  googleClientId: '${GOOGLE_CLIENT_ID}',   // Replaced during deployment
  facebookAppId: '${FACEBOOK_APP_ID}',     // Replaced during deployment
  apiUrl: '/weatherforecast'
};
```

### Deployment Options

#### Option 1: CI/CD Pipeline (Recommended)

**Azure DevOps / GitHub Actions:**

```yaml
# Example: GitHub Actions
- name: Replace environment variables
  run: |
    sed -i 's/${GOOGLE_CLIENT_ID}/${{ secrets.GOOGLE_CLIENT_ID }}/g' \
      bingoai.client/dist/main.*.js
    sed -i 's/${FACEBOOK_APP_ID}/${{ secrets.FACEBOOK_APP_ID }}/g' \
      bingoai.client/dist/main.*.js
```

#### Option 2: Build-Time Replacement

```bash
# Build the app
ng build --configuration production

# Replace placeholders in compiled JavaScript
sed -i 's/${GOOGLE_CLIENT_ID}/your-google-client-id/g' dist/bingoai.client/browser/main.*.js
sed -i 's/${FACEBOOK_APP_ID}/your-facebook-app-id/g' dist/bingoai.client/browser/main.*.js
```

### Backend Production Configuration

Use one of these methods:

1. **Azure App Settings** (for Azure App Service)
2. **Environment Variables** in your hosting environment
3. **Docker Secrets** (for containerized deployments)
4. **Azure Key Vault** (most secure)

## ✅ Verification

### Check Frontend

```bash
cd bingoai.client
npm start
```

Open browser console and check for errors:
- If you see `"YOUR-GOOGLE-CLIENT-ID"`, configure your Google credentials
- If you see `"YOUR-FACEBOOK-APP-ID"`, configure your Facebook credentials

### Check Backend

```bash
cd BingoAI.Server
dotnet run
```

Check logs for:
- `"Google ClientId is not configured"` - Missing Google credentials
- Facebook authentication errors - Missing Facebook App ID or Secret

## 🔍 Troubleshooting

### Issue: "Invalid Google token"

- **Cause**: Client ID mismatch between frontend and backend
- **Solution**: Ensure both use the same Google Client ID

### Issue: "Facebook token validation failed"

- **Cause**: Invalid or missing Facebook App Secret on backend
- **Solution**: Verify `Authentication:Facebook:AppSecret` is correctly configured

### Issue: "Facebook Login popup closes immediately"

- **Cause**: Facebook App ID mismatch or app not properly configured
- **Solution**: 
  1. Verify App ID matches between frontend and Facebook Developer Console
  2. Ensure app is in Development mode or your Facebook account is added as a tester
  3. Check that `https://localhost:59641/` is in Valid OAuth Redirect URIs

### Issue: "No token provided"

- **Cause**: Frontend not sending the token
- **Solution**: Check browser console for authentication errors

### Issue: CORS errors

- **Cause**: Frontend origin not in allowed list
- **Solution**: Update `Program.cs` CORS configuration

## 📚 Additional Resources

- [Angular Environment Configuration](https://angular.io/guide/build#configuring-application-environments)
- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Google OAuth Setup](https://developers.google.com/identity/protocols/oauth2)
- [Facebook Login Setup](https://developers.facebook.com/docs/facebook-login/web)
- [Facebook Graph API - Debug Token](https://developers.facebook.com/docs/graph-api/reference/v18.0/debug_token)

## ⚠️ Important Notes

1. **Never commit** `environment.local.ts` or `appsettings.Development.json`
2. **Always use** User Secrets or environment variables for development
3. **Rotate credentials** regularly
4. **Use different** Client IDs/App IDs for development and production
5. **Enable** Google Cloud Console audit logs
6. **Facebook App Secret** must be kept secure - only on backend, never on frontend
7. **Facebook App** must be in Development mode for localhost testing, or add test users
