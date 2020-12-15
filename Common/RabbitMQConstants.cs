namespace Common
{
    public static class RabbitMQConstants
    {
        public static string HostName => "localhost";

        public static string QueueNameForDirectPublish => "direct-queue";
        public static string QueueNameForTaskPublish => "task-queue";

        public static string FanOutExchangeName => "fanout-exchange";
    }
}
