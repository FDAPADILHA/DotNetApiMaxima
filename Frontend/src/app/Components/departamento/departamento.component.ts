import { Component, OnInit, TemplateRef } from '@angular/core';
import { DepartamentoService } from 'src/app/departamento.service';
import { FormControl, FormGroup } from '@angular/forms';
import { Departamento } from 'src/app/Departamento';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-departamento',
  templateUrl: './departamento.component.html',
  styleUrls: ['./departamento.component.css'],
})

export class DepartamentoComponent implements OnInit {
  formulario: FormGroup;
  tituloFormulario: string;
  departamentos: Departamento[] = [];
  descricao: string | undefined;
  coddepto: string | undefined;
  status: string | undefined;
  emModoEdicao: boolean = false;

  visibilidadeTabela: boolean = true;
  visibilidadeFormulario: boolean = false;

  modalRef: BsModalRef;

  constructor(
    private departamentoService: DepartamentoService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.carregarDepartamentos();

  }

  carregarDepartamentos(): void {
    this.departamentoService.ListarDepartamentoTodos().subscribe(
      (resultado) => {
        if (resultado && resultado.length > 0) {
          // Filtra os departamentos com status 'A'
          this.departamentos = resultado.filter((departamento: any) => departamento.status === 'A');
  
          if (this.departamentos.length === 0) {
            alert('Nenhum departamento ativo encontrado.');
          }
        } else {
          alert('Nenhum departamento encontrado.');
          this.departamentos = [];
        }
      },
      (erro) => {
        console.error('Erro ao carregar departamentos:', erro);
        alert('Erro ao carregar os departamentos. Tente novamente mais tarde.');
      }
    );
  }
  
  ExibirFormularioCadastro(): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = 'Adicionar Novo Departamento';
    this.emModoEdicao = false;

    this.formulario = new FormGroup({
      coddepto: new FormControl(''),
      descricao: new FormControl(''),
      status: new FormControl(''),
    });

    this.formulario.get('coddepto')?.enable();
    this.formulario.get('descricao')?.enable();
    this.formulario.get('status')?.enable();

  }

  ExibirFormularioAtualizacao(departamento: Departamento): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = `Atualizar departamento: ${departamento.coddepto}`;
    this.emModoEdicao = true;
  
    // Criação do formulário de forma dinâmica
    this.formulario = new FormGroup({
      coddepto: new FormControl(departamento.coddepto),
      descricao: new FormControl(departamento.descricao),
      status: new FormControl(departamento.status),
    });

  }

  EnviarFormulario(): void {
    const departamento: Departamento = this.formulario.value;

    if (this.emModoEdicao && !departamento.coddepto) {
      alert('O código do departamento é obrigatório.');
      return;
    }

    const departamentoArray = [departamento];

    if (this.emModoEdicao) {
      this.departamentoService.AtualizarDepartamentos(departamentoArray).subscribe(() => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento atualizado com sucesso');
        this.carregarDepartamentos();
      }, erro => {
        console.error('Erro ao atualizar departamento', erro);
        alert('Erro ao atualizar o departamento');
      });
    } else {
      this.departamentoService.AdicionarDepartamentos(departamentoArray).subscribe(() => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento adicionado com sucesso');
        this.carregarDepartamentos();
      }, erro => {
        console.error('Erro ao adicionar departamento', erro);
        alert('Erro ao adicionar o departamento');
      });
    }
  }

  Voltar(): void {
    this.visibilidadeTabela = true;
    this.visibilidadeFormulario = false;
  }

  ExibirConfirmacaoExclusao(departamento: Departamento, conteudoModal: TemplateRef<any>): void {
    if (!departamento || !departamento.coddepto) {
      console.error("O objeto departamento ou Coddepto está vazio.");
      return;
    }
    this.modalRef = this.modalService.show(conteudoModal);
    this.coddepto = departamento.coddepto;
    this.descricao = departamento.descricao;
  }

  ExcluirDepartamentos(): void {
    if (!this.coddepto) {
      alert("Código do departamento não fornecido.");
      return;
    }

    const departamentoExcluirRequest = { Coddepto: this.coddepto };

    this.departamentoService.ExcluirDepartamentos([departamentoExcluirRequest]).subscribe(
      () => {
        this.modalRef.hide();
        alert('Departamento excluído com sucesso');
        this.carregarDepartamentos();
      },
      (erro) => {
        console.error("Erro ao excluir departamento", erro);
        if (erro.error && erro.error.errors) {
          alert(`Erro de validação: ${JSON.stringify(erro.error.errors)}`);
        } else {
          alert('Ocorreu um erro ao excluir o departamento.');
        }
      }
    );
  }
}