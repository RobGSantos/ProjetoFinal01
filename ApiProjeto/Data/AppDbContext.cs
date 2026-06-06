
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApiProjeto.Domain.Entities;

namespace ApiProjeto.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            
        });
        modelBuilder.Entity<Student>(b =>
        {
                        
            b.HasKey(u => u.Id);

            b.Property(u => u.NomeCompleto)
                .IsRequired();
            
            b.Property(u => u.Email)
                .IsRequired();

            b.Property(u => u.DataCadatro)
                .IsRequired();

            b.Property(u => u.ApplicationUserId)
                .IsRequired();
            
            b.HasOne(u => u.ApplicationUser)
                .WithOne()
                .HasForeignKey<Student>(u => u.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.Property(u => u.IsDeleted)
                .IsRequired();

            b.HasMany(u => u.Enrollments)
                .WithOne(r => r.Student)
                .HasForeignKey(r => r.StudentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(u => u.ApplicationUserId)
                .HasDatabaseName("IX_Student_ApplicationUserId")
                .IsUnique();

            b.HasIndex(u => u.Email)
                .HasDatabaseName("IX_Student_Email")
                .IsUnique();
        });
        modelBuilder.Entity<Course>(b =>
        {
            b.HasKey(u => u.Id);

            b.Property(u => u.Titulo)
                .IsRequired();

            b.Property(u => u.Descricao)
                .IsRequired();
            
            b.Property(u => u.Categoria)
                .IsRequired();
            
            b.Property(u => u.CargaHoraria);
            
            b.Property(u => u.DataCricao);

            b.Property(u => u.IsDeleted)
                .IsRequired();
            
            b.HasMany(u => u.Enrollments)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(u => u.Categoria)
                .HasDatabaseName("IX_Courses_Categoria");

        });
        modelBuilder.Entity<Enrollment>(b =>
        {
            b.HasKey(u => u.Id);

            b.Property(u => u.StudentId);
            
            b.Property(u => u.CourseId);
            
            b.Property(u => u.Status)
                .IsRequired();
            
            b.Property(u => u.DataMatricula)
                .IsRequired();

            b.HasIndex(u => new {u.StudentId, u.CourseId})
                .HasDatabaseName("IX_Enrollment_Student_Course")
                .IsUnique();
        });
    }
}