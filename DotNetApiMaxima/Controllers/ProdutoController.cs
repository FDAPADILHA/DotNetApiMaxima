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
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }

        //Para tratar vários produtos a serem excluídos
        public class ProdutoExcluirRequest
        {
            public string Codprod { get; set; }
        }

        // Endpoint para excluir um ou mais produtos
        [HttpDelete("ExcluirProduto")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirProduto([FromBody] List<ProdutoExcluirRequest> produtos)
        {
            try
            {
                // Verifica se a lista de códigos foi fornecida
                if (produtos == null || !produtos.Any())
                {
                    return BadRequest("Nenhum código de produto fornecido.");
                }

                // Busca os produtos na base de dados
                var produtosEncontrados = await _contexto.Produto
                    .Where(p => produtos.Select(pr => pr.Codprod).Contains(p.Codprod))
                    .ToListAsync();

                // Verifica se os produtos foram encontrados
                if (!produtosEncontrados.Any())
                {
                    return NotFound("Nenhum dos produtos informados foi encontrado.");
                }

                // Atualiza os produtos antes de excluí-los
                foreach (var produto in produtosEncontrados)
                {
                    produto.Codoperacao = 2;
                    _contexto.Produto.Update(produto);
                }
                await _contexto.SaveChangesAsync();

                // Exclui os produtos
                _contexto.Produto.RemoveRange(produtosEncontrados);
                await _contexto.SaveChangesAsync(); // Salva as exclusões

                // Cria a mensagem de sucesso com base no número de produtos excluídos
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
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }


        // GET: api/Produto/ListarProdutos
        [HttpGet("ListarProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarProdutos()
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


        // GET: api/Produto/ConsultarProduto/{Codprod}
        [HttpGet("ConsultarProduto/{Codprod}")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarProduto(string Codprod)
        {
            if (string.IsNullOrWhiteSpace(Codprod))
            {
                return BadRequest(new { Message = "O parâmetro Codprod é obrigatório." });
            }

            try
            {
                var produto = await _contexto.Produto
                    .Where(p => p.Codprod.Trim() == Codprod.Trim())
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

                return NotFound(new { Message = $"Produto com Codprod {Codprod} não encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }

    }
}
