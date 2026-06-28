using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Presentation.Common;

[ProducesBaseResponse]
internal class InternalControllerBase : Controller;

internal class CustomControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        var isCustomController = !typeInfo.IsAbstract && typeof(InternalControllerBase).IsAssignableFrom(typeInfo);
        return isCustomController || base.IsController(typeInfo);
    }
}

internal static class InternalControllersExtension
{
    internal static IMvcBuilder EnableInternalControllers(this IMvcBuilder builder)
    {
        builder.ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
        });
        return builder;
    }
}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal class ProducesBaseResponseAttribute : Attribute { }