using AppTTHH.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppTTHH.Api.Controllers;

[ApiController]
[Route("api/hr/employees")]
[Authorize]
public sealed class EmployeesController(EmployeeService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<EmployeeDto>> GetAll(CancellationToken ct) => service.GetAllAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id, CancellationToken ct)
    {
        var employee = await service.GetByIdAsync(id, ct);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
