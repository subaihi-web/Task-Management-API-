import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { getApiErrorMessage } from '../shared/error-message';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  mode: 'login' | 'signup' = 'login';
  loading = false;
  error = '';
  success = '';

  login = { email: '', password: '' };
  signup = { username: '', email: '', password: '', confirmPassword: '' };

  submit(): void {
    this.error = '';
    this.success = '';

    if (this.mode === 'login') {
      if (!this.login.email || !this.login.password) {
        this.error = 'Please enter your email and password.';
        return;
      }
      this.loading = true;
      this.auth.login(this.login).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/tasks']);
        },
        error: (error) => {
          this.loading = false;
          this.error = getApiErrorMessage(error, 'Invalid email or password.');
        }
      });
      return;
    }

    if (!this.signup.username || !this.signup.email || !this.signup.password) {
      this.error = 'Please complete all required fields.';
      return;
    }
    if (this.signup.password !== this.signup.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }
    if (this.signup.password.length < 6) {
      this.error = 'Password must contain at least 6 characters.';
      return;
    }

    this.loading = true;
    this.auth.signup({
      username: this.signup.username,
      email: this.signup.email,
      password: this.signup.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Account created successfully. You can now log in.';
        this.mode = 'login';
        this.login = { email: this.signup.email, password: '' };
        this.signup = { username: '', email: '', password: '', confirmPassword: '' };
      },
      error: (error) => {
        this.loading = false;
        this.error = getApiErrorMessage(error, 'Unable to create the account.');
      }
    });
  }

  switchMode(mode: 'login' | 'signup'): void {
    this.mode = mode;
    this.error = '';
    this.success = '';
  }
}
