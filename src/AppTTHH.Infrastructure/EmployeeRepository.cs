using AppTTHH.Application;
using AppTTHH.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppTTHH.Infrastructure;

public sealed class EmployeeRepository(HrDbContext db) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct) =>
        await db.Employees.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(ct);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Employee employee, CancellationToken ct)
    {
        db.Employees.Add(employee);
        await db.SaveChangesAsync(ct);
    }
}
