using AppTTHH.Application;
using AppTTHH.Domain;

namespace AppTTHH.Tests;

internal sealed class FakeEmployeeRepository : IEmployeeRepository
{
    public List<Employee> Items { get; } = [];

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Employee>>(Items);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task AddAsync(Employee employee, CancellationToken ct)
    {
        Items.Add(employee);
        return Task.CompletedTask;
    }
}
