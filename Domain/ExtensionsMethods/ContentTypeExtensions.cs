using Domain.Enums;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Domain.ExtensionsMethods;

internal static class ContentTypeExtensions
{
    internal static string GetMimeType(this ContentType contentType)
    {
        if (contentType == null)
        {
            return "application/octet-stream";
        }

        return contentType
            .GetType()
            .GetMember(contentType.ToString())[0]
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? contentType.ToString();
    }

    internal static string GetMimeType(this ContentType? contentType)
    {
        if (contentType == null)
        {
            return "application/octet-stream";
        }

        return contentType
            .GetType()
            .GetMember(contentType.ToString())[0]
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? contentType.ToString();
    }

    internal static ContentType? ParseMimeType(string mimeType)
    {
        foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
        {
            var description = value.GetMimeType();
            if (string.Equals(description, mimeType, StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }
        return null;
    }

    internal static ContentType GetContentType(this string mimeType)
    {
        return ParseMimeType(mimeType) ?? ContentType.Unknown;
    }
}