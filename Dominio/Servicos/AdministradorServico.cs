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
}