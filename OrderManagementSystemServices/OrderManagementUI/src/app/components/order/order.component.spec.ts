import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { OrderComponent } from './order.component';
import { OrderService, Order, CreateOrderDto } from '../../services/order.service';
import { CustomerService, Customer } from '../../services/customer.service';
import { ProductService, Product } from '../../services/product.service';

describe('OrderComponent', () => {
  let component: OrderComponent;
  let fixture: ComponentFixture<OrderComponent>;
  let orderService: jasmine.SpyObj<OrderService>;
  let customerService: jasmine.SpyObj<CustomerService>;
  let productService: jasmine.SpyObj<ProductService>;

  const mockCustomers: Customer[] = [
    { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@example.com' },
    { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com' }
  ];

  const mockProducts: Product[] = [
    { id: 1, name: 'Product 1', price: 100, availableStock: 50 },
    { id: 2, name: 'Product 2', price: 200, availableStock: 30 }
  ];

  const mockOrders: Order[] = [
    { id: 1, customerId: 1, productId: 1, quantity: 2, totalAmount: 200, orderDate: new Date('2024-01-01') },
    { id: 2, customerId: 2, productId: 2, quantity: 1, totalAmount: 200, orderDate: new Date('2024-01-02') }
  ];

  beforeEach(async () => {
    const orderServiceSpy = jasmine.createSpyObj('OrderService', ['getOrders', 'createOrder', 'deleteOrder']);
    const customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getCustomers']);
    const productServiceSpy = jasmine.createSpyObj('ProductService', ['getProducts']);

    orderServiceSpy.getOrders.and.returnValue(of(mockOrders));
    customerServiceSpy.getCustomers.and.returnValue(of(mockCustomers));
    productServiceSpy.getProducts.and.returnValue(of(mockProducts));

    await TestBed.configureTestingModule({
      declarations: [OrderComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: OrderService, useValue: orderServiceSpy },
        { provide: CustomerService, useValue: customerServiceSpy },
        { provide: ProductService, useValue: productServiceSpy }
      ]
    }).compileComponents();

    orderService = TestBed.inject(OrderService) as jasmine.SpyObj<OrderService>;
    customerService = TestBed.inject(CustomerService) as jasmine.SpyObj<CustomerService>;
    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load orders, customers and products on init', () => {
    expect(orderService.getOrders).toHaveBeenCalled();
    expect(customerService.getCustomers).toHaveBeenCalled();
    expect(productService.getProducts).toHaveBeenCalled();
    expect(component.orders).toEqual(mockOrders);
    expect(component.customers).toEqual(mockCustomers);
    expect(component.products).toEqual(mockProducts);
  });

  it('should initialize with empty form', () => {
    expect(component.orderForm.get('customerId')?.value).toBe('');
    expect(component.orderForm.get('productId')?.value).toBe('');
    expect(component.orderForm.get('quantity')?.value).toBe('');
    expect(component.orderForm.valid).toBeFalse();
  });

  it('should validate required fields', () => {
    const form = component.orderForm;
    expect(form.valid).toBeFalse();

    form.controls['customerId'].setValue('1');
    expect(form.controls['customerId'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['productId'].setValue('1');
    expect(form.controls['productId'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['quantity'].setValue('0');
    expect(form.controls['quantity'].valid).toBeFalse();

    form.controls['quantity'].setValue('1');
    expect(form.controls['quantity'].valid).toBeTrue();
    expect(form.valid).toBeTrue();
  });

  it('should create order when form is valid', fakeAsync(() => {
    const formValue = {
      customerId: '1',
      productId: '1',
      quantity: '2'
    };

    const expectedDto: CreateOrderDto = {
      customerId: parseInt(formValue.customerId),
      productId: parseInt(formValue.productId),
      quantity: parseInt(formValue.quantity)
    };

    const mockResponse: Order = {
      id: 3,
      customerId: expectedDto.customerId,
      productId: expectedDto.productId,
      quantity: expectedDto.quantity,
      totalAmount: 200,
      orderDate: new Date('2024-01-03')
    };

    orderService.createOrder.and.returnValue(of(mockResponse));

    component.orderForm.setValue(formValue);
    component.onSubmit();
    tick();

    expect(orderService.createOrder).toHaveBeenCalledWith(expectedDto);
    expect(orderService.getOrders).toHaveBeenCalledTimes(2); // Initial load + after creation
    expect(component.orderForm.value).toEqual({
      customerId: null,
      productId: null,
      quantity: null
    });
  }));

  it('should handle error when creating order - insufficient stock', fakeAsync(() => {
    const error = { status: 400, error: 'Stock is not available for the requested product.' };
    orderService.createOrder.and.returnValue(throwError(() => error));
    spyOn(window, 'alert');

    component.orderForm.setValue({
      customerId: '1',
      productId: '1',
      quantity: '100'
    });
    component.onSubmit();
    tick();

    expect(window.alert).toHaveBeenCalledWith('Stock not available for the selected product.');
  }));

  it('should delete order after confirmation', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    orderService.deleteOrder.and.returnValue(of(void 0));

    component.deleteOrder(1);
    tick();

    expect(orderService.deleteOrder).toHaveBeenCalledWith(1);
    expect(orderService.getOrders).toHaveBeenCalledTimes(2); // Initial load + after deletion
  }));

  it('should not delete order if not confirmed', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(false);

    component.deleteOrder(1);
    tick();

    expect(orderService.deleteOrder).not.toHaveBeenCalled();
  }));

  it('should get customer name', () => {
    expect(component.getCustomerName(1)).toBe('John Doe');
    expect(component.getCustomerName(999)).toBe('Unknown Customer');
  });

  it('should get product name', () => {
    expect(component.getProductName(1)).toBe('Product 1');
    expect(component.getProductName(999)).toBe('Unknown Product');
  });
}); 