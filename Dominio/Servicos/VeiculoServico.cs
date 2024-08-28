using MininalApi.Dominio.Entidades;
using MininalApi.Dominio.Interfaces;
using MininalApi.DTOs;
using MininalApi.Infraestrutura.Db;

namespace MininalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{

    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto){
        _contexto = contexto;
    }   

    public List<Veiculo> Todos(
        int? pagina = 1,
        string? nome = null,
        string? marca = null){
            
            var query = _contexto.Veiculo.AsQueryble();

            if(!string.IsNullOrEmpty(nome)){
                query = query.where(v => v.Nome.ToLower().Contains(nome));
            }

            int itensPorPagina = 10;

            if(itensPorPagina != null){
                query = query
                        .Skip(((int) pagina -1) * itensPorPagina)
                        .Take(itensPorPagina);
            }            
                        
            return query.ToList();
        }

    public Veiculo? BuscaPorId(int id){
        return _contexto.Veiculo.FirstOrDefault(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo){
        _contexto.Veiculo.Add(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo){
        _contexto.Veiculo.Update(veiculo);
        _contexto.SaveChanges();
    }

     public void Apagar(Veiculo veiculo){
        _contexto.Veiculo.Remove(veiculo);
        _contexto.SaveChanges();
    }
   
}