import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProductService, Product, CreateProductDto } from './product.service';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService]
    });
    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all products', () => {
    const mockProducts: Product[] = [
      {
        id: 1,
        name: 'Product 1',
        price: 50,
        availableStock: 100
      },
      {
        id: 2,
        name: 'Product 2',
        price: 75,
        availableStock: 50
      }
    ];

    service.getProducts().subscribe(products => {
      expect(products).toEqual(mockProducts);
      expect(products.length).toBe(2);
    });

    const req = httpMock.expectOne('http://localhost:8086/api/products');
    expect(req.request.method).toBe('GET');
    req.flush(mockProducts);
  });

  it('should create a new product', () => {
    const newProduct: CreateProductDto = {
      name: 'New Product',
      price: 99.99,
      availableStock: 200
    };

    const mockResponse: Product = {
      id: 3,
      name: newProduct.name,
      price: newProduct.price,
      availableStock: newProduct.availableStock
    };

    service.createProduct(newProduct).subscribe(product => {
      expect(product).toEqual(mockResponse);
      expect(product.id).toBe(3);
    });

    const req = httpMock.expectOne('http://localhost:8086/api/products');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newProduct);
    req.flush(mockResponse);
  });

  it('should get a product by id', () => {
    const mockProduct: Product = {
      id: 1,
      name: 'Product 1',
      price: 50,
      availableStock: 100
    };

    service.getProduct(1).subscribe(product => {
      expect(product).toEqual(mockProduct);
    });

    const req = httpMock.expectOne('http://localhost:8086/api/products/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockProduct);
  });
}); 