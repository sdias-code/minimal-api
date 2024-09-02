using Microsoft.EntityFrameworkCore;
using MininalApi.Dominio.Entidades;

namespace MininalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configurationAppSettings;

    public DbContexto(IConfiguration configurationAppSettings){
        _configurationAppSettings = configurationAppSettings;
    }

    public DbContexto(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Administrador> Administradores {get; set;} = default!;

    public DbSet<Veiculo> Veiculos {get; set;} = default!;

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Administrador>().HasData(
    //         new Administrador{
    //             Id = 1,
    //             Email = "administrador@teste.com",
    //             Senha = "123456",
    //             Perfil = "Adm"
    //         }
    //     );
    // }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured){

            var stringConexao = _configurationAppSettings.GetConnectionString("conexaoMysql")?.ToString();

            if(!string.IsNullOrEmpty(stringConexao)){
                optionsBuilder.UseMySql(
                stringConexao,
                ServerVersion.AutoDetect(stringConexao));
            }        
        }
        
        
    }
}