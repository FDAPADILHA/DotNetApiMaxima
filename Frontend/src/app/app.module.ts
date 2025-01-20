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


import { LoginComponent } from './Components/login/login.component';
import { FormsModule } from '@angular/forms'; 


@NgModule({
  declarations: [AppComponent, DepartamentoComponent, HeaderComponent, LoginComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatSidenavModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    ModalModule.forRoot(),
  ],
  providers: [HttpClientModule, DepartamentoService],
  bootstrap: [AppComponent, HeaderComponent],
})
export class AppModule {}
