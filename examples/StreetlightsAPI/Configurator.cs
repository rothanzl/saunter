using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.Generation.PrototypeGeneration;

namespace StreetlightsAPI;

public class Configurator
{
     private readonly IServiceCollection _services;

     public Configurator(IServiceCollection services)
     {
          _services = services;
     }


     public void Configure(AsyncApiOptions opt)
     {
          IChannelsGenerator channelsGenerator = _services.BuildServiceProvider().GetService<IChannelsGenerator>();
          
          if (channelsGenerator is null)
               throw new NullReferenceException($"{nameof(IChannelsGenerator)} is null");


          var inventoryApiTag = new Tag(InventoryApi.Tag)
          {
               Description = InventoryApi.Description,
               ExternalDocs = new(InventoryApi.ExternalDocs)
               {
                    Description = InventoryApi.ExternalDocsDescription
               }
          };

          var inventoryApiTag2 = new Tag(InventoryApi2.Tag)
          {
               Description = InventoryApi2.Description,
               ExternalDocs = new(InventoryApi2.ExternalDocs)
               {
                    Description = InventoryApi2.ExternalDocsDescription
               }
          };
          
          opt.AsyncApi = new AsyncApiDocument
          {
               Info = new Info(KafkaApi.Title, KafkaApi.Version)
               {
                    Description = KafkaApi.Description
               },
               Servers =
               {
                    { KafkaServer.Name, new Server(KafkaServer.Url, KafkaServer.ProtocolName)}
               },
               Tags = { 
                    inventoryApiTag,
                    inventoryApiTag2
               }
          };
          
          var channelsRequest = new ChannelsRequest(new ChannelRequest[]
          {
               new ChannelRequest(
                    Name: RequestTopic.Name, 
                    Description: RequestTopic.Description, 
                    Enumerable.Empty<ChannelParameterRequest>(),
                    ServerNames: new []{KafkaServer.Name},
                    Publish: new OperationRequest(
                         Id: RequestTopic.PublishOperation.Id,
                         Summary: RequestTopic.PublishOperation.Summary,
                         Description: RequestTopic.PublishOperation.Description,
                         new[] { CreateMessageRequest(RequestTopic.Name, inventoryApiTag) },
                         BindingsRef: null,
                         Tags: new string[]{ inventoryApiTag.Name }
                    ),
                    Subscribe: null,
                    BindingsRef: null),
               
               new ChannelRequest(
                    Name: RequestTopic2.Name, 
                    Description: RequestTopic2.Description, 
                    Enumerable.Empty<ChannelParameterRequest>(),
                    ServerNames: new []{KafkaServer.Name},
                    Publish: new OperationRequest(
                         Id: RequestTopic2.PublishOperation.Id,
                         Summary: RequestTopic2.PublishOperation.Summary,
                         Description: RequestTopic2.PublishOperation.Description,
                         new[] { CreateMessageRequest(RequestTopic2.Name, inventoryApiTag2) },
                         BindingsRef: null,
                         Tags: new string[]{ inventoryApiTag2.Name }
                    ),
                    Subscribe: null,
                    BindingsRef: null),
          });
          
          
          channelsGenerator.Generate(channelsRequest, opt.AsyncApi);
          
     }


     private MessageRequest CreateMessageRequest(string topicName, Tag tag) => new MessageRequest(
          Id: topicName + "." + NewInventoryRequest.Id,
          PayloadType: typeof(NewInventoryRequest),
          HeadersType: typeof(KafkaMessageHeader),
          new NewInventoryRequestExample(),
          Title: NewInventoryRequest.Title,
          Summary: NewInventoryRequest.Summary,
          Description: NewInventoryRequest.Description,
          BindingsRef: null,
          Tags: new string[] { tag.Name }
     );

}