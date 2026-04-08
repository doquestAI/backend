using System;

// Domain/ValueObjects/SearchResult.cs

namespace Domain.ValueObjects;

public record SearchResult(
    string Content,
    Dictionary<string, object> Metadata,
    float Score);