import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService, Product, CreateProductDto } from '../../services/product.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
  standalone: false
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  productForm: FormGroup;
  currentProductId: number | null = null;

  constructor(
    private productService: ProductService,
    private fb: FormBuilder
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required]],
      price: ['', [Validators.required, Validators.min(0)]],
      availableStock: ['', [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (error) => console.error('Error loading products:', error)
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const productData: CreateProductDto = this.productForm.value;

      this.productService.createProduct(productData).subscribe({
        next: () => {
          this.loadProducts();
          this.resetForm();
        },
        error: (error) => console.error('Error creating product:', error)
      });
    }
  }

  resetForm(): void {
    this.currentProductId = null;
    this.productForm.reset();
  }
} 
