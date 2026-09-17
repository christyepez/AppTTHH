using AppTTHH.Api.Controllers;
using AppTTHH.Application;
using AppTTHH.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AppTTHH.Tests;

public sealed class EmployeesControllerTests
{
    [Fact]
    public async Task GetById_ReturnsOk_WhenEmployeeExists()
    {
        var repository = new FakeEmployeeRepository();
        var employee = CreateEmployee("E020", "Margaret Hamilton");
        repository.Items.Add(employee);
        var controller = new EmployeesController(new EmployeeService(repository));

        var result = await controller.GetById(employee.Id, default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<EmployeeDto>(ok.Value);
        Assert.Equal(employee.Id, dto.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var controller = new EmployeesController(new EmployeeService(new FakeEmployeeRepository()));
        var result = await controller.GetById(Guid.NewGuid(), default);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var repository = new FakeEmployeeRepository();
        var controller = new EmployeesController(new EmployeeService(repository));
        var request = new CreateEmployeeRequest("E021", "Barbara Liskov", "barbara@example.com", "Architect", "Technology");

        var result = await controller.Create(request, default);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<EmployeeDto>(created.Value);
        Assert.Equal("E021", dto.EmployeeCode);
        Assert.Single(repository.Items);
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
