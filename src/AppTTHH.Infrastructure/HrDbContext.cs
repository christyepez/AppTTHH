using AppTTHH.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppTTHH.Infrastructure;

public sealed class HrDbContext(DbContextOptions<HrDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var employee = modelBuilder.Entity<Employee>();
        employee.ToTable("Employees");
        employee.HasKey(x => x.Id);
        employee.HasIndex(x => x.EmployeeCode).IsUnique();
        employee.HasIndex(x => x.Email).IsUnique();
        employee.Property(x => x.EmployeeCode).HasMaxLength(40).IsRequired();
        employee.Property(x => x.FullName).HasMaxLength(180).IsRequired();
        employee.Property(x => x.Email).HasMaxLength(180).IsRequired();
        employee.Property(x => x.Position).HasMaxLength(120).IsRequired();
        employee.Property(x => x.Department).HasMaxLength(120).IsRequired();
    }
}
