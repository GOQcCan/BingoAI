import { TestBed } from '@angular/core/testing';
import { SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let socialAuthServiceSpy: jasmine.SpyObj<SocialAuthService>;
  let authStateSubject: BehaviorSubject<SocialUser | null>;

  beforeEach(() => {
    // Create a BehaviorSubject to simulate authState
    authStateSubject = new BehaviorSubject<SocialUser | null>(null);

    // Create spy for SocialAuthService
    const spy = jasmine.createSpyObj('SocialAuthService', ['signOut'], {
      authState: authStateSubject.asObservable()
    });

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: SocialAuthService, useValue: spy }
      ]
    });

    service = TestBed.inject(AuthService);
    socialAuthServiceSpy = TestBed.inject(SocialAuthService) as jasmine.SpyObj<SocialAuthService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should check if user is logged in', (done) => {
    const mockUser: SocialUser = {
      id: '123',
      email: 'test@example.com',
      name: 'Test User',
      photoUrl: 'https://example.com/photo.jpg',
      firstName: 'Test',
      lastName: 'User',
      authToken: 'mock-auth-token',
      idToken: 'mock-id-token',
      authorizationCode: 'mock-code',
      response: {},
      provider: 'GOOGLE'
    };

    // Emit a user to simulate logged-in state
    authStateSubject.next(mockUser);

    // Wait for async subscription
    setTimeout(() => {
      expect(service.isLoggedIn()).toBe(true);
      done();
    }, 10);
  });

  it('should return false when no user is logged in', (done) => {
    // Emit null to simulate logged-out state
    authStateSubject.next(null);

    // Wait for async subscription
    setTimeout(() => {
      expect(service.isLoggedIn()).toBe(false);
      done();
    }, 10);
  });

  it('should get user name when logged in', (done) => {
    const mockUser: SocialUser = {
      id: '123',
      email: 'test@example.com',
      name: 'John Doe',
      photoUrl: 'https://example.com/photo.jpg',
      firstName: 'John',
      lastName: 'Doe',
      authToken: 'mock-auth-token',
      idToken: 'mock-id-token',
      authorizationCode: 'mock-code',
      response: {},
      provider: 'GOOGLE'
    };

    authStateSubject.next(mockUser);

    setTimeout(() => {
      expect(service.getUserName()).toBe('John Doe');
      done();
    }, 10);
  });

  it('should return empty string when no user name available', () => {
    authStateSubject.next(null);
    expect(service.getUserName()).toBe('');
  });

  it('should get user email when logged in', (done) => {
    const mockUser: SocialUser = {
      id: '123',
      email: 'john.doe@example.com',
      name: 'John Doe',
      photoUrl: 'https://example.com/photo.jpg',
      firstName: 'John',
      lastName: 'Doe',
      authToken: 'mock-auth-token',
      idToken: 'mock-id-token',
      authorizationCode: 'mock-code',
      response: {},
      provider: 'GOOGLE'
    };

    authStateSubject.next(mockUser);

    setTimeout(() => {
      expect(service.getUserEmail()).toBe('john.doe@example.com');
      done();
    }, 10);
  });

  it('should get user photo URL when logged in', (done) => {
    const mockUser: SocialUser = {
      id: '123',
      email: 'test@example.com',
      name: 'John Doe',
      photoUrl: 'https://example.com/photo.jpg',
      firstName: 'John',
      lastName: 'Doe',
      authToken: 'mock-auth-token',
      idToken: 'mock-id-token',
      authorizationCode: 'mock-code',
      response: {},
      provider: 'GOOGLE'
    };

    authStateSubject.next(mockUser);

    setTimeout(() => {
      expect(service.getUserPhotoUrl()).toBe('https://example.com/photo.jpg');
      done();
    }, 10);
  });

  it('should get ID token when logged in', (done) => {
    const mockUser: SocialUser = {
      id: '123',
      email: 'test@example.com',
      name: 'John Doe',
      photoUrl: 'https://example.com/photo.jpg',
      firstName: 'John',
      lastName: 'Doe',
      authToken: 'mock-auth-token',
      idToken: 'mock-google-id-token-123',
      authorizationCode: 'mock-code',
      response: {},
      provider: 'GOOGLE'
    };

    authStateSubject.next(mockUser);

    setTimeout(() => {
      expect(service.getIdToken()).toBe('mock-google-id-token-123');
      done();
    }, 10);
  });

  it('should return null when no ID token available', () => {
    authStateSubject.next(null);
    expect(service.getIdToken()).toBeNull();
  });

  it('should call signOut when logout is called', async () => {
    socialAuthServiceSpy.signOut.and.returnValue(Promise.resolve());

    await service.logout();

    expect(socialAuthServiceSpy.signOut).toHaveBeenCalled();
  });

  it('should handle logout errors gracefully', async () => {
    const consoleErrorSpy = spyOn(console, 'error');
    socialAuthServiceSpy.signOut.and.returnValue(Promise.reject('Logout failed'));

    await service.logout();

    expect(socialAuthServiceSpy.signOut).toHaveBeenCalled();
    expect(consoleErrorSpy).toHaveBeenCalledWith('Logout failed', 'Logout failed');
  });
});
