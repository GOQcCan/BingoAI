import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

// Google Social Login
import { 
  SocialLoginModule, 
  SocialAuthServiceConfig,
  GoogleLoginProvider,
  GoogleSigninButtonModule 
} from '@abacritt/angularx-social-login';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { googleConfig } from './auth-config';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SocialLoginModule,
    GoogleSigninButtonModule
  ],
  providers: [
    // ✅ Angular 19+ : Nouvelle API HttpClient
    provideHttpClient(
      withInterceptorsFromDi() // Support pour les anciens interceptors si nécessaire
    ),
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        lang: 'en',
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(googleConfig.clientId, {
              oneTapEnabled: false, // Disable One Tap
              scopes: 'openid profile email'
            })
          }
        ],
        onError: (err) => {
          console.error('Google auth error:', err);
        }
      } as SocialAuthServiceConfig,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

