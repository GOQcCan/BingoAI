import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

// Social Login
import { 
  SocialLoginModule, 
  SocialAuthServiceConfig,
  GoogleLoginProvider,
  FacebookLoginProvider,
  GoogleSigninButtonModule 
} from '@abacritt/angularx-social-login';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { googleConfig, facebookConfig } from './auth-config';

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
        lang: 'fr',
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(googleConfig.clientId, {
              oneTapEnabled: false,
              prompt: 'consent',
              scopes: 'openid profile email'
            })
          },
          {
            id: FacebookLoginProvider.PROVIDER_ID,
            provider: new FacebookLoginProvider(facebookConfig.appId, {
              scope: 'public_profile',
              return_scopes: true,
              enable_profile_selector: true,
              version: facebookConfig.version
            })
          }
        ],
        onError: (err) => {
          console.error('Social auth error:', err);
        }
      } as SocialAuthServiceConfig,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

