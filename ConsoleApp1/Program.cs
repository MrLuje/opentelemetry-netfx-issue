using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var activitySource = new ActivitySource("MyCompany.MyProduct.MyLibrary");
            var trace = Sdk.CreateTracerProviderBuilder()
                .AddSource("MyCompany.MyProduct.MyLibrary")
                .AddConsoleExporter()
                .AddOtlpExporter(b => b.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf)
                .Build();

            var meter = new Meter("TestMeter");
            var metric = Sdk.CreateMeterProviderBuilder()
                .AddConsoleExporter()
                .AddOtlpExporter(b => b.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf)
                .AddMeter(meter.Name)
                .Build();

            var counter = meter.CreateCounter<int>("testcounter", "thing", "A count of things");

            Console.WriteLine(@"This app will endlessly publish :
Trace with Service Name = unknown_service");

            while(true)
            {
                var activity = activitySource.StartActivity("SayHello")
                    .SetTag("foo", 1)
                    .SetTag("bar", "Hello, World!")
                    .SetTag("baz", new int[] { 1, 2, 3 })
                    .SetStatus(ActivityStatusCode.Ok);
                counter.Add(1);

                await Task.Delay(TimeSpan.FromSeconds(5));
                Console.WriteLine("------------------------------------------------------------------");
            }
        }
    }
}
