using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace DisKek
{
    public class MainModule : DiscordModuleBase
    {
        [Command("help")]
        [Description("Help!!!")]
        public async Task HelpAsync()
        {
            var embed = new LocalEmbed()
                .WithDescription("no help");
            await Response(embed);
        }

        [Command("check")]
        [Description("Assert message kek status")]
        public async Task CheckMessageAsync()
        {
            
        }
    }
}