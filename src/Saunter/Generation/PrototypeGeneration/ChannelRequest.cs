using System.Collections.Generic;

namespace Saunter.Generation.PrototypeGeneration;

public record ChannelRequest(
    string Name, 
    string Description,
    IEnumerable<ChannelParameterRequest> Parameters,
    IEnumerable<string> ServerNames, 
    OperationRequest? Publish, 
    OperationRequest? Subscribe,
    string? BindingsRef);