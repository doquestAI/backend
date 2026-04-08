using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Reflection;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;

namespace Presentation.Common.Converters;

internal class IgnoreEmptyEnumerablesContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var prop = base.CreateProperty(member, serialization);

        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
        {
            prop.ShouldSerialize = instance =>
            {
                var collection = prop.ValueProvider.GetValue(instance) as IEnumerable;
                return collection != null && collection.GetEnumerator().MoveNext();
            };
        }
        return prop;
    }
}