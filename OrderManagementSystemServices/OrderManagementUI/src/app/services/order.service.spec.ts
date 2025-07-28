import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { OrderService, Order, CreateOrderDto, UpdateOrderDto } from './order.service';

describe('OrderService', () => {
  let service: OrderService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [OrderService]
    });
    service = TestBed.inject(OrderService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all orders', () => {
    const mockOrders: Order[] = [
      {
        id: 1,
        customerId: 1,
        productId: 1,
        quantity: 2,
        totalAmount: 100,
        orderDate: new Date('2024-01-01')
      },
      {
        id: 2,
        customerId: 2,
        productId: 3,
        quantity: 1,
        totalAmount: 50,
        orderDate: new Date('2024-01-02')
      }
    ];

    service.getOrders().subscribe(orders => {
      expect(orders).toEqual(mockOrders);
      expect(orders.length).toBe(2);
    });

    const req = httpMock.expectOne('http://localhost:8085/api/orders');
    expect(req.request.method).toBe('GET');
    req.flush(mockOrders);
  });

  it('should create a new order', () => {
    const newOrder: CreateOrderDto = {
      customerId: 1,
      productId: 2,
      quantity: 3
    };

    const mockResponse: Order = {
      id: 3,
      customerId: newOrder.customerId,
      productId: newOrder.productId,
      quantity: newOrder.quantity,
      totalAmount: 150,
      orderDate: new Date('2024-01-03')
    };

    service.createOrder(newOrder).subscribe(order => {
      expect(order).toEqual(mockResponse);
      expect(order.id).toBe(3);
    });

    const req = httpMock.expectOne('http://localhost:8085/api/orders');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newOrder);
    req.flush(mockResponse);
  });

  it('should update an order status', () => {
    const orderId = 1;
    const updateOrder: UpdateOrderDto = {
      status: 'Completed'
    };

    const mockResponse: Order = {
      id: orderId,
      customerId: 1,
      productId: 2,
      quantity: 3,
      totalAmount: 150,
      orderDate: new Date('2024-01-03')
    };

    service.updateOrder(orderId, updateOrder).subscribe(order => {
      expect(order).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`http://localhost:8085/api/orders/${orderId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateOrder);
    req.flush(mockResponse);
  });
}); 