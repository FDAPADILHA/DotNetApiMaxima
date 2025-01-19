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
  descricao: string;
  coddepto: string;
  status: string;

  visibilidadeTabela: boolean = true;
  visibilidadeFormulario: boolean = false;

  modalRef: BsModalRef;

  constructor(
    private departamentoService: DepartamentoService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.carregarDepartamentos();
    this.departamentoService.ListarDepartamentoTodos().subscribe((data: any[]) => {
      this.departamentos = data;
    });
  }

  carregarDepartamentos(): void {
    this.departamentoService.ListarDepartamentoTodos().subscribe(
      (resultado) => {
        console.log('Dados retornados da API:', resultado); // Log dos dados retornados
        if (resultado && resultado.length > 0) {
          this.departamentos = resultado;
        } else {
          alert('Nenhum departamento encontrado.');
          this.departamentos = []; // Garante que a tabela não exiba dados antigos.
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
    this.tituloFormulario = 'Novo Departamento';
    this.formulario = new FormGroup({
      Coddepto: new FormControl(null),
      Descricao: new FormControl(null),
      Status: new FormControl('A'), // Status padrão
    });
  }

  ExibirFormularioAtualizacao(departamento: Departamento): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = `Atualizar ${departamento.Coddepto} - ${departamento.Descricao}`;

    this.formulario = new FormGroup({
      Coddepto: new FormControl(departamento.Coddepto),
      Descricao: new FormControl(departamento.Descricao),
      Status: new FormControl(departamento.Status),
    });
  }

  EnviarFormulario(): void {
    const departamento: Departamento = this.formulario.value;

    if (departamento.Coddepto) {
      this.departamentoService.AdicionarDepartamentos([departamento]).subscribe(() => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento Adicionado com sucesso');
        this.carregarDepartamentos();
      });
    } else {
      this.departamentoService.AtualizarDepartamentos([departamento]).subscribe(() => {
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento Atualizado com sucesso');
        this.carregarDepartamentos();
      });
    }
  }

  Voltar(): void {
    this.visibilidadeTabela = true;
    this.visibilidadeFormulario = false;
  }

  ExibirConfirmacaoExclusao(departamento: Departamento, conteudoModal: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(conteudoModal);
    this.coddepto = departamento.Coddepto;
    this.descricao = departamento.Descricao;
  }

  ExcluirDepartamentos(): void {
    if (!this.coddepto) {
      alert("Código do departamento não fornecido.");
      console.log("Valor de this.coddepto:", this.coddepto);  // Para verificar o valor
      return;
    }
  
    const departamentoExcluirRequest = { Coddepto: this.coddepto };
  
    this.departamentoService.ExcluirDepartamentos([departamentoExcluirRequest]).subscribe(
      () => {
        this.modalRef.hide();
        alert('Departamento excluído com sucesso');
        this.carregarDepartamentos(); // Verifique se esse método está funcionando corretamente
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