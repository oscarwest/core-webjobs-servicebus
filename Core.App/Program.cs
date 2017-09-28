using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Core.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            

            JobHostConfiguration config = new JobHostConfiguration(storageConnectionString);
            ServiceBusConfiguration servicebusConfig = new ServiceBusConfiguration()
            {
                ConnectionString = serviceBusConnectionString,
                PrefetchCount = 16
            };

            config.UseServiceBus(servicebusConfig);
            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
