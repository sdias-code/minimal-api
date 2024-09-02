
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
        //var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        //var builder = new ConfigurationBuilder()
        //    .SetBasePath(path ?? Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //    .AddEnvironmentVariables();

        //var configuration = builder.Build();

        //var connectionString = configuration.GetConnectionString("conexaoMysql");
        //var options = new DbContextOptionsBuilder<DbContexto>()
        //    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        //    .Options;
        //return new DbContexto((IConfiguration)options);

        //var retornoContexto = new DbContexto(configuration);

        //return retornoContexto;

        string conectionString = "Server=localhost;Port=3306;Database=mybd_minimal_api_test;Uid=root;Pwd=teste@2024;";

        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseMySql(
                conectionString,
                new MySqlServerVersion(new Version(8, 0, 21)))
                .Options;

        return new DbContexto(options);

    }


    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();      
            
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "senha@12345",
            Perfil = "Adm"
        };

        var administradorServico = new AdminstradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        var admBd = administradorServico.BuscaPorId(1);

        // Assert
        Assert.AreEqual(1, admBd?.Id); 

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
