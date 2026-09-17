using AppTTHH.Application;
using AppTTHH.Domain;

namespace AppTTHH.Tests;

public sealed class EmployeeServiceTests
{
    [Fact]
    public async Task CreateAsync_MapsAndPersistsEmployee()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);
        var result = await service.CreateAsync(new("E001", "Ada Lovelace", "ada@example.com", "Engineer", "Technology"), default);

        Assert.Equal("E001", result.EmployeeCode);
        Assert.Equal("Ada Lovelace", result.FullName);
        Assert.True(result.IsActive);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task GetAllAsync_MapsAllEmployees()
    {
        var repository = new FakeEmployeeRepository();
        repository.Items.Add(CreateEmployee("E001", "Ada Lovelace"));
        repository.Items.Add(CreateEmployee("E002", "Grace Hopper"));
        var service = new EmployeeService(repository);
        var result = await service.GetAllAsync(default);

        Assert.Equal(2, result.Count);
        Assert.Equal("Ada Lovelace", result[0].FullName);
        Assert.Equal("Grace Hopper", result[1].FullName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedEmployee_WhenFound()
    {
        var repository = new FakeEmployeeRepository();
        var employee = CreateEmployee("E003", "Katherine Johnson");
        repository.Items.Add(employee);
        var service = new EmployeeService(repository);

        var result = await service.GetByIdAsync(employee.Id, default);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.Equal("Katherine Johnson", result.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = new EmployeeService(new FakeEmployeeRepository());
        Assert.Null(await service.GetByIdAsync(Guid.NewGuid(), default));
    }

    private static Employee CreateEmployee(string code, string name) => new()
    {
        EmployeeCode = code,
        FullName = name,
        Email = $"{code.ToLowerInvariant()}@example.com",
        Position = "Engineer",
        Department = "Technology"
    };
}
