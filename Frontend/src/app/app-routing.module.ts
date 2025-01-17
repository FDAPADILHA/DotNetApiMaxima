import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DepartamentoComponent } from './Components/departamento/departamento.component';

const routes: Routes = [
  { path: 'departamento', component: DepartamentoComponent }
  // outras rotas se houver
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
