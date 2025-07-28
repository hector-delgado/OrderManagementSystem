import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app';
import { AuthService } from './services/auth.service';
import { Component } from '@angular/core';
import { LoginComponent } from './components/login/login.component';
import { OrderComponent } from './components/order/order.component';
import { CustomerComponent } from './components/customer/customer.component';
import { ProductComponent } from './components/product/product.component';
import { AuthGuard } from './guards/auth.guard';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;
  let authGuard: jasmine.SpyObj<AuthGuard>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'logout']);
    const authGuardSpy = jasmine.createSpyObj('AuthGuard', ['canActivate']);
    authGuardSpy.canActivate.and.returnValue(true);

    await TestBed.configureTestingModule({
      declarations: [
        AppComponent,
        LoginComponent,
        OrderComponent,
        CustomerComponent,
        ProductComponent
      ],
      imports: [
        RouterTestingModule.withRoutes([
          { path: '', redirectTo: '/orders', pathMatch: 'full' },
          { path: 'login', component: LoginComponent },
          { path: 'customers', component: CustomerComponent },
          { path: 'products', component: ProductComponent },
          { path: 'orders', component: OrderComponent }
        ])
      ],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: AuthGuard, useValue: authGuardSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    authGuard = TestBed.inject(AuthGuard) as jasmine.SpyObj<AuthGuard>;
    router = TestBed.inject(Router);
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have correct title', () => {
    expect(component.title).toBe('Order Management System');
  });

  describe('isLoggedIn', () => {
    it('should return true when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      expect(component.isLoggedIn()).toBeTrue();
      expect(authService.isAuthenticated).toHaveBeenCalled();
    });

    it('should return false when not authenticated', () => {
      authService.isAuthenticated.and.returnValue(false);
      expect(component.isLoggedIn()).toBeFalse();
      expect(authService.isAuthenticated).toHaveBeenCalled();
    });
  });

  describe('logout', () => {
    it('should call logout and navigate to login page', async () => {
      const navigateSpy = spyOn(router, 'navigate');
      component.logout();
      expect(authService.logout).toHaveBeenCalled();
      expect(navigateSpy).toHaveBeenCalledWith(['/login']);
    });
  });

  describe('template', () => {
    it('should render navigation links', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.querySelector('.navbar-brand')?.textContent).toContain('Order Management System');
      expect(compiled.querySelector('a[routerLink="/orders"]')).toBeTruthy();
      expect(compiled.querySelector('a[routerLink="/products"]')).toBeTruthy();
      expect(compiled.querySelector('a[routerLink="/customers"]')).toBeTruthy();
    });
  });
});
