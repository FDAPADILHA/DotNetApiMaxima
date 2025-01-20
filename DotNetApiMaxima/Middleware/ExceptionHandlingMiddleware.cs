using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;

namespace DotNetApiMaxima.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Token expirado. Por favor, faça login novamente.",
                    Details = "O token JWT fornecido expirou."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (SecurityTokenException)
            {
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Token inválido.",
                    Details = "O token JWT fornecido é inválido."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Erro interno no servidor.",
                    Details = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
