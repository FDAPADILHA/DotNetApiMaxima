import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Departamento } from './Departamento';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class DepartamentoService {
  url = 'https://localhost:7266/api/Departamento';

  constructor(private http: HttpClient) {}

  // GET: Listar todos os departamentos
  ListarDepartamentoTodos(): Observable<Departamento[]> {
    return this.http.get<Departamento[]>(`${this.url}/ListarDepartamentosTodos`);
  }

  // GET: Consultar departamentos por lista de códigos
  ConsultarDepartamentos(coddepto: string[]): Observable<Departamento[]> {
    const params = new HttpParams().set('coddepto', coddepto.join(','));
    return this.http.get<Departamento[]>(`${this.url}/ConsultarDepartamentos`, { params });
  }

  // POST: Adicionar departamentos
  AdicionarDepartamentos(departamentos: Departamento[]): Observable<any> {
    return this.http.post(`${this.url}/AdicionarDepartamentos`, departamentos, httpOptions);
  }

  // PUT: Atualizar departamentos
  AtualizarDepartamentos(departamentos: any[]): Observable<any> {
    return this.http.put(`${this.url}/AtualizarDepartamentos`, departamentos, httpOptions);
  }

  // DELETE: Inativar departamentos
  InativarDepartamentos(departamentos: { Coddepto: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/InativarDepartamentos`, {
      body: departamentos,
      ...httpOptions
    });
  }

  // DELETE: Excluir departamentos
  ExcluirDepartamentos(departamentos: { Coddepto: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/ExcluirDepartamentos`, {
      body: departamentos,
      ...httpOptions
    });
  }
}