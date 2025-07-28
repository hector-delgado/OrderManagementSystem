import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CustomerService, Customer, CreateCustomerDto } from './customer.service';

describe('CustomerService', () => {
  let service: CustomerService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CustomerService]
    });
    service = TestBed.inject(CustomerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all customers', () => {
    const mockCustomers: Customer[] = [
      { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@example.com' },
      { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com' }
    ];

    service.getCustomers().subscribe(customers => {
      expect(customers).toEqual(mockCustomers);
      expect(customers.length).toBe(2);
    });

    const req = httpMock.expectOne('http://localhost:8084/api/customers');
    expect(req.request.method).toBe('GET');
    req.flush(mockCustomers);
  });

  it('should create a new customer', () => {
    const newCustomer: CreateCustomerDto = {
      firstName: 'Alice',
      lastName: 'Johnson',
      email: 'alice@example.com'
    };

    const mockResponse: Customer = {
      id: 3,
      ...newCustomer
    };

    service.createCustomer(newCustomer).subscribe(customer => {
      expect(customer).toEqual(mockResponse);
      expect(customer.id).toBe(3);
    });

    const req = httpMock.expectOne('http://localhost:8084/api/customers');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newCustomer);
    req.flush(mockResponse);
  });

  it('should get a customer by id', () => {
    const mockCustomer: Customer = {
      id: 1,
      firstName: 'John',
      lastName: 'Doe',
      email: 'john@example.com'
    };

    service.getCustomer(1).subscribe(customer => {
      expect(customer).toEqual(mockCustomer);
    });

    const req = httpMock.expectOne('http://localhost:8084/api/customers/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockCustomer);
  });
}); 