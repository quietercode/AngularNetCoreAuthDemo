import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router } from '@angular/router';
import { RegisterModel } from '../../shared/models/register-model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent {

  fb: FormBuilder = inject(FormBuilder);
  authService: AuthService = inject(AuthService);
  router: Router = inject(Router);

  registerForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required, Validators.minLength(8)]],
    fullName: ['', [Validators.required, Validators.minLength(3)]]
  });

  register() {
    const registerModel: RegisterModel = {
      email: this.registerForm.controls['email'].value,
      password: this.registerForm.controls['password'].value,
      confirmPassword: this.registerForm.controls['confirmPassword'].value,
      fullName: this.registerForm.controls['fullName'].value
    }
    this.authService.register(registerModel).subscribe({
      next: (response) => {
        if (response.result) {
          console.log('Registration succeeded');
          this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.log('Registration failed');
        console.log(error);
      }
    });
  }

}
