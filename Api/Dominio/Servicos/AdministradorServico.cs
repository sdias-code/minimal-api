using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Interfaces;
using MininalApi.DTOs;
using MininalApi.Infraestrutura.Db;

namespace MininalApi.Dominio.Servicos;

public class AdminstradorServico : IAdministradorServico
{

    private readonly DbContexto _contexto;

    public AdminstradorServico(DbContexto contexto){
        _contexto = contexto;
    }    

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores
        .FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);

        return adm; 
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public List<Administrador> Todos(
        int? pagina){
            
            var query = _contexto.Administradores.AsQueryable();            

            int itensPorPagina = 10;

            if(pagina != null){
                query = query
                        .Skip(((int) pagina -1) * itensPorPagina)
                        .Take(itensPorPagina);
            }            
                        
            return [.. query];
        }

    public Administrador? BuscaPorId(int id){
        return _contexto.Administradores.FirstOrDefault(a => a.Id == id);
    }
}