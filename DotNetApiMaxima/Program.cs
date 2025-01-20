using DotNetApiMaxima.Config;
using DotNetApiMaxima.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Necessário para adicionar a configuração do Swagger
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Definir o esquema de autenticação para Bearer Token
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

// Adicionando a configuração do JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Validando se a chave JWT está configurada para evitar erros
        var jwtKey = builder.Configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("A chave 'Jwt:Key' não está configurada.");
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

// Configuração do DbContext (Oracle)
builder.Services.AddDbContext<Contexto>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Adicionando CORS
builder.Services.AddCors();

var app = builder.Build();

// Usando Swagger para documentação da API

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Definir onde a interface do Swagger será acessada
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Define a rota para o Swagger UI
});


// Usando middleware de exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configuração do CORS (antes de UseRouting)
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configuração do roteamento
app.UseRouting();

// Usando autenticação JWT
app.UseAuthentication();  // Adiciona autenticação
app.UseAuthorization();   // Adiciona autorização

app.MapControllers(); // Configura os controllers

app.Run();
