using AppTTHH.Domain;
using AppTTHH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppTTHH.Tests;

public sealed class EmployeeRepositoryTests
{
    [Fact]
    public async Task AddAndGetByIdAsync_PersistsEmployee()
    {
        await using var db = CreateDb();
        var repository = new EmployeeRepository(db);
        var employee = CreateEmployee("E010", "Linus Torvalds");

        await repository.AddAsync(employee, default);
        var result = await repository.GetByIdAsync(employee.Id, default);

        Assert.NotNull(result);
        Assert.Equal("E010", result.EmployeeCode);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByFullName()
    {
        await using var db = CreateDb();
        var repository = new EmployeeRepository(db);
        await repository.AddAsync(CreateEmployee("E011", "Zoe Developer"), default);
        await repository.AddAsync(CreateEmployee("E012", "Ada Architect"), default);

        var result = await repository.GetAllAsync(default);

        Assert.Equal(2, result.Count);
        Assert.Equal("Ada Architect", result[0].FullName);
        Assert.Equal("Zoe Developer", result[1].FullName);
    }

    private static HrDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<HrDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new HrDbContext(options);
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
