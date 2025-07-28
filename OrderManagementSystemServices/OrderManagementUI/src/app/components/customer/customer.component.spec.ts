import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { CustomerComponent } from './customer.component';
import { CustomerService, Customer, CreateCustomerDto } from '../../services/customer.service';

describe('CustomerComponent', () => {
  let component: CustomerComponent;
  let fixture: ComponentFixture<CustomerComponent>;
  let customerService: jasmine.SpyObj<CustomerService>;

  const mockCustomers: Customer[] = [
    { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@example.com' },
    { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com' }
  ];

  beforeEach(async () => {
    const customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getCustomers', 'createCustomer']);
    customerServiceSpy.getCustomers.and.returnValue(of(mockCustomers));

    await TestBed.configureTestingModule({
      declarations: [CustomerComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: CustomerService, useValue: customerServiceSpy }
      ]
    }).compileComponents();

    customerService = TestBed.inject(CustomerService) as jasmine.SpyObj<CustomerService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load customers on init', () => {
    expect(customerService.getCustomers).toHaveBeenCalled();
    expect(component.customers).toEqual(mockCustomers);
  });

  it('should initialize with empty form', () => {
    expect(component.customerForm.get('firstName')?.value).toBe('');
    expect(component.customerForm.get('lastName')?.value).toBe('');
    expect(component.customerForm.get('email')?.value).toBe('');
    expect(component.customerForm.valid).toBeFalse();
  });

  it('should validate required fields', () => {
    const form = component.customerForm;
    expect(form.valid).toBeFalse();

    form.controls['firstName'].setValue('John');
    expect(form.controls['firstName'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['lastName'].setValue('Doe');
    expect(form.controls['lastName'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['email'].setValue('invalid-email');
    expect(form.controls['email'].valid).toBeFalse();

    form.controls['email'].setValue('john@example.com');
    expect(form.controls['email'].valid).toBeTrue();
    expect(form.valid).toBeTrue();
  });

  it('should create customer when form is valid', fakeAsync(() => {
    const newCustomer: CreateCustomerDto = {
      firstName: 'Alice',
      lastName: 'Johnson',
      email: 'alice@example.com'
    };

    const mockResponse: Customer = {
      id: 3,
      ...newCustomer
    };

    customerService.createCustomer.and.returnValue(of(mockResponse));

    component.customerForm.setValue(newCustomer);
    component.onSubmit();
    tick();

    expect(customerService.createCustomer).toHaveBeenCalledWith(newCustomer);
    expect(customerService.getCustomers).toHaveBeenCalledTimes(2); // Initial load + after creation
    expect(component.customerForm.value).toEqual({
      firstName: null,
      lastName: null,
      email: null
    });
  }));

  it('should not create customer when form is invalid', fakeAsync(() => {
    component.customerForm.controls['firstName'].setValue('');
    component.onSubmit();
    tick();

    expect(customerService.createCustomer).not.toHaveBeenCalled();
  }));

  it('should handle error when loading customers', fakeAsync(() => {
    customerService.getCustomers.and.returnValue(throwError(() => new Error('Network error')));
    spyOn(console, 'error');

    component.loadCustomers();
    tick();

    expect(console.error).toHaveBeenCalledWith('Error loading customers:', jasmine.any(Error));
  }));

  it('should handle error when creating customer', fakeAsync(() => {
    const newCustomer: CreateCustomerDto = {
      firstName: 'Alice',
      lastName: 'Johnson',
      email: 'alice@example.com'
    };

    customerService.createCustomer.and.returnValue(throwError(() => new Error('Network error')));
    spyOn(console, 'error');

    component.customerForm.setValue(newCustomer);
    component.onSubmit();
    tick();

    expect(console.error).toHaveBeenCalledWith('Error creating customer:', jasmine.any(Error));
  }));

  it('should reset form', () => {
    component.customerForm.setValue({
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com'
    });

    component.resetForm();

    expect(component.currentCustomerId).toBeNull();
    expect(component.customerForm.value).toEqual({
      firstName: null,
      lastName: null,
      email: null
    });
  });
}); 