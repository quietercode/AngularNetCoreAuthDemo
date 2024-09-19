import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { LoginModel } from '../models/login-model';
import { Observable, map } from 'rxjs';
import { AuthResponseModel } from '../models/auth-response-model';
import { RegisterModel } from '../models/register-model';
import { jwtDecode } from 'jwt-decode'

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  http: HttpClient = inject(HttpClient);

  tokenKey: string = "AngularNetCoreAuthDemo_Token";
  apiUrl: string = environment.apiUrl;

  constructor() { }

  login(vm: LoginModel): Observable<AuthResponseModel> {
    return this.http.post<AuthResponseModel>(`${this.apiUrl}account/login`, vm)
      .pipe(
        map((res) => {
          if (res.result) {
            localStorage.setItem(this.tokenKey, res.token);
          }

          return res;
        })
      )
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  register(vm: RegisterModel): Observable<AuthResponseModel> {
    return this.http.post<AuthResponseModel>(`${this.apiUrl}account/register`, vm);
  }

  isLoggedIn(): boolean {
    const token = this.retrieveToken();
    if (token == null) {
      return false;
    }

    return this.isTokenValid(token);
  }

  private retrieveToken(): string | null {
    return localStorage.getItem(this.tokenKey) || null;
  }

  private isTokenValid(token: string): boolean {
    if (token == null) return false;

    const decodedToken = jwtDecode(token);

    if (Date.now() > decodedToken['exp']! * 1000) {
      this.logout();
      return false;
    }

    return true;
  }
}
