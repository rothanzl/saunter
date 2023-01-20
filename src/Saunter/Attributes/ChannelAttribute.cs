using System;
using System.Linq;

namespace Saunter.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ChannelAttribute : Attribute
    {
        private Type ChannelSourceProvider { get; }

        public ChannelAttribute(Type channelSourceProvider)
        {
            if (channelSourceProvider.GetInterface(nameof(IChannelSourceProvider)) is null)
                throw new ArgumentException($"Type {channelSourceProvider.FullName ?? channelSourceProvider.Name} must implement {nameof(IChannelSourceProvider)} interface");

            ChannelSourceProvider = channelSourceProvider;
        }

        public IChannelSourceProvider CreateSourceProvider()
        {
            var instance = Activator.CreateInstance(ChannelSourceProvider) as IChannelSourceProvider;

            if (instance == null)
                throw new NullReferenceException($"Cannot create instance of {ChannelSourceProvider.FullName ?? ChannelSourceProvider.Name}");

            return instance;

        }
    }
}