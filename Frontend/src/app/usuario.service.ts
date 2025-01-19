import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Definindo o modelo da resposta do login
export interface LoginResponse {
  token: string;  // O token JWT gerado
  usuario: { id: number; nome: string; login: string; status: string }; // Informações do usuário
}

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private apiUrl = 'http://localhost:5103/api/Usuario'; // URL da sua API

  constructor(private http: HttpClient) {}

  // Método de login
  login(loginData: { login: string; senha: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, loginData);
  }

  // Método para consultar os usuários (exemplo de outro método protegido, mas não é necessário agora)
  consultarUsuarios(ids: number[]): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = { 'Authorization': `Bearer ${token}` };

    return this.http.get(`${this.apiUrl}/ConsultarUsuarios`, {
      headers,
      params: { id: ids.join(',') }
    });
  }
}
