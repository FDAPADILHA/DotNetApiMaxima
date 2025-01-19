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
    this.departamentoService.ListarDepartamentoTodos().subscribe((data: any[]) => {
      this.departamentos = data;
    });
  }

  carregarDepartamentos(): void {
    this.departamentoService.ListarDepartamentoTodos().subscribe(
      (resultado) => {
        console.log('Dados retornados da API:', resultado);
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

  ExibirFormularioAtualizacao(departamento: Departamento): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = `Atualizar departamento: ${departamento.coddepto}`;
    this.emModoEdicao = true; // Indicando que estamos em modo de edição
  
    // Popula o formulário com os dados do departamento
    this.formulario = new FormGroup({
      coddepto: new FormControl({ value: departamento.coddepto, disabled: true }), // Campo desabilitado
      descricao: new FormControl(departamento.descricao),
      status: new FormControl(departamento.status),
    });
  }
  
  ExibirFormularioCadastro(): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;
    this.tituloFormulario = 'Adicionar Novo Departamento';
    this.emModoEdicao = false; // Indicando que estamos em modo de criação
  
    // Inicializa o formulário vazio
    this.formulario = new FormGroup({
      coddepto: new FormControl(''), // Inicializa como vazio, para a criação de novos departamentos
      descricao: new FormControl(''),
      status: new FormControl(''),
    });
  }
  
  EnviarFormulario(): void {
    const departamento: Departamento = this.formulario.value;
  
    // Certifique-se de que coddepto não esteja undefined ou null
    if (!departamento.coddepto && !this.emModoEdicao) {  // Não exige coddepto para criação
      alert('O código do departamento é obrigatório.');
      return;
    }
  
    // Envia o departamento no formato esperado pela API (array de objetos)
    const departamentoArray = [departamento]; 
  
    if (this.emModoEdicao) {
      // Lógica de atualização (PUT)
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
      // Lógica de criação (POST)
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
    console.log("Departamento recebido no método ExibirConfirmacaoExclusao:", departamento);
  
    if (!departamento || !departamento.coddepto) {
      console.error("O objeto departamento ou Coddepto está vazio.");
      return;
    }
  
    this.modalRef = this.modalService.show(conteudoModal);
    this.coddepto = departamento.coddepto;
    this.descricao = departamento.descricao;
  
    console.log("Coddepto atribuído:", this.coddepto);
    console.log("Descrição atribuída:", this.descricao);
  }

  ExcluirDepartamentos(): void {
    console.log("Iniciando exclusão com Coddepto:", this.coddepto);
  
    if (!this.coddepto) {
      alert("Código do departamento não fornecido.");
      console.log("Valor de this.coddepto:", this.coddepto);
      return;
    }
  
    const departamentoExcluirRequest = { Coddepto: this.coddepto };
    console.log("Payload enviado:", departamentoExcluirRequest);
  
    this.departamentoService.ExcluirDepartamentos([departamentoExcluirRequest]).subscribe(
      () => {
        console.log("Departamento excluído com sucesso.");
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
