using MininalApi.Dominio.Enuns;

namespace MininalApi.Dominio.ModelViews;

public record AdministradorLogadoModelViews
{    
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
    public string Token { get; set; } = default!;

}