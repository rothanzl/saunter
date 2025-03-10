using System;
using Saunter.Generation.SchemaGeneration;

namespace Saunter.Generation.PrototypeGeneration;

public record MessageRequest(string Id, Type PayloadType, Type? HeadersType, IExamplesProvider ExamplesProvider,
    string Title, string Summary, string Description, string? BindingsRef, string[]? Tags);