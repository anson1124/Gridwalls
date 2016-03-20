
using System;
using System.Runtime.Serialization;

namespace SimpleClient
{
    public class SubscribeEvent
    {
        public static readonly string Name = typeof(SubscribeEvent).Name;
        public readonly string Tag;

        public SubscribeEvent(string tag)
        {
            Tag = tag;
        }

        public string Serialize()
        {
            return GetSerializedString(Name, Tag);
        }

        public static string GetSerializedString(string name, string tag)
        {
            return $"{name}-{tag}";
        }

        public static SubscribeEvent Deserialize(string text)
        {
            string tag = GetTagFrom(text);
            return new SubscribeEvent(tag);
        }

        internal static string GetTagFrom(string text)
        {
            return text.Substring(text.IndexOf("-", StringComparison.Ordinal) + 1);
        }
    }
}