import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-departamento',
  imports: [],
  templateUrl: './departamento.component.html',
  styleUrl: './departamento.component.css'
})
export class DepartamentoComponent {

  formulario: any;
  tituloFormulario: string;

  construtor() { }

  ngOnInit(): void {
    this.formulario = new FormGroup({
      
      coddepto: new FormControl(null),
      descricao: new FormControl(null),
      status: new FormControl(null)

    })

  }
}
