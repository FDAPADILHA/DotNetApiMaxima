import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DepartamentoComponent } from './components/departamento/departamento.component';

const routes: Routes = [
  { path: '', redirectTo: '/departamento', pathMatch: 'full' },  // Redireciona para "departamento"
  { path: 'departamento', component: DepartamentoComponent },   // Rota para departamento
  // outras rotas...
];


@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
