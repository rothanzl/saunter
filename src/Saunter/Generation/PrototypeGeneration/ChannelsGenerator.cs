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
            var ch = GenerateChannel(channelRequest, schemaResolver, asyncApiDocument.Tags);
            asyncApiDocument.Channels[ch.Item1] = ch.Item2;
        }
    }

    private (string, ChannelItem) GenerateChannel(ChannelRequest ch, AsyncApiSchemaResolver schemaResolver, ISet<Tag> tags)
    {
        var channelItem = new ChannelItem()
        {
            Description = ch.Description,
            Parameters = GenerateChannelParameters(ch.Parameters, schemaResolver),
            Publish = GenerateOperation(ch.Publish, schemaResolver, tags),
            Subscribe = GenerateOperation(ch.Subscribe, schemaResolver, tags),
            Bindings = ch.BindingsRef != null ? new ChannelBindingsReference(ch.BindingsRef) : null,
            Servers = ch.ServerNames.ToList(),
        };
        
        return (ch.Name, channelItem);
    }

    private IDictionary<string, IParameter> GenerateChannelParameters(IEnumerable<ChannelParameterRequest> prms, 
        AsyncApiSchemaResolver schemaResolver)
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

    private Operation GenerateOperation(OperationRequest? op, AsyncApiSchemaResolver schemaResolver, ISet<Tag> tags)
    {
        if (op is null)
            return null;
        
        return new Operation()
        {
            OperationId = op.Id,
            Summary = op.Summary,
            Description = op.Description,
            Message = GenerateMessages(op.Messages, schemaResolver, tags),
            Bindings = op.BindingsRef != null ? new OperationBindingsReference(op.BindingsRef) : null,
            Tags = SubSelect(tags, op.Tags),
        };
    }

    private ISet<Tag> SubSelect(ISet<Tag> tags, string[] select)
    {
        if (select is null || select.Length == 0 || tags is null || tags.Count == 0)
            return new HashSet<Tag>();

        var result = tags
            .Where(t => select.Contains(t.Name))
            .ToArray();
        
        return result.Any() 
            ? new HashSet<Tag>(result) 
            : new HashSet<Tag>();
    }

    private Messages GenerateMessages(IEnumerable<MessageRequest> messages, AsyncApiSchemaResolver schemaResolver, ISet<Tag> tags)
    {
        List<IMessage> msgs = new();
        foreach (MessageRequest messageRequest in messages)
        {
            msgs.Add(GenerateMessage(messageRequest, schemaResolver, tags));
        }

        return new Messages()
        {
            OneOf = msgs
        };
    }

    private IMessage GenerateMessage(MessageRequest msg, AsyncApiSchemaResolver schemaResolver, ISet<Tag> tags)
    {
        var message = new Message()
        {
            MessageId = msg.Id,
            Name = msg.Id,
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
            Tags = SubSelect(tags, msg.Tags)
        };

        return schemaResolver.GetMessageOrReference(message);
    }
}