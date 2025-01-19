using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetApiMaxima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly Contexto _contexto;
        private readonly IConfiguration _configuration;

        public UsuarioController(Contexto contexto, IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(401, Type = typeof(string))]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // SQL para validar o login e senha
            var sql = @"
                SELECT idusuario, nome, status
                FROM MXSUSUARIOS
                WHERE login = :Login
                AND senha = :Senha
                AND status = 'A'
                AND ROWNUM = 1";

            var parameters = new List<OracleParameter>
            {
                new OracleParameter(":Login", loginRequest.Login),
                new OracleParameter(":Senha", loginRequest.Senha)
            };

            try
            {
                using (var connection = new OracleConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var usuario = new Usuario
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("idusuario")),
                                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                                    Status = reader.GetString(reader.GetOrdinal("status")),
                                    Login = loginRequest.Login,
                                    Senha = loginRequest.Senha
                                };

                                var token = GenerateJwtToken(usuario);
                                return Ok(new { Token = token });
                            }
                            else
                            {
                                return Unauthorized(new { Message = "Credenciais inválidas." });
                            }
                        }
                    }
                }
            }
            catch (OracleException)
            {
                return Unauthorized(new { Message = "Erro interno no servidor." });
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            // Verifica se a chave JWT está configurada
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, "User") // Adiciona um papel para o usuário
            };

            var expiration = DateTime.UtcNow.AddHours(10);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginRequest
        {
            public string Login { get; set; }
            public string Senha { get; set; }
        }
    


        /*########################################################################################################################################################
        *******************************************************   // GET: api/Usuario/ListarUsuariosTodos   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ListarUsuariosTodos")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ListarUsuariosTodos()
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

        /*########################################################################################################################################################
        *******************************************************   // GET: api/Usuario/ConsultarUsuarios   *******************************************************
        ########################################################################################################################################################*/

        [HttpGet("ConsultarUsuarios")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> ConsultarUsuarios([FromQuery] List<int> id)
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

        /*########################################################################################################################################################
        *******************************************************   // POST: api/Usuario/AdicionarUsuarios   *******************************************************
        ########################################################################################################################################################*/

        [HttpPost("AdicionarUsuarios")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<IActionResult> AdicionarUsuarios([FromBody] List<Usuario> usuarios)
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
                var parametros = new List<OracleParameter>();  // Adicionada a referência ao OracleParameter

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
    }
}
