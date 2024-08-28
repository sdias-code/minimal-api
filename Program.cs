using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Enuns;
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
app.MapPost("/administradores/login", (
    [FromBody] LoginDTO loginDTO, 
    IAdministradorServico adminstradorServico) => {
    if(adminstradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();

}).WithTags("Administradores");

app.MapGet("/administradores", (
    [FromQuery] int? pagina, 
    IAdministradorServico adminstradorServico) => {

        var listAdms = new List<AdministradorModelViews>();

        var administradores = adminstradorServico.Todos(pagina);

        foreach(var adm in administradores){
            listAdms.Add(new AdministradorModelViews{
                Id = adm.Id,
                Email = adm.Email,
                Perfil = adm.Perfil
            });
        }
    
        return Results.Ok(listAdms);

}).WithTags("administradores");

app.MapGet("/administradores/{id}",(
    [FromRoute] int id, 
    IAdministradorServico adminstradorServico) => {
        
    var administrador = adminstradorServico.BuscaPorId(id);

    if (administrador != null){
        var admView = new AdministradorModelViews{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
            };

        return Results.Ok(admView);
    }    
    else{
        return Results.NotFound();
    } 

}).WithTags("Administradores");

app.MapPost("/administradores", (
    [FromBody] AdministradorDTO administradorDTO, 
    IAdministradorServico adminstradorServico) => {

        var validacao = new ErrosDeValidacoes{
            Mensagens = new List<string>()
        };

        if(string.IsNullOrEmpty(administradorDTO.Email)){
            validacao.Mensagens.Add("Email não pode ser vazio");
        }

        if(string.IsNullOrEmpty(administradorDTO.Senha)){
            validacao.Mensagens.Add("A senha deve ser digitada");
        }

        if(administradorDTO.Perfil == null){
            validacao.Mensagens.Add("O perfil deve ser incluído");
        }

        if(validacao.Mensagens.Count > 0){
            return Results.BadRequest(validacao);
        }

        var veiculo = new Administrador{
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };

        var admView = new AdministradorModelViews{
            Id = veiculo.Id,
            Email = veiculo.Email,
            Perfil = veiculo.Perfil
            };

        adminstradorServico.Incluir(veiculo);

        return Results.Created($"/administrador/{admView.Id}", admView);

}).WithTags("Administradores");

#endregion

#region Veículos

static ErrosDeValidacoes validarVeiculoDTO( VeiculoDTO veiculoDTO){
     var validacao = new ErrosDeValidacoes{
        Mensagens = new List<string>()
     };

        if(string.IsNullOrEmpty(veiculoDTO.Nome)){
            validacao.Mensagens.Add("O campo Nome deve ser preenchido");
        }

        if(string.IsNullOrEmpty(veiculoDTO.Marca)){
            validacao.Mensagens.Add("O campo Marca deve ser preenchido");
        }

        if(veiculoDTO.Ano < 1900){
            validacao.Mensagens.Add("O campo Ano deve ser preenchido com um valor válido");
        }
        
        return validacao;
}

app.MapPost("/veiculos", (
    [FromBody] VeiculoDTO veiculoDTO, 
    IVeiculoServico veiculoServico) => {

        var validacao = validarVeiculoDTO(veiculoDTO);        

        if(validacao.Mensagens.Count != 0)
        {
            return Results.BadRequest(validacao.Mensagens);
        }
    
        var veiculo = new Veiculo{
                Nome = veiculoDTO.Nome,
                Marca = veiculoDTO.Marca,
                Ano = veiculoDTO.Ano
            };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).WithTags("Veiculos");

app.MapGet("/veiculos",(
    [FromQuery] int? pagina, 
    IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);

}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}",(
    [FromRoute] int id, 
    IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null){
        return Results.NotFound();
    }

    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}",(
    [FromRoute] int id, 
    VeiculoDTO veiculoDTO,
    IVeiculoServico veiculoServico) => {

         var validacao = validarVeiculoDTO(veiculoDTO);        

        if(validacao.Mensagens.Count != 0)
        {
            return Results.BadRequest(validacao.Mensagens);
        }

        var veiculo = veiculoServico.BuscaPorId(id);

        if(veiculo == null){
            return Results.NotFound();
        }

        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculoServico.Atualizar(veiculo);

        return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}",(
    [FromRoute] int id,     
    IVeiculoServico veiculoServico) => {

        var veiculo = veiculoServico.BuscaPorId(id);

        if(veiculo == null){
            return Results.NotFound();
        }    

        veiculoServico.Apagar(veiculo);

        return Results.NoContent();

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