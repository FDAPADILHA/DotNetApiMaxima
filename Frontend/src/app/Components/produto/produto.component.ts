import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProdutoService } from 'src/app/produto.service';
import { Produto } from 'src/app/Produto';

@Component({
  selector: 'app-produto',
  templateUrl: './produto.component.html',
  styleUrls: ['./produto.component.css'],
})
export class ProdutoComponent implements OnInit {
  produtos = [];
  formulario: FormGroup;
  formularioVisivel = false;
  modoEdicao = false;
  tituloFormulario = '';

  constructor(
    private fb: FormBuilder,
    private produtoService: ProdutoService
  ) {}

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.produtoService.ListarProdutosTodos().subscribe(
      (resultado) => {
        this.produtos = resultado || [];
        if (this.produtos.length === 0) {
          alert('Nenhum produto encontrado.');
        }
      },
      (erro) => {
        console.error('Erro ao carregar produtos:', erro);
        alert('Erro ao carregar os produtos. Tente novamente mais tarde.');
      }
    );
  }

  exibirFormulario(produto = null): void {
    this.formularioVisivel = true;
    this.modoEdicao = !!produto;
    this.tituloFormulario = produto ? 'Editar Produto' : 'Adicionar novo produto';

    this.formulario = this.fb.group({
      codprod: [produto?.codprod || '', [Validators.required]],
      descricao: [produto?.descricao || '', [Validators.required]],
      preco: [produto?.preco || 0, [Validators.required, Validators.min(0)]],
      status: [produto?.status || 'Ativo', [Validators.required]],
    });

    if (this.modoEdicao) this.formulario.get('codprod').disable();
  }

  salvarProduto(): void {
    if (this.formulario.invalid) return;

    const produto = this.formulario.getRawValue();

    if (this.modoEdicao) {
      this.produtoService.AtualizarProdutos(produto).subscribe(() => {
        this.carregarProdutos();
        this.fecharFormulario();
      });
    } else {
      this.produtoService.AdicionarProdutos(produto).subscribe(() => {
        this.carregarProdutos();
        this.fecharFormulario();
      });
    }
  }

  confirmarExclusao(produto): void {
    if (confirm(`Deseja excluir o produto ${produto.descricao}?`)) {
      this.produtoService.ExcluirProdutos(produto.codprod).subscribe(() =>
        this.carregarProdutos()
      );
    }
  }

  fecharFormulario(): void {
    this.formularioVisivel = false;
    this.formulario.reset();
  }
}
