using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Entidades;
using MyTest.Dominio.Db;

public class AdminTests
{
    private readonly MyDbContext _context;

    public AdminTests()
    {
        string conectionString = "Server=localhost;Port=3306;Database=mybd_minimal_api_test;Uid=root;Pwd=teste@123;";

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseMySql(
                conectionString, 
                new MySqlServerVersion(new Version(8, 0, 21)))
                .Options;

        _context = new MyDbContext(options);
    }

    [Fact]
    public void TesteAdicionarAdministrador()
    {
        var admin = new Administrador { Email = "teste@exemplo.com", Senha = "teste@123", Perfil = "Editor", };
        _context.Administradores.Add(admin);
        _context.SaveChanges();

        var adminDoBanco = _context.Administradores.FirstOrDefault(a => a.Email == "teste@exemplo.com");
        Assert.NotNull(adminDoBanco);
        Assert.Equal("teste@exemplo.com", adminDoBanco.Email);
        Assert.Equal("teste@123", adminDoBanco.Senha);
        Assert.Equal("Editor", adminDoBanco.Perfil);

    }
}
