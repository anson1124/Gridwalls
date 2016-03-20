using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Logging;
using Messaging;
using SimpleClient;

namespace SimpleServer
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly Logger logger;
        private readonly HashSet<Node>  clients = new HashSet<Node>();
        private readonly Dictionary<string, HashSet<Node>> subscriptions = new Dictionary<string, HashSet<Node>>();
        private const string TagPattern = @"\[([a-zA-Z0-9]+)\] (.*)";

        public MessageDispatcher(Logger logger)
        {
            this.logger = logger;
        }

        public void AddClient(Node client)
        {
            clients.Add(client);
        }

        public void RemoveClient(Node client)
        {
            foreach (KeyValuePair<string, HashSet<Node>> subscription in subscriptions)
            {
                if (subscription.Value.Contains(client))
                {
                    subscription.Value.Remove(client);
                }
            }
            clients.Remove(client);
        }

        public void OnMessageReceived(Node source, string msg)
        {
            logger.Write<MessageDispatcher>("Received message: " + msg);
            if (msg.StartsWith(SubscribeEvent.Name))
            {
                SubscribeEvent subscription = SubscribeEvent.Deserialize(msg);
                addSubscription(source, subscription);
                sendSubscribedEvent(source, subscription);
            }
            else
            {
                broadcastMessage(source, msg);
            }
        }

        private void addSubscription(Node source, SubscribeEvent subscription)
        {
            logger.Write<MessageDispatcher>($"Adding subscription from {source} to tag {subscription.Tag}");

            HashSet<Node> nodesForTag;
            if (!subscriptions.TryGetValue(subscription.Tag, out nodesForTag))
            {
                subscriptions.Add(subscription.Tag, new HashSet<Node> {source});
            }
            else
            {
                subscriptions[subscription.Tag].Add(source);
            }
        }

        private void sendSubscribedEvent(Node source, SubscribeEvent subscription)
        {
            source.SendMessage(new SubscribedEvent(subscription.Tag).Serialize());
        }

        private void broadcastMessage(Node source, string msg)
        {
            if (messageContainsTagThatOneOrMoreClientsAreSubscribedTo(msg))
            {
                broadcastMessageToSubscriptions(source, msg);
            }
            else
            {
                broadcastMessageToClients(clients, source, msg);
            }
        }

        private bool messageContainsTagThatOneOrMoreClientsAreSubscribedTo(string msg)
        {
            return Regex.Matches(msg, TagPattern).Cast<Match>().Any(match => subscriptions.ContainsKey(getTagnameFromRegexMatch(match)));
        }

        private string getTagnameFromRegexMatch(Match eventMatch)
        {
            return eventMatch.Groups[1].Value;
        }

        private void broadcastMessageToSubscriptions(Node source, string msg)
        {
            string tagname = getTagnameFromRegexMatch(Regex.Match(msg, TagPattern));
            HashSet<Node> subscribedClients = subscriptions[tagname];
            var msgWithoutTags = removeTagnameFromMessage(msg);
            broadcastMessageToClients(subscribedClients, source, msgWithoutTags);
        }

        private string toString(IEnumerable<object> subscribedClients)
        {
            return subscribedClients.Aggregate("", (current, subscribedClient) => current + "," + subscribedClient.ToString());
        }

        private string removeTagnameFromMessage(string msg)
        {
            return Regex.Match(msg, TagPattern).Groups[2].Value;
        }

        private void broadcastMessageToClients(HashSet<Node> clients, Node source, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<Node> { source };
            IEnumerable<Node> clientsToBroadcastTo = clients.Except(theClientThatTheMessageCameFrom);

            logger.Write<MessageDispatcher>(
                $"Broadcasting message from {source} to {clientsToBroadcastTo.Count()} clients. Clients: {toString(clientsToBroadcastTo)}. Message: {msg}");
            foreach (var clientNode in clientsToBroadcastTo)
            {
                clientNode.SendMessage(msg);
            }
        }
    }
}