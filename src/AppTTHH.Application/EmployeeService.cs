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
        var employee = new Employee { EmployeeCode = request.EmployeeCode.Trim(), FullName = request.FullName.Trim(), Email = request.Email.Trim().ToLowerInvariant(), Position = request.Position.Trim(), Department = request.Department.Trim() };
        await repository.AddAsync(employee, ct);
        return Map(employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        var employee = await repository.GetByIdAsync(id, ct);
        if (employee is null) return null;

        employee.FullName = request.FullName.Trim();
        employee.Email = request.Email.Trim().ToLowerInvariant();
        employee.Position = request.Position.Trim();
        employee.Department = request.Department.Trim();
        await repository.UpdateAsync(employee, ct);
        return Map(employee);
    }

    public async Task<EmployeeDto?> SetActiveAsync(Guid id, bool active, CancellationToken ct)
    {
        var employee = await repository.GetByIdAsync(id, ct);
        if (employee is null) return null;

        employee.IsActive = active;
        await repository.UpdateAsync(employee, ct);
        return Map(employee);
    }

    private static EmployeeDto Map(Employee e) => new(e.Id, e.EmployeeCode, e.FullName, e.Email, e.Position, e.Department, e.IsActive);
}
