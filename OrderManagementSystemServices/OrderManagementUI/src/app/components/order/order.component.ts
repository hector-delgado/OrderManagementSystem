import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrderService, Order, CreateOrderDto } from '../../services/order.service';
import { CustomerService, Customer } from '../../services/customer.service';
import { ProductService, Product } from '../../services/product.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css'],
  standalone: false
})
export class OrderComponent implements OnInit {
  orders: Order[] = [];
  customers: Customer[] = [];
  products: Product[] = [];
  orderForm: FormGroup;

  constructor(
    private orderService: OrderService,
    private customerService: CustomerService,
    private productService: ProductService,
    private fb: FormBuilder
  ) {
    this.orderForm = this.fb.group({
      customerId: ['', Validators.required],
      productId: ['', Validators.required],
      quantity: ['', [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.loadOrders();
    this.loadCustomers();
    this.loadProducts();
  }

  loadOrders(): void {
    this.orderService.getOrders().subscribe({
      next: (data) => this.orders = data,
      error: (error) => console.error('Error loading orders:', error)
    });
  }

  loadCustomers(): void {
    this.customerService.getCustomers().subscribe({
      next: (data) => this.customers = data,
      error: (error) => console.error('Error loading customers:', error)
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (error) => console.error('Error loading products:', error)
    });
  }

  onSubmit(): void {
    if (this.orderForm.valid) {
      const orderData: CreateOrderDto = {
        customerId: parseInt(this.orderForm.value.customerId),
        productId: parseInt(this.orderForm.value.productId),
        quantity: parseInt(this.orderForm.value.quantity)
      };

      this.orderService.createOrder(orderData).subscribe({
        next: () => {
          this.loadOrders();
          this.resetForm();
        },
        error: (error) => {
          console.error('Error creating order:', error);
          if (error.status === 400) {
            if (error.error.includes('Stock is not available for the requested product.')) {
              alert('Stock not available for the selected product.');
            } else {
              alert('Failed to create order. Please check the form and try again.');
            }
          } else {
            alert('An error occurred while creating the order. Please try again.');
          }
        }
      });
    }
  }

  deleteOrder(id: number): void {
    if (confirm('Are you sure you want to delete this order?')) {
      this.orderService.deleteOrder(id).subscribe({
        next: () => this.loadOrders(),
        error: (error) => console.error('Error deleting order:', error)
      });
    }
  }

  resetForm(): void {
    this.orderForm.reset();
  }

  getCustomerName(customerId: number): string {
    const customer = this.customers.find(c => c.id === customerId);
    return customer ? `${customer.firstName} ${customer.lastName}` : 'Unknown Customer';
  }

  getProductName(productId: number): string {
    const product = this.products.find(p => p.id === productId);
    return product ? product.name : 'Unknown Product';
  }
} 
