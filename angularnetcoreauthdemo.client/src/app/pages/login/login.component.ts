import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router } from '@angular/router';
import { LoginModel } from '../../shared/models/login-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: ``
})

export class LoginComponent {

  fb: FormBuilder = inject(FormBuilder);
  authService: AuthService = inject(AuthService);
  router: Router = inject(Router);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  login() {
    const loginModel: LoginModel = {
      email: this.loginForm.controls['email'].value,
      password: this.loginForm.controls['password'].value
    }
    this.authService.login(loginModel).subscribe({
      next: (response) => {
        if (response.result) {
          console.log('Login succeeded');
          this.router.navigate(['/']);
        }
        this.router.navigate(['/'])
      },
      error: (error) => {
        console.log('Login failed');
        console.log(error);
      }
    });
  }

}
