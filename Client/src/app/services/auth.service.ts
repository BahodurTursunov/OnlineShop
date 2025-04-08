import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {User} from '../models/user';
import {IUser} from '../interfaces/user';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private apiUrl = 'https://localhost:7269/api/auth';

  constructor(private https: HttpClient) {
  }

  // Добавляем явную аннотацию типа для параметра и возвращаемого значения
  login(credentials: { username: string, password: string }): Observable<IUser> {
    return this.https.post<IUser>(`${this.apiUrl}/login`, credentials);
  }

  // Метод для сохранения токена
  setToken(token: string): void {
    localStorage.setItem('jwtToken', token);
  }

  // Получение токена
  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  // Удаление токена
  logout(): void {
    localStorage.removeItem('jwtToken');
  }

  // Проверка наличия токена
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}

