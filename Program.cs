using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Interfaces;
using MininalApi.Dominio.Servicos;
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

app.MapGet("/", () => Results.Json(
    new Home()
));

#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] MininalApi.DTOs.LoginDTO loginDTO, IAdministradorServico adminstradorServico) => {
    if(adminstradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();
});

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
});

#endregion

#region App

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

#endregion