# 🎯 BingoAI - Weather Forecast Application

A full-stack application built with **Angular 19** and **.NET 8** featuring **Google** and **Facebook** OAuth authentication.

## 🏗️ Architecture

```
BingoAI/
├── BingoAI.Server/          # .NET 8 Web API
│   ├── Controllers/         # API endpoints
│   ├── Program.cs          # Application configuration
│   └── appsettings.json    # Configuration (no secrets)
│
├── bingoai.client/         # Angular 19 SPA
│   ├── src/
│   │   ├── app/           # Application components
│   │   └── environments/  # Environment configurations
│   └── package.json
│
└── ENVIRONMENT_SETUP.md   # Detailed setup guide
```

## ✨ Features

- ✅ **Google OAuth Authentication** via Google Sign-In
- ✅ **Facebook OAuth Authentication** via Facebook Login
- ✅ **JWT Token Validation** on backend (Google + Facebook Graph API)
- ✅ **Secure API Endpoints** with Bearer token authentication
- ✅ **Dark Mode Support** with automatic system detection
- ✅ **Responsive UI** with modern design
- ✅ **HTTPS Development** with ASP.NET Core certificates
- ✅ **Proxy Configuration** for seamless frontend-backend communication

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- [Google Cloud Console](https://console.cloud.google.com/) account
- [Facebook Developers](https://developers.facebook.com/) account

### 1. Clone the Repository

```bash
git clone https://github.com/GOQcCan/BingoAI.git
cd BingoAI
```

### 2. Configure Environment Variables

**⚠️ IMPORTANT**: Follow the detailed guide in [ENVIRONMENT_SETUP.md](./ENVIRONMENT_SETUP.md)

**Quick Setup:**

```bash
# Frontend - Create local environment
cd bingoai.client/src/environments
cp environment.local.template.ts environment.local.ts
# Edit environment.local.ts with your Google Client ID and Facebook App ID

# Backend - Use .NET User Secrets
cd ../../../BingoAI.Server
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Facebook:AppId" "YOUR-FACEBOOK-APP-ID"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "YOUR-FACEBOOK-APP-SECRET"
```

### 3. Install Dependencies

```bash
# Angular dependencies
cd bingoai.client
npm install

# .NET dependencies (automatic on build)
cd ../BingoAI.Server
dotnet restore
```

### 4. Run the Application

#### Option A: Visual Studio (Recommended)

1. Open `BingoAI.sln` in Visual Studio 2022
2. Set **Multiple Startup Projects**:
   - `BingoAI.Server` → Start
   - `bingoai.client` → Start
3. Press **F5** to run

#### Option B: Command Line

**Terminal 1 - Backend:**
```bash
cd BingoAI.Server
dotnet run
# Backend runs at https://localhost:7077
```

**Terminal 2 - Frontend:**
```bash
cd bingoai.client
npm start
# Frontend runs at https://localhost:59641
```

### 5. Access the Application

Open your browser and navigate to:
```
https://localhost:59641
```

## 🔐 Security Configuration

### Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create OAuth 2.0 Client ID
3. Configure authorized origins:
   - Development: `https://localhost:59641`
   - Production: Your production URL
4. Copy the Client ID

### Facebook OAuth Setup

1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Create a new app (Consumer type)
3. Add "Facebook Login" product
4. Go to Settings > Basic to get your App ID and App Secret
5. In Facebook Login > Settings:
   - Valid OAuth Redirect URIs: `https://localhost:59641/`
6. Copy the App ID and App Secret

### Frontend Configuration

```typescript
// bingoai.client/src/environments/environment.local.ts
export const environment = {
  production: false,
  googleClientId: 'YOUR-GOOGLE-CLIENT-ID.apps.googleusercontent.com',
  facebookAppId: 'YOUR-FACEBOOK-APP-ID',
  apiUrl: '/weatherforecast'
};
```

### Backend Configuration

```bash
# Using .NET User Secrets (recommended)
cd BingoAI.Server

# Google OAuth
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR-GOOGLE-CLIENT-ID"

# Facebook OAuth
dotnet user-secrets set "Authentication:Facebook:AppId" "YOUR-FACEBOOK-APP-ID"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "YOUR-FACEBOOK-APP-SECRET"
```

## 🛠️ Technology Stack

### Frontend
- **Framework**: Angular 19.1.0
- **Language**: TypeScript 5.7.2
- **Authentication**: @abacritt/angularx-social-login 2.2.0 (Google + Facebook)
- **HTTP Client**: Angular HttpClient
- **Testing**: Jasmine + Karma

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Authentication**: 
  - Google: JWT Token Validation via Google OIDC
  - Facebook: Access Token Validation via Graph API
- **API Documentation**: Swagger/OpenAPI

## 📝 Environment Variables

### Required Variables

| Variable | Location | Description | Example |
|----------|----------|-------------|---------|
| `googleClientId` | Frontend | Google OAuth Client ID | `xxxxx.apps.googleusercontent.com` |
| `facebookAppId` | Frontend | Facebook App ID | `1234567890` |
| `Authentication:Google:ClientId` | Backend | Google OAuth Client ID | `xxxxx.apps.googleusercontent.com` |
| `Authentication:Facebook:AppId` | Backend | Facebook App ID | `1234567890` |
| `Authentication:Facebook:AppSecret` | Backend | Facebook App Secret | `abc123def456...` |

## 🔗 Resources

- [Angular Documentation](https://angular.io/docs)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Google OAuth Guide](https://developers.google.com/identity/protocols/oauth2)
- [Facebook Login Guide](https://developers.facebook.com/docs/facebook-login/)
- [Environment Setup Guide](./ENVIRONMENT_SETUP.md)

---

**⚠️ Security Note**: Never commit real credentials to version control. Always use environment variables or user secrets for sensitive data.