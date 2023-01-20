namespace Saunter.Attributes;

public interface IChannelSourceProvider
{
    /// <summary>
    /// The name of the channel. 
    /// Format depends on the underlying messaging protocol's conventions.
    /// For example, amqp uses dot-separated paths 'light.measured'.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// An optional description of this channel item.
    /// CommonMark syntax can be used for rich text representation.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The name of a channel bindings item to reference.
    /// The bindings must be added to components/channelBindings with the same name.
    /// </summary>
    string BindingsRef { get; }

    /// <summary>
    /// The servers on which this channel is available, specified as an optional unordered
    /// list of names (string keys) of Server Objects defined in the Servers Object (a map).
    /// If servers is absent or empty then this channel must be available on all servers
    /// defined in the Servers Object.
    /// </summary>
    string[] Servers { get; }
}