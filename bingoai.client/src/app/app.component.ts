import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from './auth.service';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {
  // ✅ Utilisation de Signals pour la réactivité
  public forecasts = signal<WeatherForecast[]>([]);
  public isLoading = signal(false);
  public error = signal<string | null>(null);
  
  // ✅ Signal computed pour vérifier si on a des données
  public hasForecasts = computed(() => this.forecasts().length > 0);
  
  private readonly _destroying$ = new Subject<void>();

  constructor(
    private readonly http: HttpClient,
    public authService: AuthService
  ) {}

  ngOnInit() {
    // Listen to auth state changes
    this.authService.user$
      .pipe(takeUntil(this._destroying$))
      .subscribe(user => {
        if (user) {
          console.log('User logged in:', user);
          this.getForecasts();
        } else {
          console.log('User logged out');
          this.forecasts.set([]);
        }
      });
  }

  ngOnDestroy(): void {
    this._destroying$.next();
    this._destroying$.complete();
  }

  logout() {
    this.authService.logout();
  }

  getForecasts() {
    this.isLoading.set(true);
    this.error.set(null);

    // Get the ID token from Google
    const idToken = this.authService.getIdToken();

    // Create headers with the token
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${idToken}`
    });

    this.http.get<WeatherForecast[]>('/weatherforecast', { headers })
      .pipe(takeUntil(this._destroying$))
      .subscribe({
        next: (result) => {
          this.forecasts.set(result);
          this.isLoading.set(false);
        },
        error: (error) => {
          console.error('Error fetching forecasts:', error);
          this.error.set('Failed to load weather forecasts. Please try again.');
          this.isLoading.set(false);
        }
      });
  }

  title = 'bingoai.client';
}

