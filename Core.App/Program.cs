using System;
using System.IO;
using Azure.WebJobs.Sdk.Core.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Core.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            JobHostConfiguration config = new JobHostConfiguration();
            ServiceBusConfiguration servicebusConfig = new ServiceBusConfiguration()
            {
                ConnectionString = configuration.GetSection("connectionstrings:Servicebus").Value,
                PrefetchCount = 16
            };

            config.UseServiceBus(servicebusConfig);
            config.DashboardConnectionString = configuration.GetSection("connectionstrings:AzureWebJobsDashboard").Value;
            config.StorageConnectionString = configuration.GetSection("connectionstrings:AzureWebJobsStorage").Value;

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
