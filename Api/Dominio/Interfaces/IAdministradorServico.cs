using MininalApi.Dominio.Entidades;
using MininalApi.DTOs;

namespace MininalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
        public Administrador? Login(LoginDTO loginDTO);
        public Administrador Incluir(Administrador administrador);
        public List<Administrador> Todos(int? pagina);
        public Administrador? BuscaPorId(int id);
}