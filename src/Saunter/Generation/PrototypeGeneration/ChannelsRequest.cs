using System.Collections.Generic;

namespace Saunter.Generation.PrototypeGeneration;

public record ChannelsRequest(IEnumerable<ChannelRequest> Channels);