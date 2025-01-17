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
    public class UsuarioController : ControllerBase
    {
        private readonly Contexto _contexto;

        public UsuarioController(Contexto contexto) => _contexto = contexto;

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Usuario/ListarUsuariosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ListarUsuariosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarUsuariosTodos()
        {
            try
            {
                var lista = await _contexto.Usuario
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Login,
                        u.Status
                    })
                    .ToListAsync();

                if (lista == null || !lista.Any())
                {
                    return NotFound("Nenhum usuário cadastrado.");
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Usuario/ConsultarUsuario   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ConsultarUsuario")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarUsuario([FromQuery] List<int> id)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return BadRequest(new { Message = "Nenhum código de usuário fornecido." });
                }

                var usuariosEncontrados = await _contexto.Usuario
                    .Where(u => id.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Login,
                        u.Status
                    })
                    .ToListAsync();

                if (!usuariosEncontrados.Any())
                {
                    return NotFound(new { Message = "Nenhum dos usuários informados foi encontrado." });
                }

                return Ok(usuariosEncontrados);
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
        *******************************************************   // POST: api/Usuario/AdicionarUsuarios   *******************************************************
        ########################################################################################################################################################*/

        [HttpPost("AdicionarUsuarios")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarUsuarios([FromBody] List<Usuario> usuarios)
        {
            try
            {
                if (usuarios == null || !usuarios.Any())
                {
                    return BadRequest(new { Message = "A lista de usuários não pode ser nula ou vazia." });
                }

                foreach (var usuario in usuarios)
                {
                    if (string.IsNullOrEmpty(usuario.Nome))
                    {
                        return BadRequest(new { Message = $"O usuário com ID {usuario.Id} é inválido: O nome é obrigatório." });
                    }

                    if (string.IsNullOrEmpty(usuario.Login))
                    {
                        return BadRequest(new { Message = $"O usuário {usuario.Nome} é inválido: O login é obrigatório." });
                    }

                    if (string.IsNullOrEmpty(usuario.Senha))
                    {
                        return BadRequest(new { Message = $"O usuário {usuario.Nome} é inválido: A senha é obrigatória." });
                    }

                    if (usuario.Status != "A" && usuario.Status != "I")
                    {
                        return BadRequest(new { Message = $"O usuário {usuario.Nome} é inválido: O status deve ser 'A' (Ativo) ou 'I' (Inativo)." });
                    }

                    usuario.Id = 0; // O ID será gerado automaticamente pelo banco de dados
                }

                _contexto.Usuario.AddRange(usuarios);
                await _contexto.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Usuários adicionados com sucesso.",
                    Usuarios = usuarios.Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Login,
                        u.Status
                    })
                });
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
        *******************************************************   // PUT: api/Usuario/AtualizarUsuarios   *******************************************************
        ########################################################################################################################################################*/

        public class UsuarioAtualizarRequest
        {
            public required int Id { get; set; }
            public string? Nome { get; set; }
            public string? Login { get; set; }
            public string? Senha { get; set; }
            public string? Status { get; set; }
        }

        [HttpPut("AtualizarUsuarios")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AtualizarUsuarios([FromBody] List<UsuarioAtualizarRequest> usuarios)
        {
            try
            {
                if (usuarios == null || !usuarios.Any())
                {
                    return BadRequest(new { Message = "A lista de usuários não pode ser nula ou vazia." });
                }

                if (usuarios.Any(u => u.Id <= 0))
                {
                    return BadRequest(new { Message = "Todos os usuários devem ter um ID válido (maior que zero)." });
                }

                var idsNaoEncontrados = new List<int>();

                foreach (var usuario in usuarios)
                {
                    var sql = new StringBuilder("UPDATE MXSUSUARIOS SET ");
                    var parametros = new List<OracleParameter>();

                    if (!string.IsNullOrEmpty(usuario.Nome))
                    {
                        sql.Append("NOME = :Nome, ");
                        parametros.Add(new OracleParameter(":Nome", usuario.Nome));
                    }
                    if (!string.IsNullOrEmpty(usuario.Login))
                    {
                        sql.Append("LOGIN = :Login, ");
                        parametros.Add(new OracleParameter(":Login", usuario.Login));
                    }
                    if (!string.IsNullOrEmpty(usuario.Senha))
                    {
                        sql.Append("SENHA = :Senha, ");
                        parametros.Add(new OracleParameter(":Senha", usuario.Senha));
                    }
                    if (!string.IsNullOrEmpty(usuario.Status))
                    {
                        sql.Append("STATUS = :Status, ");
                        parametros.Add(new OracleParameter(":Status", usuario.Status));
                    }

                    // Remove a vírgula final extra antes do WHERE
                    if (sql[sql.Length - 2] == ',')
                    {
                        sql.Length -= 2;
                    }

                    sql.Append(" WHERE IDUSUARIO = :Id");
                    parametros.Add(new OracleParameter(":Id", usuario.Id));

                    var rowsAffected = await _contexto.Database.ExecuteSqlRawAsync(sql.ToString(), parametros.ToArray());

                    if (rowsAffected == 0)
                    {
                        idsNaoEncontrados.Add(usuario.Id);
                    }
                }

                if (idsNaoEncontrados.Any())
                {
                    return NotFound(new
                    {
                        Message = "Alguns usuários não foram encontrados.",
                        UsuariosNaoEncontrados = idsNaoEncontrados
                    });
                }

                return Ok(new { Message = "Usuários atualizados com sucesso." });
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao atualizar os usuários: {ex.Message}");
                return StatusCode(500, new { Message = "Erro ao atualizar os usuários", Error = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Usuario/InativarUsuarios   *******************************************************
        ########################################################################################################################################################*/

        public class UsuarioInativarRequest
        {
            public required int Id { get; set; }
        }

        [HttpDelete("InativarUsuarios")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> InativarUsuarios([FromBody] List<UsuarioInativarRequest> usuarios)
        {
            try
            {
                if (usuarios == null || !usuarios.Any())
                {
                    return BadRequest("Nenhum ID de usuário fornecido.");
                }

                var idsUsuarios = usuarios.Select(u => u.Id).ToList();

                var usuariosEncontrados = await _contexto.Usuario
                    .Where(u => idsUsuarios.Contains(u.Id))
                    .ToListAsync();

                if (!usuariosEncontrados.Any())
                {
                    return NotFound("Nenhum dos usuários informados foi encontrado.");
                }

                foreach (var usuario in usuariosEncontrados)
                {
                    usuario.Status = "I";
                    _contexto.Usuario.Update(usuario);
                }

                await _contexto.SaveChangesAsync();

                var idsNaoEncontrados = idsUsuarios
                    .Except(usuariosEncontrados.Select(u => u.Id))
                    .ToList();

                var mensagemRetorno = new
                {
                    Mensagem = "Operação concluída.",
                    UsuariosInativados = usuariosEncontrados.Select(u => new { u.Id, u.Nome }),
                    UsuariosNaoEncontrados = idsNaoEncontrados
                };

                return Ok(mensagemRetorno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }

        /*########################################################################################################################################################
        *******************************************************   // DELETE: api/Usuario/ExcluirUsuarios   *******************************************************
        ########################################################################################################################################################*/

        public class UsuarioExcluirRequest
        {
            public required int Id { get; set; }
        }

        [HttpDelete("ExcluirUsuarios")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ExcluirUsuarios([FromBody] List<UsuarioExcluirRequest> usuarios)
        {
            try
            {
                if (usuarios == null || !usuarios.Any())
                {
                    return BadRequest("Nenhum ID de usuário fornecido.");
                }

                var idsUsuarios = usuarios.Select(u => u.Id).ToList();

                var usuariosEncontrados = await _contexto.Usuario
                    .Where(u => idsUsuarios.Contains(u.Id))
                    .ToListAsync();

                if (!usuariosEncontrados.Any())
                {
                    return NotFound("Nenhum dos usuários informados foi encontrado.");
                }

                // Remove os usuários do banco
                _contexto.Usuario.RemoveRange(usuariosEncontrados);
                await _contexto.SaveChangesAsync();

                // Prepara a mensagem de retorno
                var mensagem = usuariosEncontrados.Count == 1
                    ? $"Usuário com ID {usuariosEncontrados.First().Id} foi excluído com sucesso."
                    : $"Usuários com IDs {string.Join(", ", usuariosEncontrados.Select(u => u.Id))} foram excluídos com sucesso.";

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
        }


    }



}