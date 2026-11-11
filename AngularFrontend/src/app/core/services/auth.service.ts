import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserDto,
} from '../models/auth.models';

const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/api/auth`;

  private readonly _user = signal<UserDto | null>(this.readUser());
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._user() !== null);

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, payload)
      .pipe(tap((response) => this.store(response)));
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/register`, payload)
      .pipe(tap((response) => this.store(response)));
  }

  logout(): void {
    const storage = this.storage;
    storage?.removeItem(TOKEN_KEY);
    storage?.removeItem(USER_KEY);
    this._user.set(null);
  }

  get token(): string | null {
    return this.storage?.getItem(TOKEN_KEY) ?? null;
  }

  private store(response: AuthResponse): void {
    const storage = this.storage;
    storage?.setItem(TOKEN_KEY, response.token);
    storage?.setItem(USER_KEY, JSON.stringify(response.user));
    this._user.set(response.user);
  }

  private readUser(): UserDto | null {
    try {
      const raw = this.storage?.getItem(USER_KEY);
      return raw ? (JSON.parse(raw) as UserDto) : null;
    } catch {
      return null;
    }
  }

  private get storage(): Storage | null {
    return typeof localStorage === 'undefined' ? null : localStorage;
  }
}
