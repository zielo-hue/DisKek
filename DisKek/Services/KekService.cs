using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Hosting;
using Disqord.Rest;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DisKek
{
    public class KekService : DiscordBotService
    {
        /// <summary>
        /// A service that handles reactions. This is terrible and will be refactored to use database instead.
        /// </summary>
        public FileService FileService;
        private readonly DiscordClientBase _client;
        
        public KekService(ILogger<KekService> logger, DiscordBotBase bot) : base(logger, bot)
        {
            _client = bot;
            FileService = (FileService) Bot.Services.GetService(typeof(FileService));
        }

        private ValueTask ClientOnReactionAdded(object sender, ReactionAddedEventArgs e)
        {
            // var minReactions = FileService._targetChannels.Contains(e.ChannelId) ? 6 : 3;
            var minReactions = 1;
            if (!FileService.ReactionEmojiDictionary.ContainsKey(e.Emoji.Name))
                return default;
            // use emoji from eventargs as dict key
            FileService.ReactionEmojiDictionary.TryGetValue(e.Emoji.Name, out var value);
            switch (value)
            {
                case 0:
                    RepostMessage(847345475957817365, e);
                    break;
                case 1:
                    RepostMessage(847345493569699850, e);
                    break;
                default:
                    Logger.Log(LogLevel.Error, "failed reposting message!");
                    break;
            }

            return default;
        }
        // use concurrent dictionary to cache messages???

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Log(LogLevel.Debug, "ExecuteAsync in KekService");
            await Client.WaitUntilReadyAsync(stoppingToken);
            _client.ReactionAdded += ClientOnReactionAdded;
        }

        async Task RepostMessage(Snowflake channelId, ReactionAddedEventArgs e)
        {
            var ogmsg = e.Message;
            var embed = new LocalEmbed()
                .WithTitle(e.Emoji.Name);
            if (e.Message is null)
            {
                embed.WithDescription("Message not cached! Not embedding...");
            }
            else
            { // More elegant solution pls?
                embed.WithDescription($"[Jump to message]({ogmsg.GetJumpUrl()})")
                    .WithImageUrl(ogmsg.Attachments.FirstOrDefault()?.Url)
                    .WithImageUrl(ogmsg.Embeds.FirstOrDefault()?.Url)
                    .WithAuthor(ogmsg.Author);
            }
            
            
            var msg = new LocalMessage().WithEmbed(embed);
            await _client.SendMessageAsync(channelId, msg, new DefaultRestRequestOptions());
        }
    }
}