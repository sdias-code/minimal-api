using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Enuns;
using MininalApi.Dominio.Interfaces;
using MininalApi.Dominio.ModelViews;
using MininalApi.Dominio.Servicos;
using MininalApi.DTOs;
using MininalApi.Infraestrutura.Db;

#region builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();

if (string.IsNullOrEmpty(key)) key = "Chave_temporaria_token_de_acesso";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(key)
        ),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
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

var pbuilder = builder.Configuration.GetConnectionString("conexaoMysql");

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        pbuilder,
        ServerVersion.AutoDetect(pbuilder)
    );
});

var app = builder.Build();

#endregion

#region Home

app.MapGet("/", () => Results
    .Json(new Home()))
    .AllowAnonymous()
    .WithTags("Home");

#endregion

#region Administradores

app.MapPost("/administradores/login", (
    [FromBody] LoginDTO loginDTO,
    IAdministradorServico adminstradorServico) =>
{
    var adm = adminstradorServico.Login(loginDTO);
    if (adm != null)
    {
        var token = GerarTokenJwt(adm);

        return Results.Ok(new AdministradorLogadoModelViews
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();

})
.AllowAnonymous()
.WithTags("Administradores");

app.MapGet("/administradores", (
    [FromQuery] int? pagina,
    IAdministradorServico adminstradorServico) =>
{
    var listAdms = new List<AdministradorModelViews>();
    var administradores = adminstradorServico.Todos(pagina);

    foreach (var adm in administradores)
    {
        listAdms.Add(new AdministradorModelViews
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(listAdms);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("administradores");

app.MapGet("/administradores/{id}", (
    [FromRoute] int id,
    IAdministradorServico adminstradorServico) =>
{
    var administrador = adminstradorServico.BuscaPorId(id);

    if (administrador != null)
    {
        var admView = new AdministradorModelViews
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        };

        return Results.Ok(admView);
    }
    else
    {
        return Results.NotFound();
    }

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapPost("/administradores", (
    [FromBody] AdministradorDTO administradorDTO,
    IAdministradorServico adminstradorServico) =>
{
    var validacao = new ErrosDeValidacoes
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
    {
        validacao.Mensagens.Add("Email não pode ser vazio");
    }

    if (string.IsNullOrEmpty(administradorDTO.Senha))
    {
        validacao.Mensagens.Add("A senha deve ser digitada");
    }

    if (administradorDTO.Perfil == null)
    {
        validacao.Mensagens.Add("O perfil deve ser incluído");
    }

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    var admView = new AdministradorModelViews
    {
        Id = veiculo.Id,
        Email = veiculo.Email,
        Perfil = veiculo.Perfil
    };

    adminstradorServico.Incluir(veiculo);

    return Results.Created($"/administrador/{admView.Id}", admView);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

#endregion

#region Veículos

app.MapPost("/veiculos", (
    [FromBody] VeiculoDTO veiculoDTO,
    IVeiculoServico veiculoServico) =>
{

    var validacao = validarVeiculoDTO(veiculoDTO);

    if (validacao.Mensagens.Count != 0)
    {
        return Results.BadRequest(validacao.Mensagens);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Veiculos");

app.MapGet("/veiculos", (
    [FromQuery] int? pagina,
    IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);

})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", (
    [FromRoute] int id,
    IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}", (
    [FromRoute] int id,
    VeiculoDTO veiculoDTO,
    IVeiculoServico veiculoServico) =>
{

    var validacao = validarVeiculoDTO(veiculoDTO);

    if (validacao.Mensagens.Count != 0)
    {
        return Results.BadRequest(validacao.Mensagens);
    }

    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", (
    [FromRoute] int id,
    IVeiculoServico veiculoServico) =>
{

    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veiculos");
#endregion

#region App

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

#endregion

#region Validações
static ErrosDeValidacoes validarVeiculoDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacoes
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O campo Nome deve ser preenchido");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("O campo Marca deve ser preenchido");
    }

    if (veiculoDTO.Ano < 1900)
    {
        validacao.Mensagens.Add("O campo Ano deve ser preenchido com um valor válido");
    }

    return validacao;
}

#endregion

#region Métodos
string GerarTokenJwt(Administrador administrador)
{

    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(key));

    var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>(){
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: credential
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

#endregion
