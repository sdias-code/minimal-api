using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Interfaces;
using MininalApi.Dominio.ModelViews;
using MininalApi.Dominio.Servicos;
using MininalApi.DTOs;
using MininalApi.Infraestrutura.Db;

#region builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var pbuilder = builder.Configuration.GetConnectionString("conexaoMysql");

builder.Services.AddDbContext<DbContexto>( options => {
    options.UseMySql(
        pbuilder,
        ServerVersion.AutoDetect(pbuilder)
    );
}

);

var app = builder.Build();

#endregion

#region Home

app.MapGet("/", () => Results
    .Json(new Home()))
    .WithTags("Home");

#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] MininalApi.DTOs.LoginDTO loginDTO, IAdministradorServico adminstradorServico) => {
    if(adminstradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

#endregion

#region Veículos

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
    
    var veiculo = new Veiculo{
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos",(int pagina, IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos((int)pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculos");

#endregion

#region App

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

#endregion