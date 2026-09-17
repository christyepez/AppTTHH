using AppTTHH.Application;
using AppTTHH.Domain;

namespace AppTTHH.Tests;

public sealed class EmployeeServiceTests
{
    [Fact]
    public async Task CreateAsync_MapsAndPersistsEmployee()
    {
        var repository = new FakeRepository();
        var service = new EmployeeService(repository);
        var result = await service.CreateAsync(new("E001", "Ada Lovelace", "ada@example.com", "Engineer", "Technology"), default);
        Assert.Equal("E001", result.EmployeeCode);
        Assert.Single(repository.Items);
    }

    private sealed class FakeRepository : IEmployeeRepository
    {
        public List<Employee> Items { get; } = [];
        public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Employee>>(Items);
        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        public Task AddAsync(Employee employee, CancellationToken ct) { Items.Add(employee); return Task.CompletedTask; }
    }
}
