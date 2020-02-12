using SpeedTestLogger.Models;
using SpeedTestLogger.Services;
using System;
using System.Threading.Tasks;

namespace SpeedTestLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello SpeedTestLogger!");

            var config = new LoggerConfiguration();

            var serviceBusListener = new ServiceBusListener(config);
            await serviceBusListener.Listen(async (msg) => await DoWork(config));
        }

        private static async Task DoWork(LoggerConfiguration config)
        {
            var runner = new SpeedTestRunner(config.LoggerLocation);
            var testData = runner.RunSpeedTest();
            var results = new TestResult
            {
                SessionId = new Guid(),
                User = config.UserId,
                Device = config.LoggerId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Data = testData
            };

            var success = false;
            using (var client = new SpeedTestApiClient(config.ApiUrl))
            {
                success = await client.PublishTestResult(results);
            }

            if (success)
            {
                Console.WriteLine("Speedtest complete!");
            }
            else
            {
                Console.WriteLine("Speedtest failed!");
            }
        }
    }
}
