using Saunter.AsyncApiSchema.v2;

namespace Saunter
{
    public interface IAsyncApiDocumentProvider
    {
        AsyncApiDocument GetDocument(AsyncApiOptions options, AsyncApiSchemaOptions schemaOptions, AsyncApiDocument prototype);
    }
}