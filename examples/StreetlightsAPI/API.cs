using Saunter.AsyncApiSchema.v2;
using Saunter.Generation.SchemaGeneration;

namespace StreetlightsAPI
{
    public class KafkaApi
    {
        public const string Title = "Inventory Async API";
        public const string Version = "1.0.0";
        public const string Description = "The  asynchronous API";
    }

    public class KafkaServer
    {
        public const string Name = "Kafka";
        public const string Url = "tbd.url.to.kafka.cluster";
        public const string ProtocolName = "kafka-secure";
    }

    public class InventoryApi
    {
        public const string Description = "The Inventory asynchronous API allows you to handle inventory";
        
        public const string Tag = "Inventory";
        public const string ExternalDocs = "https://example.com/";
        public const string ExternalDocsDescription = "TBD";
    }
    public class InventoryApi2
    {
        public const string Description = "The Inventory asynchronous API allows you to handle inventory";
        
        public const string Tag = "Inventory2";
        public const string ExternalDocs = "https://example.com/";
        public const string ExternalDocsDescription = "TBD";
    }

    public class RequestTopic
    {
        public const string Name = "request.topic.name";
        public const string Description = "Request topic";

        public class PublishOperation
        {
            public const string Id = "Requests";
            public const string Summary = "Operation summary";
            public const string Description = "Operation description";
        }
    }
    public class RequestTopic2
    {
        public const string Name = "request2.topic.name";
        public const string Description = "Request2 topic";

        public class PublishOperation
        {
            public const string Id = "Requests2";
            public const string Summary = "Operation summary";
            public const string Description = "Operation description";
        }
    }

    public class NewInventoryRequest
    {
        public const string Id = nameof(NewInventoryRequest);

        public const string Title = "message title";
        public const string Summary = "message summary";
        public const string Description = "message description";
        
        
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        
    }

    public record KafkaMessageHeader(string Version);
    
    
    public class NewInventoryRequestExample : IExamplesProvider
    {
        public MessageExample[] GetExamples()
        {
            return new MessageExample[]
            {
                new()
                {
                    Name = "this is name",
                    Summary = "this is summary",
                    Payload = new NewInventoryRequest
                    {
                        Property1 = "test string #1",
                        Property2 = "test string #2",
                    },
                    Headers = new KafkaMessageHeader("x.y.z")
                },
                new()
                {
                    Name = "the second example",
                    Summary = "summary of the second example",
                    Payload = new NewInventoryRequest()
                    {
                        Property1 = "111",
                        Property2 = "222"
                    },
                    Headers = new KafkaMessageHeader("1.2.3")
                }
            };
        }
    }
}
