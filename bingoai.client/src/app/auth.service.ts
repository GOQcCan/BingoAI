import { Injectable } from '@angular/core';
import { SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly currentUserSubject = new BehaviorSubject<SocialUser | null>(null);
  public user$: Observable<SocialUser | null> = this.currentUserSubject.asObservable();

  constructor(private readonly socialAuthService: SocialAuthService) {
    this.socialAuthService.authState.subscribe(user => 
      this.currentUserSubject.next(user)
    );
  }

  /**
   * Check if user is logged in
   */
  isLoggedIn(): boolean {
    const currentUser = this.getCurrentUser();
    return currentUser !== null;
  }

  /**
   * Get the current user
   */
  getCurrentUser(): SocialUser | null {
    return this.currentUserSubject.value;
  }

  /**
   * Get user display name
   */
  getUserName(): string {
    const user = this.getCurrentUser();
    return user ? user.name : '';
  }

  /**
   * Get user email
   */
  getUserEmail(): string {
    const user = this.getCurrentUser();
    return user ? user.email : '';
  }

  /**
   * Get user photo URL
   */
  getUserPhotoUrl(): string {
    const user = this.getCurrentUser();
    return user ? user.photoUrl : '';
  }

  /**
   * Get the authentication provider (google, facebook)
   */
  getProvider(): string {
    const user = this.getCurrentUser();
    return user ? user.provider : '';
  }

  /**
   * Get token for API calls
   * Returns idToken for Google, authToken for Facebook
   */
  getIdToken(): string | null {
    const user = this.getCurrentUser();
    if (!user) return null;
    
    // Google uses idToken, Facebook uses authToken (access token)
    return user.idToken || user.authToken || null;
  }

  /**
   * Get access token (for Facebook Graph API calls)
   */
  getAccessToken(): string | null {
    const user = this.getCurrentUser();
    return user ? user.authToken : null;
  }

  /**
   * Logout user
   */
  async logout(): Promise<void> {
    try {
      await this.socialAuthService.signOut();
    } catch (error) {
      console.error('Logout failed', error);
    }
  }
}
