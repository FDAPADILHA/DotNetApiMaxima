using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;

namespace DotNetApiMaxima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentoController : ControllerBase
    {
        private readonly Contexto _contexto;

        public DepartamentoController(Contexto contexto) => _contexto = contexto;

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Departamento/ListarDepartamentosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ListarDepartamentosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarDepartamentosTodos()
        {
            var lista = await _contexto.Departamento
                .Select(d => new
                {
                    d.Coddepto,
                    d.Descricao,
                    d.Status
                })
                .ToListAsync();

            if (lista == null || !lista.Any())
            {
                return NotFound("Nenhum departamento cadastrado.");
            }

            return Ok(lista);
        }

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Departamento/ConsultarDepartamentos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ConsultarDepartamentos")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarDepartamento([FromQuery] List<string> coddepto)
        {
            if (coddepto == null || !coddepto.Any())
            {
                return BadRequest(new { Message = "Nenhum código de departamento fornecido." });
            }

            var departamentosEncontrados = await _contexto.Departamento
                .Where(d => coddepto.Contains(d.Coddepto.Trim()))
                .Select(d => new
                {
                    d.Coddepto,
                    d.Descricao,
                    d.Status
                })
                .ToListAsync();

            if (!departamentosEncontrados.Any())
            {
                return NotFound(new { Message = "Nenhum dos departamentos informados foi encontrado." });
            }

            return Ok(departamentosEncontrados);
        }

        /*########################################################################################################################################################
        *******************************************************   // POST: api/Departamento/AdicionarDepartamentos   *******************************************************
        ########################################################################################################################################################*/

        [HttpPost("AdicionarDepartamentos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarDepartamentos([FromBody] List<Departamento> departamentos)
        {
            if (departamentos == null || !departamentos.Any())
            {
                return BadRequest(new { Message = "A lista de departamentos não pode ser nula ou vazia." });
            }

            foreach (var departamento in departamentos)
            {
                if (string.IsNullOrEmpty(departamento.Descricao))
                {
                    return BadRequest(new { Message = $"O departamento com código {departamento.Coddepto} é inválido: A descrição é obrigatória." });
                }

                if (departamento.Status != "A" && departamento.Status != "I")
                {
                    return BadRequest(new { Message = $"O departamento {departamento.Descricao} é inválido: O status deve ser 'A' (Ativo) ou 'I' (Inativo)." });
                }
            }

            _contexto.Departamento.AddRange(departamentos);
            await _contexto.SaveChangesAsync();

            return Ok(new
            {
                Message = "Departamentos adicionados com sucesso.",
                Departamentos = departamentos.Select(d => new
                {
                    d.Coddepto,
                    d.Descricao,
                    d.Status
                })
            });
        }

        /*########################################################################################################################################################
        *******************************************************   // PUT: api/Departamento/AtualizarDepartamentos   *******************************************************
        ########################################################################################################################################################*/

        public class DepartamentoAtualizarRequest
        {
            public required string Coddepto { get; set; }
            public string? Descricao { get; set; }
            public string? Status { get; set; }
        }

        [HttpPut("AtualizarDepartamentos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AtualizarDepartamentos([FromBody] List<DepartamentoAtualizarRequest> departamentos)
        {
            if (departamentos == null || !departamentos.Any())
            {
                return BadRequest(new { Message = "A lista de departamentos não pode ser nula ou vazia." });
            }

            var codsNaoEncontrados = new List<string>();

            foreach (var departamento in departamentos)
            {
                var sql = new StringBuilder("UPDATE MXSDEPARTAMENTO SET ");
                var parametros = new List<OracleParameter>();

                if (!string.IsNullOrEmpty(departamento.Descricao))
                {
                    sql.Append("DESCRICAO = :Descricao, ");
                    parametros.Add(new OracleParameter(":Descricao", departamento.Descricao));
                }

                if (!string.IsNullOrEmpty(departamento.Status))
                {
                    sql.Append("STATUS = :Status, ");
                    parametros.Add(new OracleParameter(":Status", departamento.Status));
                }

                if (sql[sql.Length - 2] == ',')
                {
                    sql.Length -= 2;
                }

                sql.Append(" WHERE CODDEPTO = :Coddepto");
                parametros.Add(new OracleParameter(":Coddepto", departamento.Coddepto));

                var rowsAffected = await _contexto.Database.ExecuteSqlRawAsync(sql.ToString(), parametros.ToArray());

                if (rowsAffected == 0)
                {
                    codsNaoEncontrados.Add(departamento.Coddepto);
                }
            }

            if (codsNaoEncontrados.Any())
            {
                return NotFound(new
                {
                    Message = "Alguns departamentos não foram encontrados.",
                    DepartamentosNaoEncontrados = codsNaoEncontrados
                });
            }

            return Ok(new { Message = "Departamentos atualizados com sucesso." });
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Departamento/InativarDepartamentos   *******************************************************
        ########################################################################################################################################################*/

        public class DepartamentoInativarRequest
        {
            public required string Coddepto { get; set; }
        }

        [HttpDelete("InativarDepartamentos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> InativarDepartamentos([FromBody] List<DepartamentoInativarRequest> departamentos)
        {
            if (departamentos == null || !departamentos.Any())
            {
                return BadRequest("Nenhum código de departamento fornecido.");
            }

            var codsDepartamentos = departamentos.Select(d => d.Coddepto).ToList();

            var departamentosEncontrados = await _contexto.Departamento
                .Where(d => codsDepartamentos.Contains(d.Coddepto))
                .ToListAsync();

            if (!departamentosEncontrados.Any())
            {
                return NotFound("Nenhum dos departamentos informados foi encontrado.");
            }

            foreach (var departamento in departamentosEncontrados)
            {
                departamento.Status = "I";
                _contexto.Departamento.Update(departamento);
            }

            await _contexto.SaveChangesAsync();

            var codsNaoEncontrados = codsDepartamentos
                .Except(departamentosEncontrados.Select(d => d.Coddepto))
                .ToList();

            var mensagemRetorno = new
            {
                Mensagem = "Operação concluída.",
                DepartamentosInativados = departamentosEncontrados.Select(d => new { d.Coddepto, d.Descricao }),
                DepartamentosNaoEncontrados = codsNaoEncontrados
            };

            return Ok(mensagemRetorno);
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Departamento/ExcluirDepartamentos   *******************************************************
        ########################################################################################################################################################*/

        public class DepartamentoExcluirRequest
        {
            public required string Coddepto { get; set; }
        }

        [HttpDelete("ExcluirDepartamentos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirDepartamentos([FromBody] List<DepartamentoExcluirRequest> departamentos)
        {
            if (departamentos == null || !departamentos.Any())
            {
                return BadRequest("Nenhum código de departamento fornecido.");
            }

            var codsDepartamentos = departamentos.Select(d => d.Coddepto).ToList();

            var departamentosEncontrados = await _contexto.Departamento
                .Where(d => codsDepartamentos.Contains(d.Coddepto))
                .ToListAsync();

            if (!departamentosEncontrados.Any())
            {
                return NotFound("Nenhum dos departamentos informados foi encontrado.");
            }

            _contexto.Departamento.RemoveRange(departamentosEncontrados);
            await _contexto.SaveChangesAsync();

            var mensagem = departamentosEncontrados.Count == 1
                ? $"Departamento com código {departamentosEncontrados[0].Coddepto} excluído com sucesso."
                : $"{departamentosEncontrados.Count} departamentos excluídos com sucesso.";

            return Ok(new { Message = mensagem });
        }
    }
}