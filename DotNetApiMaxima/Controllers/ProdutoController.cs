using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace DotNetApiMaxima.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly Contexto _contexto;

        public ProdutoController(Contexto contexto) => _contexto = contexto;

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Produto/ListarProdutosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ListarProdutosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarProdutosTodos()
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

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Produto/ConsultarProdutos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ConsultarProdutos")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarProdutos([FromQuery] List<string> codprod)
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

        /*########################################################################################################################################################
        *******************************************************   // POST: api/Produto/AdicionarProdutos   *******************************************************
        ########################################################################################################################################################*/

        [HttpPost("AdicionarProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarProdutos([FromBody] List<Produto> produtos)
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

        /*########################################################################################################################################################
        *******************************************************   // PUT: api/Produto/AtualizarProdutos   *******************************************************
        ########################################################################################################################################################*/

        public class ProdutoAtualizarRequest
        {
            public required string Codprod { get; set; }
            public string? Descricao { get; set; }
            public string? Coddepto { get; set; }
            public decimal? Preco { get; set; }
            public string? Status { get; set; }
        }

        [HttpPut("AtualizarProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AtualizarProdutos([FromBody] List<ProdutoAtualizarRequest> produtos)
        {
            foreach (var produto in produtos)
            {
                if (string.IsNullOrEmpty(produto.Codprod))
                    return BadRequest("O código do produto é obrigatório para a atualização.");

                var sql = new StringBuilder("UPDATE MXSPRODUTO SET ");
                var parametros = new List<OracleParameter>();

                if (!string.IsNullOrEmpty(produto.Descricao))
                {
                    sql.Append("DESCRICAO = :Descricao, ");
                    parametros.Add(new OracleParameter(":Descricao", produto.Descricao));
                }
                if (!string.IsNullOrEmpty(produto.Coddepto))
                {
                    sql.Append("CODDEPTO = :Coddepto, ");
                    parametros.Add(new OracleParameter(":Coddepto", produto.Coddepto));
                }
                if (produto.Preco > 0)
                {
                    sql.Append("PRECO = :Preco, ");
                    parametros.Add(new OracleParameter(":Preco", produto.Preco));
                }
                if (!string.IsNullOrEmpty(produto.Status))
                {
                    sql.Append("STATUS = :Status, ");
                    parametros.Add(new OracleParameter(":Status", produto.Status));

                    if (produto.Status == "A")
                    {
                        sql.Append("CODOPERACAO = :Codoperacao, ");
                        parametros.Add(new OracleParameter(":Codoperacao", 1));
                    }
                }

                if (sql[^2] == ',')
                {
                    sql.Length -= 2;
                }

                sql.Append(" WHERE CODPROD = :Codprod");
                parametros.Add(new OracleParameter(":Codprod", produto.Codprod));

                var rowsAffected = await _contexto.Database.ExecuteSqlRawAsync(sql.ToString(), parametros.ToArray());

                if (rowsAffected == 0)
                {
                    return NotFound($"Produto com código {produto.Codprod} não encontrado.");
                }
            }

            return Ok(new { message = "Produtos atualizados com sucesso." });
        }
        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Produto/InativarProdutos   *******************************************************
        ########################################################################################################################################################*/

        public class ProdutoInativarrRequest
        {
            public required string Codprod { get; set; }
        }

        [HttpDelete("InativarProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> InativarProdutos([FromBody] List<ProdutoInativarrRequest> produtos)
        {
            if (produtos == null || !produtos.Any())
            {
                return BadRequest("Nenhum código de produto fornecido.");
            }

            var codprodList = produtos.Select(pr => pr.Codprod).ToList();

            var produtosEncontrados = await _contexto.Produto
                .Where(p => codprodList.Contains(p.Codprod))
                .ToListAsync();

            if (!produtosEncontrados.Any())
            {
                return NotFound("Nenhum dos produtos informados foi encontrado.");
            }

            foreach (var produto in produtosEncontrados)
            {
                produto.Status = "I";
                _contexto.Produto.Update(produto);
            }

            await _contexto.SaveChangesAsync();

            var codprodNaoEncontrados = codprodList
                .Except(produtosEncontrados.Select(p => p.Codprod))
                .ToList();

            var mensagemRetorno = new
            {
                Mensagem = "Operação concluída.",
                ProdutosInativados = produtosEncontrados.Select(p => new { p.Codprod, p.Descricao }),
                ProdutosNaoEncontrados = codprodNaoEncontrados
            };

            return Ok(mensagemRetorno);
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Produto/ExcluirProdutos   *******************************************************
        ########################################################################################################################################################*/

        public class ProdutoExcluirRequest
        {
            public required string Codprod { get; set; }
        }

        [HttpDelete("ExcluirProdutos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirProdutos([FromBody] List<ProdutoExcluirRequest> produtos)
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

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Produto/ExcluirProdutosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpDelete("ExcluirProdutosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirTodosProdutos()
        {
            var produtos = await _contexto.Produto.ToListAsync();

            if (produtos == null || !produtos.Any())
            {
                return NotFound("Nenhum produto encontrado para exclusão.");
            }

            // Define Codoperacao = 2 para todos os produtos
            foreach (var produto in produtos)
            {
                produto.Codoperacao = 2;
                _contexto.Produto.Update(produto);
            }
            await _contexto.SaveChangesAsync();

            // Remove todos os produtos
            _contexto.Produto.RemoveRange(produtos);
            await _contexto.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Todos os produtos foram excluídos com sucesso.",
                QuantidadeExcluida = produtos.Count
            });
        }
    }
}