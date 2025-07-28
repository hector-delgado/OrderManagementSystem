import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { ProductComponent } from './product.component';
import { ProductService, Product, CreateProductDto } from '../../services/product.service';

describe('ProductComponent', () => {
  let component: ProductComponent;
  let fixture: ComponentFixture<ProductComponent>;
  let productService: jasmine.SpyObj<ProductService>;

  const mockProducts: Product[] = [
    { id: 1, name: 'Product 1', price: 100, availableStock: 50 },
    { id: 2, name: 'Product 2', price: 200, availableStock: 30 }
  ];

  beforeEach(async () => {
    const productServiceSpy = jasmine.createSpyObj('ProductService', ['getProducts', 'createProduct']);
    productServiceSpy.getProducts.and.returnValue(of(mockProducts));

    await TestBed.configureTestingModule({
      declarations: [ProductComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy }
      ]
    }).compileComponents();

    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products on init', () => {
    expect(productService.getProducts).toHaveBeenCalled();
    expect(component.products).toEqual(mockProducts);
  });

  it('should initialize with empty form', () => {
    expect(component.productForm.get('name')?.value).toBe('');
    expect(component.productForm.get('price')?.value).toBe('');
    expect(component.productForm.get('availableStock')?.value).toBe('');
    expect(component.productForm.valid).toBeFalse();
  });

  it('should validate required fields', () => {
    const form = component.productForm;
    expect(form.valid).toBeFalse();

    form.controls['name'].setValue('Test Product');
    expect(form.controls['name'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['price'].setValue(-1);
    expect(form.controls['price'].valid).toBeFalse();

    form.controls['price'].setValue(100);
    expect(form.controls['price'].valid).toBeTrue();
    expect(form.valid).toBeFalse();

    form.controls['availableStock'].setValue(-1);
    expect(form.controls['availableStock'].valid).toBeFalse();

    form.controls['availableStock'].setValue(50);
    expect(form.controls['availableStock'].valid).toBeTrue();
    expect(form.valid).toBeTrue();
  });

  it('should create product when form is valid', fakeAsync(() => {
    const formValue = {
      name: 'New Product',
      price: 150,
      availableStock: 75
    };

    const mockResponse: Product = {
      id: 3,
      name: formValue.name,
      price: formValue.price,
      availableStock: formValue.availableStock
    };

    productService.createProduct.and.returnValue(of(mockResponse));

    component.productForm.setValue(formValue);
    component.onSubmit();
    tick();

    expect(productService.createProduct).toHaveBeenCalledWith({
      name: formValue.name,
      price: formValue.price,
      availableStock: formValue.availableStock
    });
    expect(productService.getProducts).toHaveBeenCalledTimes(2); // Initial load + after creation
    expect(component.productForm.value).toEqual({
      name: null,
      price: null,
      availableStock: null
    });
  }));

  it('should not create product when form is invalid', fakeAsync(() => {
    component.productForm.controls['name'].setValue('');
    component.onSubmit();
    tick();

    expect(productService.createProduct).not.toHaveBeenCalled();
  }));

  it('should handle error when loading products', fakeAsync(() => {
    productService.getProducts.and.returnValue(throwError(() => new Error('Network error')));
    spyOn(console, 'error');

    component.loadProducts();
    tick();

    expect(console.error).toHaveBeenCalledWith('Error loading products:', jasmine.any(Error));
  }));

  it('should handle error when creating product', fakeAsync(() => {
    const formValue = {
      name: 'New Product',
      price: 150,
      availableStock: 75
    };

    productService.createProduct.and.returnValue(throwError(() => new Error('Network error')));
    spyOn(console, 'error');

    component.productForm.setValue(formValue);
    component.onSubmit();
    tick();

    expect(console.error).toHaveBeenCalledWith('Error creating product:', jasmine.any(Error));
  }));

  it('should reset form', () => {
    component.productForm.setValue({
      name: 'Test Product',
      price: 100,
      availableStock: 50
    });

    component.resetForm();

    expect(component.currentProductId).toBeNull();
    expect(component.productForm.value).toEqual({
      name: null,
      price: null,
      availableStock: null
    });
  });
}); 