using System.Collections.Generic;

namespace Saunter.Generation.PrototypeGeneration;

public record OperationRequest(string Id, string Summary, string Description, IEnumerable<MessageRequest> Messages,
    string? BindingsRef, string[]? Tags);