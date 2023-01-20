using Saunter.AsyncApiSchema.v2;

namespace Saunter.Generation.PrototypeGeneration;

public interface IChannelsGenerator
{
    void Generate(ChannelsRequest req, AsyncApiDocument asyncApiDocument);
}