using System.IO;
using Microsoft.Azure.WebJobs;

namespace Core.App
{
    public class Functions
    {
        private const string PrefixForAll = "s-";
        private const string QueueNamePrefix = PrefixForAll + "queue-";
        private const string TopicName = PrefixForAll + "topic";

        public const string StartQueueName = QueueNamePrefix + "start";

        // Topic subscription listener #1
        public static void SBTopicListener1(
            [ServiceBusTrigger(TopicName, QueueNamePrefix + "topic-1")] string message,
            TextWriter log)
        {
            log.WriteLine("SBTopicListener1: " + message);
        }
    }
}
