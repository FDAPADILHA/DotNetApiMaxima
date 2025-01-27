import { DepartamentoService } from './departamento.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DepartamentoComponent } from './Components/departamento/departamento.component';
import { HeaderComponent } from './Components/header/header.component';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';



import { LoginComponent } from './Components/login/login.component';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { ProdutoComponent } from './Components/produto/produto.component';
import { ProdutoService } from './produto.service';


@NgModule({
  declarations: [AppComponent, DepartamentoComponent, HeaderComponent, LoginComponent, NavbarComponent, ProdutoComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatSidenavModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    FormsModule,
    MatCardModule,
    ModalModule.forRoot(),
    BrowserAnimationsModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule
  ],
  providers: [HttpClientModule, DepartamentoService, ProdutoService],
  bootstrap: [AppComponent],
})
export class AppModule {}
