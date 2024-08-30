
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Servicos;
using MininalApi.Infraestrutura.Db;
using System.Reflection;

namespace TestApi.Domain.Servicos;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("conexaoMysql");

        //var options = new DbContextOptionsBuilder<DbContexto>()
        //    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        //    .Options;

        //return new DbContexto((IConfiguration)options);

        return new DbContexto(configuration);

    }


    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TALBE Adminstradores");
        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "senha@123";
        adm.Perfil = "Adm";
        var administradorServico = new AdminstradorServico(context);

        // Act
        administradorServico.Incluir(adm);
        administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count()); 

    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TALBE Adminstradores");

        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "senha@123";
        adm.Perfil = "Adm";

        var administradorServico = new AdminstradorServico(context);

        // Act
        var admBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admBanco?.Id);

    }

}
