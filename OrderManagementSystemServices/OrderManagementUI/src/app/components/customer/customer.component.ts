import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService, Customer, CreateCustomerDto } from '../../services/customer.service';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css'],
  standalone: false
})
export class CustomerComponent implements OnInit {
  customers: Customer[] = [];
  customerForm: FormGroup;
  currentCustomerId: number | null = null;

  constructor(
    private customerService: CustomerService,
    private fb: FormBuilder
  ) {
    this.customerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.customerService.getCustomers().subscribe({
      next: (data) => this.customers = data,
      error: (error) => console.error('Error loading customers:', error)
    });
  }

  onSubmit(): void {
    if (this.customerForm.valid) {
      const customerData: CreateCustomerDto = this.customerForm.value;

      this.customerService.createCustomer(customerData).subscribe({
        next: () => {
          this.loadCustomers();
          this.resetForm();
        },
        error: (error) => console.error('Error creating customer:', error)
      });
    }
  }

  resetForm(): void {
    this.currentCustomerId = null;
    this.customerForm.reset();
  }
} 
