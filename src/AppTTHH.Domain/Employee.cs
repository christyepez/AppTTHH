namespace AppTTHH.Domain;

public sealed class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string EmployeeCode { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Position { get; set; }
    public required string Department { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
