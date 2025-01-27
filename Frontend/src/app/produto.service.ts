import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Produto } from './Produto';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  url = 'http://localhost:5103/api/Produto';

  constructor(private http: HttpClient) {}

  // GET: Listar todos os produtos
  ListarProdutosTodos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(`${this.url}/ListarProdutosTodos`);
  }

  // GET: Consultar produtos por lista de códigos
  ConsultarProdutos(codprod: string[]): Observable<Produto[]> {
    const params = new HttpParams().set('codprod', codprod.join(','));
    return this.http.get<Produto[]>(`${this.url}/ConsultarProdutos`, { params });
  }

  // POST: Adicionar produtos
  AdicionarProdutos(produto: Produto[]): Observable<any> {
    return this.http.post(`${this.url}/AdicionarProdutos`, produto, httpOptions);
  }

  // PUT: Atualizar produtos
  AtualizarProdutos(produto: any[]): Observable<any> {
    return this.http.put(`${this.url}/AtualizarProdutos`, produto, httpOptions);
  }

  // DELETE: Inativar produtos
  InativarProdutos(produto: { codprod: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/InativarProdutos`, {
      body: produto,
      ...httpOptions
    });
  }

  // DELETE: Excluir produtos
  ExcluirProdutos(produto: { codprod: string }[]): Observable<any> {
    return this.http.request('delete', `${this.url}/ExcluirProdutos`, {
      body: produto,
      ...httpOptions
    });
  }
}