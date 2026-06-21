using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Presentation.Common.Api;

internal class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        var isController = !typeInfo.IsAbstract &&
            typeof(ControllerBase).IsAssignableFrom(typeInfo);

        return isController;
    }
}