using AppTTHH.Domain;

namespace AppTTHH.Application;

public sealed record EmployeeDto(Guid Id, string EmployeeCode, string FullName, string Email, string Position, string Department, bool IsActive);
public sealed record CreateEmployeeRequest(string EmployeeCode, string FullName, string Email, string Position, string Department);

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Employee employee, CancellationToken cancellationToken);
}
