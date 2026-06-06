
namespace ApiProjeto.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public int CargaHoraria { get; set; }
    public DateTime DataCricao { get; set; }

    public ICollection<Enrollment> Enrollments {get; set; } = [];

}