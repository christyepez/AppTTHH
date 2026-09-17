using AppTTHH.Domain;

namespace AppTTHH.Application;

public sealed class EmployeeService(IEmployeeRepository repository)
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct) =>
        (await repository.GetAllAsync(ct)).Select(Map).ToList();

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var employee = await repository.GetByIdAsync(id, ct);
        return employee is null ? null : Map(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct)
    {
        var employee = new Employee { EmployeeCode = request.EmployeeCode, FullName = request.FullName, Email = request.Email, Position = request.Position, Department = request.Department };
        await repository.AddAsync(employee, ct);
        return Map(employee);
    }

    private static EmployeeDto Map(Employee e) => new(e.Id, e.EmployeeCode, e.FullName, e.Email, e.Position, e.Department, e.IsActive);
}
