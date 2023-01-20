using System;
using System.Reflection;
using Saunter.AsyncApiSchema.v2;

namespace Saunter.Generation
{
    public interface IDocumentGenerator
    {
        AsyncApiDocument GenerateDocument(TypeInfo[] asyncApiTypes, AsyncApiOptions options, AsyncApiSchemaOptions schemaOptions, AsyncApiDocument prototype, IServiceProvider serviceProvider);
    }
}