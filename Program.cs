using Microsoft.EntityFrameworkCore;
using MininalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

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

app.MapPost("/login", (MininalApi.DTOs.LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();
});

app.Run();

