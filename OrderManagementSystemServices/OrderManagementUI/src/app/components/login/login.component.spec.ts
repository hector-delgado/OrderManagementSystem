import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login', 'isAuthenticated']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty form', () => {
    expect(component.loginForm.get('username')?.value).toBe('');
    expect(component.loginForm.get('password')?.value).toBe('');
    expect(component.loginForm.valid).toBeFalse();
  });

  it('should validate required fields', () => {
    const usernameControl = component.loginForm.get('username');
    const passwordControl = component.loginForm.get('password');

    expect(usernameControl?.errors?.['required']).toBeTruthy();
    expect(passwordControl?.errors?.['required']).toBeTruthy();

    usernameControl?.setValue('testuser');
    expect(usernameControl?.errors).toBeNull();
    expect(component.loginForm.valid).toBeFalse(); // still false because password is empty

    passwordControl?.setValue('testpass');
    expect(passwordControl?.errors).toBeNull();
    expect(component.loginForm.valid).toBeTrue();
  });

  it('should redirect to home if already authenticated', () => {
    authService.isAuthenticated.and.returnValue(true);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should not redirect if not authenticated', () => {
    authService.isAuthenticated.and.returnValue(false);
    component.ngOnInit();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  describe('onSubmit', () => {
    beforeEach(() => {
      component.loginForm.setValue({
        username: 'testuser',
        password: 'testpass'
      });
    });

    it('should not call login service if form is invalid', () => {
      component.loginForm.get('username')?.setValue('');
      component.onSubmit();
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should call login service and navigate on success', fakeAsync(() => {
      const mockLoginResponse = {
        username: 'testuser',
        accessToken: 'mock-token',
        expiresIn: 3600
      };

      authService.login.and.returnValue(of(mockLoginResponse));
      
      component.onSubmit();
      tick();

      expect(authService.login).toHaveBeenCalledWith({
        username: 'testuser',
        password: 'testpass'
      });
      expect(component.isLoading).toBeFalse();
      expect(component.errorMessage).toBe('');
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    }));

    it('should handle login error', fakeAsync(() => {
      const errorResponse = {
        error: { message: 'Invalid credentials' }
      };

      authService.login.and.returnValue(throwError(() => errorResponse));
      
      component.onSubmit();
      tick();

      expect(component.isLoading).toBeFalse();
      expect(component.errorMessage).toBe('Invalid credentials');
      expect(router.navigate).not.toHaveBeenCalled();
    }));

    it('should set default error message if no error message received', fakeAsync(() => {
      authService.login.and.returnValue(throwError(() => ({ error: {} })));
      
      component.onSubmit();
      tick();

      expect(component.errorMessage).toBe('Login failed. Please try again.');
    }));
  });
}); 