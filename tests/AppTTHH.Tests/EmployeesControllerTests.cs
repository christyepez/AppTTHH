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

    [Fact]
    public async Task Update_ReturnsOk_WhenEmployeeExists()
    {
        var repository = new FakeEmployeeRepository();
        var employee = CreateEmployee("E022", "Original");
        repository.Items.Add(employee);
        var controller = new EmployeesController(new EmployeeService(repository));

        var result = await controller.Update(employee.Id, new("Updated", "updated@example.com", "Manager", "People"), default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<EmployeeDto>(ok.Value);
        Assert.Equal("Updated", dto.FullName);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var controller = new EmployeesController(new EmployeeService(new FakeEmployeeRepository()));
        var result = await controller.Update(Guid.NewGuid(), new("Name", "mail@example.com", "Role", "Area"), default);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task ActivateAndDeactivate_ReturnUpdatedEmployee()
    {
        var repository = new FakeEmployeeRepository();
        var employee = CreateEmployee("E023", "Lifecycle");
        repository.Items.Add(employee);
        var controller = new EmployeesController(new EmployeeService(repository));

        var deactivated = await controller.Deactivate(employee.Id, default);
        var deactivatedDto = Assert.IsType<EmployeeDto>(Assert.IsType<OkObjectResult>(deactivated.Result).Value);
        Assert.False(deactivatedDto.IsActive);

        var activated = await controller.Activate(employee.Id, default);
        var activatedDto = Assert.IsType<EmployeeDto>(Assert.IsType<OkObjectResult>(activated.Result).Value);
        Assert.True(activatedDto.IsActive);
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
