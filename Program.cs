using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Interfaces;
using MininalApi.Dominio.Servicos;
using MininalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();

builder.Services.AddEndpointsApiExplerer();
builder.Services.AddSwagerGen();

var pbuilder = builder.Configuration.GetConnectionString("conexaoMysql");

builder.Services.AddDbContext<DbContexto>( options => {
    options.UseMySql(
        pbuilder,
        ServerVersion.AutoDetect(pbuilder)
    );
}

);

var app = builder.Build();

app.MapGet("/", () => "Olá pessoal!");

app.MapPost("/login", ([FromBody] MininalApi.DTOs.LoginDTO loginDTO, IAdministradorServico adminstradorServico) => {
    if(adminstradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

