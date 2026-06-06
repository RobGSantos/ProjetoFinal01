
namespace ApiProjeto.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DataCadatro { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public ICollection<Enrollment> Enrollments {get; set; } = [];

}