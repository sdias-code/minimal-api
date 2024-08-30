
using System.ComponentModel.DataAnnotations;

namespace MininalApi.DTOs;
public record VeiculoDTO
{
    
    [StringLength(150)]
    [Required]
    public string  Nome { get; set; } = default!;

    [StringLength(100)]
    [Required]
    public string Marca { get; set; } = default!;

    [Required]
    public int Ano { get; set; } = default!;
}