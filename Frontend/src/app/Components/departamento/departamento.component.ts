import { Component, OnInit, TemplateRef } from '@angular/core';
import { DepartamentoService } from './departamento.service';
import { FormControl, FormGroup } from '@angular/forms';
import { Departamento } from 'src/app/Departamento';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-departamento',
  templateUrl: './departamento.component.html',
  styleUrls: ['./departamento.component.css'],
})
export class departamentoComponent implements OnInit {
  formulario: any;
  tituloFormulario: string;
  departamento: Departamento[];
  Descricao: string;
  Coddepto: string;

  visibilidadeTabela: boolean = true;
  visibilidadeFormulario: boolean = false; 
  
  modalRef: BsModalRef;

  constructor(private DepartamentoService: DepartamentoService,
    private modalService: BsModalService) {}

  ngOnInit(): void {
    this.DepartamentoService.ListarDepartamentoTodos().subscribe((resultado) => {
      this.departamento = resultado;
    });
  }

  ExibirFormularioCadastro(): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = 'Novo Departamento';
    this.formulario = new FormGroup({
      Coddepto: new FormControl(null),
      Descricao: new FormControl(null),
      Status: new FormControl(null)
    });
  }

  ExibirFormularioAtualizacao(departamento): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;

    this.DepartamentoService.ConsultarDepartamentos(departamento).subscribe((resultado) => {
      this.tituloFormulario = `Atualizar ${resultado.Coddepto} ${resultado.Descricao}`;

      this.formulario = new FormGroup({
        Coddepto: new FormControl(resultado.Coddepto),
        Descricao: new FormControl(resultado.Descricao),
        Status: new FormControl(resultado.Status),
      });
    });
  }

  EnviarFormulario(): void {
    const departamento: Departamento = this.formulario.value;

    if (departamento.Coddepto > 0) {
      this.DepartamentoService.AtualizarDepartamentos(departamento).subscribe((resultado) => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento atualizado com sucesso');
        this.DepartamentoService.ListarDepartamentoTodos().subscribe((registros) => {
          this.departamento = registros;
        });
      });
    } else {
      this.DepartamentoService.AdicionarDepartamentos(departamento).subscribe((resultado) => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento inserido com sucesso');
        this.DepartamentoService.ListarDepartamentosTodos().subscribe((registros) => {
          this.departamento = registros;
        });
      });
    }
  }

  Voltar(): void {
    this.visibilidadeTabela = true;
    this.visibilidadeFormulario = false;
  }

  ExibirConfirmacaoExclusao(Coddepto, Descricao, conteudoModal: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(conteudoModal);
    this.Coddepto = Coddepto;
    this.Descricao = Descricao;
  }

  ExcluirDepartamentos(Coddepto){
    this.DepartamentoService.ExcluirDepartamentos(Coddepto).subscribe(resultado => {
      this.modalRef.hide();
      alert('Departamento excluído com sucesso');
      this.DepartamentoService.ListarDepartamentosTodos().subscribe(registros => {
        this.departamento = registros;
      });
    });
  }
}
