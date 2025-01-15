using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

        /*########################################################################################################################################################
        *******************************************************   // POST: api/Produto/AdicionarProduto   *******************************************************
        ########################################################################################################################################################*/

        [HttpPost("AdicionarProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarProdutos([FromBody] List<Produto> produtos)
        {
            try
            {
                if (produtos == null || !produtos.Any())
                {
                    return BadRequest("A lista de produtos não pode ser nula ou vazia.");
                }

                foreach (var produto in produtos)
                {
                    if (string.IsNullOrEmpty(produto.Coddepto) || produto.Preco <= 0)
                    {
                        return BadRequest($"O produto {produto.Codprod} é inválido: Código do departamento é obrigatório e o preço deve ser maior que zero.");
                    }

                    if (string.IsNullOrEmpty(produto.Status))
                    {
                        return BadRequest($"O produto {produto.Codprod} é inválido: O status A (Ativo) ou I (Inativo) é obrigatório.");
                    }

                    produto.Codoperacao = null;
                    produto.Id = null;
                }

                _contexto.Produto.AddRange(produtos);
                await _contexto.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Produtos adicionados com sucesso.",
                    Produtos = produtos.Select(p => new
                    {
                        p.Codprod,
                        p.Descricao,
                        p.Coddepto,
                        p.Preco,
                        p.Status
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor. Considere contatar o Suporte à API.", Details = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Produto/ExcluirProdutos   *******************************************************
        ########################################################################################################################################################*/

        // Para tratar vários produtos a serem excluídos
        public class ProdutoExcluirRequest
        {
            public required string Codprod { get; set; }
        }

        [HttpDelete("ExcluirProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirProdutos([FromBody] List<ProdutoExcluirRequest> produtos)
        {
            try
            {

                if (produtos == null || !produtos.Any())
                {
                    return BadRequest("Nenhum código de produto fornecido.");
                }

                var produtosEncontrados = await _contexto.Produto
                    .Where(p => produtos.Select(pr => pr.Codprod).Contains(p.Codprod))
                    .ToListAsync();

                if (!produtosEncontrados.Any())
                {
                    return NotFound("Nenhum dos produtos informados foi encontrado.");
                }

                foreach (var produto in produtosEncontrados)
                {
                    produto.Codoperacao = 2;
                    _contexto.Produto.Update(produto);
                }
                await _contexto.SaveChangesAsync();

                _contexto.Produto.RemoveRange(produtosEncontrados);
                await _contexto.SaveChangesAsync();

                string mensagem;
                if (produtosEncontrados.Count == 1)
                {
                    mensagem = $"Produto com o código {produtosEncontrados.First().Codprod} foi excluído com sucesso.";
                }
                else
                {
                    mensagem = $"Produtos com os códigos {string.Join(", ", produtosEncontrados.Select(p => p.Codprod))} foram excluídos com sucesso.";
                }

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor. Considere contatar o Suporte à API", Details = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Produto/ListarProdutosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ListarProdutosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarProdutosTodos()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Produto/ConsultarProdutos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ConsultarProdutos")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarProdutos([FromQuery] List<string> codprod)
        {
            try
            {
                if (codprod == null || !codprod.Any())
                {
                    return BadRequest(new { Message = "Nenhum código de produto fornecido." });
                }

                var produtosEncontrados = await _contexto.Produto
                    .Where(p => codprod.Contains(p.Codprod.Trim()))
                    .Select(p => new
                    {
                        p.Codprod,
                        p.Descricao,
                        p.Coddepto,
                        p.Preco,
                        p.Status
                    })
                    .ToListAsync();

                if (!produtosEncontrados.Any())
                {
                    return NotFound(new { Message = "Nenhum dos produtos informados foi encontrado." });
                }

                return Ok(produtosEncontrados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Erro interno no servidor. Considere contatar o Suporte à API.",
                    Details = ex.Message
                });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // PUT: api/Produto/AtualizarProdutos   *******************************************************
        ########################################################################################################################################################*/

        [HttpPut("AtualizarProduto")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AtualizarProduto([FromBody] Produto produto)
        {
            try
            {
                if (string.IsNullOrEmpty(produto.Codprod))
                {
                    return BadRequest("O código do produto (Codprod) é obrigatório.");
                }

                var produtoExistente = await _contexto.Produto
                    .FirstOrDefaultAsync(p => p.Codprod == produto.Codprod);

                if (produtoExistente == null)
                {
                    return NotFound($"Produto com código {produto.Codprod} não encontrado.");
                }

                // Atualizando os campos do produto com os dados recebidos
                produtoExistente.Descricao = produto.Descricao ?? produtoExistente.Descricao;
                produtoExistente.Coddepto = produto.Coddepto ?? produtoExistente.Coddepto;
                produtoExistente.Preco = produto.Preco ?? produtoExistente.Preco;
                produtoExistente.Status = produto.Status ?? produtoExistente.Status;

                // Definindo os campos Codoperacao e Idprod como null
                produtoExistente.Codoperacao = null;
                produtoExistente.Id = null;

                // Atualizando o produto no banco de dados
                _contexto.Produto.Update(produtoExistente);
                await _contexto.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Produto atualizado com sucesso.",
                    Produto = new
                    {
                        produtoExistente.Codprod,
                        produtoExistente.Descricao,
                        produtoExistente.Coddepto,
                        produtoExistente.Preco,
                        produtoExistente.Status
                    }
                });
            }
            catch (Exception ex)
            {
                // Detalha o erro de forma mais informativa
                return StatusCode(500, new { Message = "Erro interno no servidor. Considere contatar o Suporte à API.", Details = ex.Message });
            }
        }










    }
}
