using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Presentation.Common;

[ProducesBaseResponse]
public class InternalControllerBase : Controller
{
    protected IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = 200)
    {
        if (result.IsSuccess)
            return StatusCode(successStatusCode, result.Value);

        return UnprocessableEntity(result.Notifications);
    }
}

public class CustomControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        var isCustomController = !typeInfo.IsAbstract && typeof(InternalControllerBase).IsAssignableFrom(typeInfo);
        return isCustomController || base.IsController(typeInfo);
    }
}

public static class InternalControllersExtension
{
    public static IMvcBuilder EnableInternalControllers(this IMvcBuilder builder)
    {
        builder.ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
        });
        return builder;
    }
}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ProducesBaseResponseAttribute : Attribute { }