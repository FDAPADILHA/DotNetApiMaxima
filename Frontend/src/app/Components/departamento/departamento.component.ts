import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DepartamentoService } from '../../departamento.service';
import { Departamento } from '../../Departamento';
//import { Router } from '@angular/router';

@Component({
  selector: 'app-departamento',
  templateUrl: './departamento.component.html',
  styleUrl: './departamento.component.css'
})
export class DepartamentoComponent implements OnInit {
  formulario: any;
  tituloFormulario: string | undefined; // Definido um valor
  departamento: Departamento[] | undefined;

  constructor(private departamentoService: DepartamentoService) { }

  ngOnInit(): void {

    this.departamentoService.ListarDepartamentoTodos().subscribe(resultado => {
      this.departamento = resultado;
    });

    this.tituloFormulario = 'Cadastro de Departamento';
    this.formulario = new FormGroup({
      coddepto: new FormControl('', [Validators.required]),
      descricao: new FormControl('', [Validators.required]),
      status: new FormControl('', [Validators.required])
      
    });
  }

  EnviarFormulario(): void {
    const departamento : Departamento = this.formulario.value;

    this.departamentoService.AdicionarDepartamentos(departamento).subscribe(resultado => {
      alert('Departamento cadastrado com sucesso');
    });
  }
}