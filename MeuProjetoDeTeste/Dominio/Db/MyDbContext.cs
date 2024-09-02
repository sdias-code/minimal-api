using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Entidades;

namespace MyTest.Dominio.Db;
public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

}