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
                    inventoryApiTag
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
                         new[]
                         {
                              new MessageRequest(
                                   Id: NewInventoryRequest.Id,
                                   Name: NewInventoryRequest.Name,
                                   PayloadType: typeof(NewInventoryRequest),
                                   HeadersType: typeof(KafkaMessageHeader),
                                   new NewInventoryRequestExample(),
                                   Title: NewInventoryRequest.Title,
                                   Summary: NewInventoryRequest.Summary,
                                   Description: NewInventoryRequest.Description,
                                   BindingsRef: null,
                                   Tags: new string[]{ inventoryApiTag.Name }
                              ),
                         },
                         BindingsRef: null,
                         Tags: new string[]{ inventoryApiTag.Name }
                    ),
                    Subscribe: null,
                    BindingsRef: null)
          });
          
          
          channelsGenerator.Generate(channelsRequest, opt.AsyncApi);
          
     }

}