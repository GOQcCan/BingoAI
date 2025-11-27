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
