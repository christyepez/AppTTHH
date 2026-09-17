using AppTTHH.Api.Controllers;
using AppTTHH.Application;
using Microsoft.AspNetCore.Authorization;

namespace AppTTHH.Tests;

public sealed class EmployeeAuthorizationTests
{
    [Theory]
    [InlineData(nameof(EmployeesController.GetAll), HrPermissions.EmployeesView)]
    [InlineData(nameof(EmployeesController.GetById), HrPermissions.EmployeesView)]
    [InlineData(nameof(EmployeesController.Create), HrPermissions.EmployeesManage)]
    public void Endpoints_require_expected_permission(string methodName, string expectedPolicy)
    {
        var method = typeof(EmployeesController).GetMethod(methodName);
        var authorize = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .SingleOrDefault();

        Assert.NotNull(authorize);
        Assert.Equal(expectedPolicy, authorize.Policy);
    }

    [Fact]
    public void Hr_permission_names_are_unique()
    {
        var permissions = new[] { HrPermissions.EmployeesView, HrPermissions.EmployeesManage };
        Assert.Equal(permissions.Length, permissions.Distinct(StringComparer.Ordinal).Count());
    }
}
