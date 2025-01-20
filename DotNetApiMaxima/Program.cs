using DotNetApiMaxima.Config;
using DotNetApiMaxima.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Necess�rio para adicionar a configura��o do Swagger
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configura��o de Servi�os
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Definir o esquema de autentica��o para Bearer Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Entre com o token JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers(); // Adiciona suporte a Controllers

// Adicionando a configura��o do JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Validando se a chave JWT est� configurada para evitar erros
        var jwtKey = builder.Configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("A chave 'Jwt:Key' n�o est� configurada.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configura��o do DbContext (Oracle)
builder.Services.AddDbContext<Contexto>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Adicionando CORS
builder.Services.AddCors();

var app = builder.Build();

// Usando Swagger para documenta��o da API

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Definir onde a interface do Swagger ser� acessada
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Define a rota para o Swagger UI
});


// Usando middleware de exce��es
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configura��o do CORS (antes de UseRouting)
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configura��o do roteamento
app.UseRouting();

// Usando autentica��o JWT
app.UseAuthentication();  // Adiciona autentica��o
app.UseAuthorization();   // Adiciona autoriza��o

app.MapControllers(); // Configura os controllers

app.Run();
