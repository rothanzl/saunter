using Saunter.AsyncApiSchema.v2;

namespace Saunter.Generation.SchemaGeneration;

public interface IExamplesProvider
{
    MessageExample[] GetExamples();
}