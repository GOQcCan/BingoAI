import { Injectable } from '@angular/core';
import { SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public user$: Observable<SocialUser | null>;

  constructor(private socialAuthService: SocialAuthService) {
    this.user$ = this.socialAuthService.authState;
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
    // The authState is a BehaviorSubject, we can get its current value
    let user: SocialUser | null = null;
    this.user$.subscribe(u => user = u).unsubscribe();
    return user;
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
   * Get ID token for API calls
   */
  getIdToken(): string | null {
    const user = this.getCurrentUser();
    return user ? user.idToken : null;
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
