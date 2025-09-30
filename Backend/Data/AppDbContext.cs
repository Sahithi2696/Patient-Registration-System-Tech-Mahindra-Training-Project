using Microsoft.EntityFrameworkCore;
using PatientReg.Api.Models;

namespace PatientReg.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(e =>
        {
            e.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            e.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            e.Property(p => p.Dob).IsRequired();
            e.Property(p => p.Gender).HasMaxLength(20).IsRequired();
            e.Property(p => p.Phone).HasMaxLength(30).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasIndex(p => p.LastName);
            e.HasIndex(p => p.Dob);
        });
    }
}
