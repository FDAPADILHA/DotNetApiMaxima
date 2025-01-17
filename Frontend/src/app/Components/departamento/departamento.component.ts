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
      Status: new FormControl('A'), // Define um status padrão como "Ativo".
    });
  }
  
  ExibirFormularioAtualizacao(departamento: Departamento): void {
    this.visibilidadeTabela = false;
    this.visibilidadeFormulario = true;

    this.departamentoService.ConsultarDepartamentos([departamento.Coddepto]).subscribe((resultado) => {
      const dept = resultado[0];  // Supondo que o retorno seja um array com o departamento encontrado
      this.tituloFormulario = `Atualizar ${dept.Coddepto} - ${dept.Descricao}`;

      this.formulario = new FormGroup({
        Coddepto: new FormControl(dept.Coddepto),
        Descricao: new FormControl(dept.Descricao),
        Status: new FormControl(dept.Status),
      });
    });
  }

  EnviarFormulario(): void {
    const departamento: Departamento = this.formulario.value;

    // Verifique se o Coddepto existe para diferenciar entre criação e atualização
    if (departamento.Coddepto) {
      // Atualizar
      this.departamentoService.AtualizarDepartamentos([departamento]).subscribe(() => {  // Coloque o departamento dentro de um array
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento atualizado com sucesso');
        this.carregarDepartamentos();
      });
    } else {
      // Adicionar
      this.departamentoService.AdicionarDepartamentos([departamento]).subscribe(() => {  // Coloque o departamento dentro de um array
        this.visibilidadeFormulario = false;
        this.visibilidadeTabela = true;
        alert('Departamento cadastrado com sucesso');
        this.carregarDepartamentos();
      });
    }
    
  }

  Voltar(): void {
    this.visibilidadeTabela = true;
    this.visibilidadeFormulario = false;
  }

  ExibirConfirmacaoExclusao(Coddepto: string, Descricao: string, conteudoModal: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(conteudoModal);
    this.coddepto = Coddepto;
    this.descricao = Descricao;
  }

  ExcluirDepartamentos(): void {
    const departamento: Departamento = { Coddepto: this.coddepto, Descricao: this.descricao, Status: 'Inativo' };
  
    // Envolva o objeto em um array
    this.departamentoService.ExcluirDepartamentos([departamento]).subscribe(() => {
      this.modalRef.hide();
      alert('Departamento excluído com sucesso');
      this.carregarDepartamentos();
    });
  }
  
}
