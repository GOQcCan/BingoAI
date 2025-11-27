# 🎯 BingoAI - Weather Forecast Application

A full-stack application built with **Angular 19** and **.NET 8** featuring Google OAuth authentication.

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
- ✅ **JWT Token Validation** on backend
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
- Google Cloud Console account

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
# Edit environment.local.ts with your Google Client ID

# Backend - Use .NET User Secrets
cd ../../../BingoAI.Server
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR-CLIENT-ID.apps.googleusercontent.com"
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

### Frontend Configuration

```typescript
// bingoai.client/src/environments/environment.local.ts
export const environment = {
  production: false,
  googleClientId: 'YOUR-CLIENT-ID.apps.googleusercontent.com',
  apiUrl: '/weatherforecast'
};
```

### Backend Configuration

```bash
# Using .NET User Secrets (recommended)
cd BingoAI.Server
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR-CLIENT-ID"
```

## 🛠️ Technology Stack

### Frontend
- **Framework**: Angular 19.1.0
- **Language**: TypeScript 5.7.2
- **Authentication**: @abacritt/angularx-social-login 2.2.0
- **HTTP Client**: Angular HttpClient
- **Testing**: Jasmine + Karma

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Authentication**: Google.Apis.Auth 1.68.0
- **API Documentation**: Swagger/OpenAPI

## 📝 Environment Variables

### Required Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `GOOGLE_CLIENT_ID` | Google OAuth Client ID | `xxxxx.apps.googleusercontent.com` |
| `Authentication__Google__ClientId` | Backend Google Client ID | Same as above |

## 🔗 Resources

- [Angular Documentation](https://angular.io/docs)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Google OAuth Guide](https://developers.google.com/identity/protocols/oauth2)
- [Environment Setup Guide](./ENVIRONMENT_SETUP.md)

---

**⚠️ Security Note**: Never commit real credentials to version control. Always use environment variables or user secrets for sensitive data.