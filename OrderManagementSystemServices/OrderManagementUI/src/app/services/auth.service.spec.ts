import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService, LoginRequest, LoginResponse } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    const mockCredentials: LoginRequest = {
      username: 'testuser',
      password: 'testpass'
    };

    const mockResponse: LoginResponse = {
      username: 'testuser',
      accessToken: 'mock-token',
      expiresIn: 3600
    };

    it('should send POST request and store token', () => {
      service.login(mockCredentials).subscribe(response => {
        expect(response).toEqual(mockResponse);
        expect(localStorage.getItem('jwt_token')).toBe(mockResponse.accessToken);
      });

      const req = httpMock.expectOne('http://localhost:8082/api/identity/login');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockCredentials);
      req.flush(mockResponse);
    });

    it('should handle login error', () => {
      const errorMessage = 'Invalid credentials';
      
      service.login(mockCredentials).subscribe({
        error: error => {
          expect(error.error.message).toBe(errorMessage);
          expect(localStorage.getItem('jwt_token')).toBeNull();
        }
      });

      const req = httpMock.expectOne('http://localhost:8082/api/identity/login');
      req.flush({ message: errorMessage }, { status: 401, statusText: 'Unauthorized' });
    });
  });

  describe('logout', () => {
    it('should remove token from localStorage', () => {
      localStorage.setItem('jwt_token', 'test-token');
      service.logout();
      expect(localStorage.getItem('jwt_token')).toBeNull();
    });
  });

  describe('getToken', () => {
    it('should return token from localStorage', () => {
      const token = 'test-token';
      localStorage.setItem('jwt_token', token);
      expect(service.getToken()).toBe(token);
    });

    it('should return null if no token exists', () => {
      localStorage.clear();
      expect(service.getToken()).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    it('should return true when token exists', () => {
      localStorage.setItem('jwt_token', 'test-token');
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should return false when no token exists', () => {
      localStorage.clear();
      expect(service.isAuthenticated()).toBe(false);
    });
  });
}); 