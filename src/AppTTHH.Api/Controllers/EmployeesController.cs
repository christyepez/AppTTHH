using AppTTHH.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppTTHH.Api.Controllers;

[ApiController]
[Route("api/hr/employees")]
public sealed class EmployeesController(EmployeeService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = HrPermissions.EmployeesView)]
    public Task<IReadOnlyList<EmployeeDto>> GetAll(CancellationToken ct) => service.GetAllAsync(ct);

    [HttpGet("{id:guid}")]
    [Authorize(Policy = HrPermissions.EmployeesView)]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id, CancellationToken ct)
    {
        var employee = await service.GetByIdAsync(id, ct);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [Authorize(Policy = HrPermissions.EmployeesManage)]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = HrPermissions.EmployeesManage)]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = HrPermissions.EmployeesManage)]
    public async Task<ActionResult<EmployeeDto>> Activate(Guid id, CancellationToken ct)
    {
        var updated = await service.SetActiveAsync(id, true, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = HrPermissions.EmployeesManage)]
    public async Task<ActionResult<EmployeeDto>> Deactivate(Guid id, CancellationToken ct)
    {
        var updated = await service.SetActiveAsync(id, false, ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}
