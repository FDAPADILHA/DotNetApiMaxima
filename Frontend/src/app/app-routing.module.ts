import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { DepartamentoComponent } from './Components/departamento/departamento.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },  // Redireciona para "login"
  { path: 'login', component: LoginComponent },   // Rota para login
  { path: 'departamento', component: DepartamentoComponent },   // Rota para departamento
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
