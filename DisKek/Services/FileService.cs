using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DisKek
{
    public class FileService : DiscordBotService
    {
        public readonly Dictionary<string, uint> ReactionEmojiDictionary; // we should be using emoji ids, not names
        public List<ulong> _targetChannels;
        
        public FileService(ILogger<FileService> logger, DiscordBotBase bot) : base(logger, bot)
        {
            JObject conf = JObject.Parse(File.ReadAllText(@"config.json"));
            ReactionEmojiDictionary = new Dictionary<string, uint>
            {
                { "kek", 0 },
                { "cringe", 1 }
            };
            // ReactionEmojiDictionary = JsonConvert.DeserializeObject<Dictionary<string, uint>>(conf["emojis"]!.ToString()); // This doesn't work, but it doesn't matter.
            _targetChannels = JsonConvert.DeserializeObject<List<ulong>>(conf["target_channels"]!.ToString());
        }
    }
}