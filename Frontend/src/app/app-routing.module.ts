import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { DepartamentoComponent } from './Components/departamento/departamento.component';
import { ProdutoComponent } from './Components/produto/produto.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },  // Redireciona para "login"
  { path: 'login', component: LoginComponent },   // Rota para login
  { path: 'departamento', component: DepartamentoComponent },   // Rota para departamento
  { path: 'produto', component: ProdutoComponent } //Rota para produto
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
