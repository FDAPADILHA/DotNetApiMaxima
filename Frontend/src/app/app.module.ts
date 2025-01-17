import { DepartamentoService } from './departamento.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from "ngx-bootstrap/modal";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DepartamentoComponent } from './Components/departamento/departamento.component';

@NgModule({
    declarations: [
        AppComponent,
        DepartamentoComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        ModalModule.forRoot()
    ],
    providers: [DepartamentoService], // HttpClientModule não precisa estar em providers
    bootstrap: [DepartamentoComponent]
})
export class AppModule { }