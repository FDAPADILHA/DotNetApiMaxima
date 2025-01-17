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
  url = 'http://localhost:5103/api/Departamento';

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
  AdicionarDepartamentos(departamento: Departamento[]): Observable<any> {
    return this.http.post(`${this.url}/AdicionarDepartamentos`, departamento, httpOptions);
  }

  // PUT: Atualizar departamentos
  AtualizarDepartamentos(departamento: any[]): Observable<any> {
    return this.http.put(`${this.url}/AtualizarDepartamentos`, departamento, httpOptions);
  }

  // DELETE: Inativar departamentos
  InativarDepartamentos(departamento: { Coddepto: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/InativarDepartamentos`, {
      body: departamento,
      ...httpOptions
    });
  }

  // DELETE: Excluir departamentos
  ExcluirDepartamentos(departamento: { Coddepto: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/ExcluirDepartamentos`, {
      body: departamento,
      ...httpOptions
    });
  }
}