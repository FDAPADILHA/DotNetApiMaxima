using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetApiMaxima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly Contexto _contexto;

        public ProdutoController(Contexto contexto)
        {
            _contexto = contexto;
        }

        // POST: api/Produto/AdicionarProduto
        [HttpPost("AdicionarProduto")]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            if (produto == null)
            {
                return BadRequest("Produto não pode ser nulo.");
            }

            if (string.IsNullOrEmpty(produto.Coddepto) || produto.Preco <= 0)
            {
                return BadRequest("Código do departamento é obrigatório e o preço deve ser maior que zero.");
            }

            if (string.IsNullOrEmpty(produto.Status))
            {
                return BadRequest("O Status do produto A - Ativo ou I - Inativo é obrigatório.");
            }

            produto.Codoperacao = null;
            produto.Id = null;

            _contexto.Produto.Add(produto);
            await _contexto.SaveChangesAsync();

            return Ok(new
            {
                produto.Codprod,
                produto.Descricao,
                produto.Coddepto,
                produto.Preco,
                produto.Status
            });
        }

        // POST: api/Produto/ExcluirProduto/{Codprod}
        [HttpPost("ExcluirProduto/{Codprod}")]
        public async Task<IActionResult> ExcluirProduto(string Codprod)
        {
            var produto = await _contexto.Produto.FirstOrDefaultAsync(p => p.Codprod == Codprod);

            if (produto != null)
            {
                produto.Codoperacao = 2;
                _contexto.Produto.Update(produto);
                await _contexto.SaveChangesAsync();

                _contexto.Produto.Remove(produto);
                await _contexto.SaveChangesAsync();

                return Ok($"Produto com Codprod {Codprod} foi excluído com sucesso.");
            }

            return NotFound($"Produto com Codprod {Codprod} não encontrado.");
        }

        // GET: api/Produto/ListarProdutos
        [HttpGet("ListarProdutos")]
        public async Task<IActionResult> ListarProdutos()
        {
            var lista = await _contexto.Produto
                .Select(p => new
                {
                    p.Codprod,
                    p.Descricao,
                    p.Coddepto,
                    p.Preco,
                    p.Status
                })
                .ToListAsync();

            if (lista == null || !lista.Any())
            {
                return NotFound("Nenhum produto cadastrado.");
            }

            return Ok(lista);
        }

        // GET: api/Produto/ConsultarProduto/{Codprod}
        [HttpGet("ConsultarProduto/{Codprod}")]
        public async Task<IActionResult> ConsultarProduto(string Codprod)
        {
            var produto = await _contexto.Produto
                .Where(p => p.Codprod == Codprod)
                .Select(p => new
                {
                    p.Codprod,
                    p.Descricao,
                    p.Coddepto,
                    p.Preco,
                    p.Status
                })
                .FirstOrDefaultAsync();

            if (produto != null)
            {
                return Ok(produto);
            }

            return NotFound($"Produto com Codprod {Codprod} não encontrado.");
        }
    }
}
