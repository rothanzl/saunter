using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using NJsonSchema.Generation;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.Generation.SchemaGeneration;

namespace Saunter.Generation.PrototypeGeneration;

public class ChannelsGenerator : IChannelsGenerator
{
    private readonly JsonSchemaGenerator _generator;
    private readonly AsyncApiSchemaOptions _schemaOptions;

    
    public ChannelsGenerator(IOptions<AsyncApiSchemaOptions> schemaOptions)
    {
        _schemaOptions = schemaOptions.Value;
        _generator = new JsonSchemaGenerator(schemaOptions.Value);
    }
    
    
    public void Generate(ChannelsRequest req, AsyncApiDocument asyncApiDocument)
    {
        var schemaResolver = new AsyncApiSchemaResolver(asyncApiDocument, _schemaOptions);
        
        foreach (ChannelRequest channelRequest in req.Channels)
        {
            var ch = GenerateChannel(channelRequest, schemaResolver);
            asyncApiDocument.Channels[ch.Item1] = ch.Item2;
        }
    }

    private (string, ChannelItem) GenerateChannel(ChannelRequest ch, AsyncApiSchemaResolver schemaResolver)
    {
        var channelItem = new ChannelItem()
        {
            Description = ch.Description,
            Parameters = GenerateChannelParameters(ch.Parameters, schemaResolver),
            Publish = GenerateOperation(ch.Publish, schemaResolver),
            Subscribe = GenerateOperation(ch.Subscribe, schemaResolver),
            Bindings = ch.BindingsRef != null ? new ChannelBindingsReference(ch.BindingsRef) : null,
            Servers = ch.ServerNames.ToList(),
        };
        
        return (ch.Name, channelItem);
    }

    private IDictionary<string, IParameter> GenerateChannelParameters(IEnumerable<ChannelParameterRequest> prms, AsyncApiSchemaResolver schemaResolver)
    {
        Dictionary<string, IParameter> result = new();
        foreach (ChannelParameterRequest prm in prms)
        {
            var parameter = schemaResolver.GetParameterOrReference(new Parameter
            {
                Description = prm.Description,
                Name = prm.Name,
                Schema = _generator.Generate(prm.Type, schemaResolver),
                Location = prm.Location,
            });
                    
            result.Add(prm.Name, parameter);
        }

        return result;
    }

    private Operation GenerateOperation(OperationRequest? op, AsyncApiSchemaResolver schemaResolver)
    {
        if (op is null)
            return null;
        
        return new Operation()
        {
            OperationId = op.Id,
            Summary = op.Summary,
            Description = op.Description,
            Message = GenerateMessages(op.Messages, schemaResolver),
            Bindings = op.BindingsRef != null ? new OperationBindingsReference(op.BindingsRef) : null,
            Tags = new HashSet<Tag>(op.Tags?.Select(x => new Tag(x)) ?? new List<Tag>())
        };
    }

    private Messages GenerateMessages(IEnumerable<MessageRequest> messages, AsyncApiSchemaResolver schemaResolver)
    {
        List<IMessage> msgs = new();
        foreach (MessageRequest messageRequest in messages)
        {
            msgs.Add(GenerateMessage(messageRequest, schemaResolver));
        }

        return new Messages()
        {
            OneOf = msgs
        };
    }

    private IMessage GenerateMessage(MessageRequest msg, AsyncApiSchemaResolver schemaResolver)
    {
        var message = new Message()
        {
            MessageId = msg.Id,
            Payload = _generator.Generate(msg.PayloadType, schemaResolver),
            Headers = msg.HeadersType != null
                ? _generator.Generate(msg.HeadersType, schemaResolver)
                : null,
            Examples = msg.ExamplesProvider.GetExamples(),
            Title = msg.Title,
            Summary = msg.Summary,
            Description = msg.Description,
            Bindings = msg.BindingsRef != null
                ? new MessageBindingsReference(msg.BindingsRef)
                : null,
            Tags = new HashSet<Tag>(msg.Tags?.Select(x => new Tag(x)) ?? new List<Tag>())
        };
        
        message.Name = msg.Name ?? message.Payload.ActualSchema.Id;
        
        return schemaResolver.GetMessageOrReference(message);
    }
}