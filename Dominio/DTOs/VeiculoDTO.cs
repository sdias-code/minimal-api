
namespace MininalApi.DTOs;
public record VeiculoDTO
{
    
    [StringLength(150)]
    public string  Nome { get; set; } = default!;

    [StringLength(100)]
    public string Marca { get; set; } = default!;

    public int Ano { get; set; } = default!;
}