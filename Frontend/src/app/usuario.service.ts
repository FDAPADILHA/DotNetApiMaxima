import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

// Definindo um modelo de resposta da API
export interface LoginResponse {
  token: string; // Ou qualquer outro campo que sua API retorne após o login
  usuario: { id: number, nome: string, login: string, status: string };
}

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private apiUrl = 'http://localhost:5103/api/Usuario'; // Ajuste para a URL correta da sua API

  constructor(private http: HttpClient) {}

  // Método para login
  login(loginData: { username: string; password: string }): Observable<LoginResponse> {
    // Aqui, utilizando POST, com o loginData no corpo da requisição
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, loginData);
  }

  // Método para consultar os usuários após o login
  consultarUsuarios(ids: number[]): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/ConsultarUsuarios`, {
      params: { id: ids.join(',') } // Enviando os ids como parâmetro
    });
  }
}
