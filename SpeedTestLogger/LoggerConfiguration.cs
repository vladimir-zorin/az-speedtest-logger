using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpeedTestLogger
{
    public class LoggerConfiguration
    {
        public readonly string UserId;
        public readonly int LoggerId;
        public readonly RegionInfo LoggerLocation;
        public readonly Uri ApiUrl;
        public readonly string ServiceBusConnectionString;
        public readonly string ServiceBusTopic;
        public readonly string ServiceBusSubscriptionName;

        public LoggerConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json");

            var configuration = builder.Build();

            var countryCode = configuration["loggerLocationCountryCode"];
            LoggerLocation = new RegionInfo(countryCode);
            UserId = configuration["userId"];
            LoggerId = int.Parse(configuration["loggerId"]);
            ApiUrl = new Uri(configuration["speedTestApiUrl"]);

            // service bus
            ServiceBusConnectionString = configuration["ServiceBus:connectionString"];
            ServiceBusTopic = configuration["ServiceBus:topicName"];
            ServiceBusSubscriptionName = configuration["ServiceBus:subscriptionName"];

            Console.WriteLine("Logger located in {0}", LoggerLocation.EnglishName);
        }
    }
}
