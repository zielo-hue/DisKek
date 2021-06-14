using System;
using System.Linq;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Extensions.Interactivity;
using Disqord.Gateway;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DisKek
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(x =>
                {
                    x.AddCommandLine(args);
                })
                .ConfigureAppConfiguration(x =>
                {
                    x.AddCommandLine(args);
                    x.AddEnvironmentVariables("DISQORD_");
                })
                .ConfigureLogging(x =>
                {
                    var logger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                        .WriteTo.File($"logs/log-{DateTime.Now:HH_mm_ss}.txt", LogEventLevel.Verbose, fileSizeLimitBytes: null, buffered: true)
                        .CreateLogger();
                    x.AddSerilog(logger, true);

                    x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddInteractivity();
                })
                .ConfigureDiscordBotSharder((context, bot) =>
                {
                    bot.Token = context.Configuration["TOKEN_DISKEK"];
                    bot.UseMentionPrefix = true;
                    bot.Intents += GatewayIntent.Members;
                    bot.Prefixes = new[] { "kek." };
                    bot.ReadyEventDelayMode = ReadyEventDelayMode.Guilds;
                    bot.ShardCount = 1;
                })
                .Build();
            
            try
            {
                using (host)
                {
                    host.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}