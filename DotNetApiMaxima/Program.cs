using DotNetApiMaxima.Config;
using DotNetApiMaxima.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Abre o navegador
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Contexto>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();

app.MapPost("AdicionarProduto", async (Produto produto, Contexto contexto) =>
{
    if (produto == null)
    {
        return Results.BadRequest("Produto n�o pode ser nulo.");
    }

    if (string.IsNullOrEmpty(produto.Coddepto) || produto.Preco <= 0)
    {
        return Results.BadRequest("C�digo do departamento � obrigat�rio e o pre�o deve ser maior que zero.");
    }

    if (string.IsNullOrEmpty(produto.Status))
    {
        return Results.BadRequest("O Status do produto A - Ativo ou I - Inativo � obrigat�rio");
    }

    contexto.Produto.Add(produto);
    await contexto.SaveChangesAsync();

    return Results.Ok($"Produto com Codprod {Codprod} foi Cadastrado com sucesso.");
});

app.MapPost("ExcluirProduto/{Codprod}", async (string Codprod, Contexto contexto) =>
{
    var produto = await contexto.Produto.FirstOrDefaultAsync(p => p.Codprod == Codprod);

    if (produto != null)
    {
        // Atualiza o Codoperacao para 2 antes de excluir
        produto.Codoperacao = 2;
        contexto.Produto.Update(produto);
        await contexto.SaveChangesAsync();

        // Agora remove o produto
        contexto.Produto.Remove(produto);
        await contexto.SaveChangesAsync();

        return Results.Ok($"Produto com Codprod {Codprod} foi exclu�do com sucesso.");
    }

    return Results.NotFound($"Produto com Codprod {Codprod} n�o encontrado.");
});


app.UseSwaggerUI();

app.Run();
