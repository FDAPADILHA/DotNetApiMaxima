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
        return Results.BadRequest("Produto não pode ser nulo.");
    }

    if (string.IsNullOrEmpty(produto.Coddepto) || produto.Preco <= 0)
    {
        return Results.BadRequest("Código do departamento é obrigatório e o preço deve ser maior que zero.");
    }

    if (string.IsNullOrEmpty(produto.Status))
    {
        return Results.BadRequest("O Status do produto A - Ativo ou I - Inativo é obrigatório.");
    }

    // Define os campos padrão como null
    produto.Codoperacao = null;
    produto.Id = null;

    // Adiciona o produto
    contexto.Produto.Add(produto);
    await contexto.SaveChangesAsync();

    // Retorna apenas os campos visíveis ao usuário
    var produtoRetorno = new
    {
        produto.Codprod,
        produto.Descricao,
        produto.Coddepto,
        produto.Preco,
        produto.Status
    };

    return Results.Ok(produtoRetorno);
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

        return Results.Ok($"Produto com Codprod {Codprod} foi excluído com sucesso.");
    }

    return Results.NotFound($"Produto com Codprod {Codprod} não encontrado.");
});

app.MapGet("ListarProdutos/", async (Contexto contexto) =>
{
    var lista = await contexto.Produto
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
        return Results.NotFound("Nenhum produto cadastrado.");
    }

    return Results.Ok(lista);
});


app.MapGet("ConsultarProduto/{Codprod}", async (string Codprod, Contexto contexto) =>
{
    var produto = await contexto.Produto
        .Where(p => p.Codprod == Codprod)
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
        return Results.Ok(produto);
    }

    return Results.NotFound($"Produto com Codprod {Codprod} não encontrado.");
});



app.UseSwaggerUI();

app.Run();
