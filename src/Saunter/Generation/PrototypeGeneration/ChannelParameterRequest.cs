using System;

namespace Saunter.Generation.PrototypeGeneration;

public record ChannelParameterRequest(string Name, string Description, Type Type, string Location);