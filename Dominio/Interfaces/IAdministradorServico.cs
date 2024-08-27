using MininalApi.Dominio.Entidades;
using MininalApi.DTOs;

namespace MininalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
        public Administrador? Login(LoginDTO loginDTO);
}