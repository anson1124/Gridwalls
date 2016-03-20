using System;
using System.Text.RegularExpressions;

namespace SimpleClient
{
    public class SubscribedEvent
    {
        public static readonly string Name = typeof(SubscribedEvent).Name;
        public readonly string Tag;

        public SubscribedEvent(string tag)
        {
            Tag = tag;
        }

        public string Serialize()
        {
            return SubscribeEvent.GetSerializedString(Name, Tag);
        }

        public static SubscribedEvent Deserialize(string text)
        {
            string tag = SubscribeEvent.GetTagFrom(text);
            return new SubscribedEvent(tag);
        }
    }
}