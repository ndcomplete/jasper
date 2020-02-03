using System;

namespace Jasper.Attributes
{
    /// <summary>
    /// Used to specify outbound topic names per message type. Only used if the outbound
    /// endpoint is using topic-based routing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class TopicAttribute : Attribute
    {
        public string TopicName { get; }

        public TopicAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
