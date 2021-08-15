using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace WhoDat.Module.Commands
{
    class GameCommands : BaseCommandModule
    {
        private List<KeyValuePair<string,string>> ImageUrls = new List<KeyValuePair<string,string>>
        {
            new KeyValuePair<string,string>("Amanda", "https://cdn.discordapp.com/attachments/865730865745231892/873307605903241216/Amanda.png"),
            new KeyValuePair<string,string>("Andre", "https://cdn.discordapp.com/attachments/865730865745231892/873307611540369428/Andre.png"),
            new KeyValuePair<string,string>("Becky", "https://cdn.discordapp.com/attachments/865730865745231892/873307616258949130/Becky.png"),
            new KeyValuePair<string,string>("Catherine", "https://cdn.discordapp.com/attachments/865730865745231892/873307621434744882/Catherine.png"),
            new KeyValuePair<string,string>("Charlie", "https://cdn.discordapp.com/attachments/865730865745231892/873307625452863518/Charlie.png"),
            new KeyValuePair<string,string>("David", "https://cdn.discordapp.com/attachments/865730865745231892/873307629450067968/David.png"),
            new KeyValuePair<string,string>("Geoff", "https://cdn.discordapp.com/attachments/865730865745231892/873307635250786324/Geoff.png"),
            new KeyValuePair<string,string>("Idris", "https://cdn.discordapp.com/attachments/865730865745231892/873307637905764442/Idris.png"),
            new KeyValuePair<string,string>("Jake", "https://cdn.discordapp.com/attachments/865730865745231892/873307642766975117/Jake.png"),
            new KeyValuePair<string,string>("John", "https://cdn.discordapp.com/attachments/865730865745231892/873307646868992000/John.png"),
            new KeyValuePair<string,string>("Julie", "https://cdn.discordapp.com/attachments/865730865745231892/873307652963315802/Julie.png"),
            new KeyValuePair<string,string>("Karen", "https://cdn.discordapp.com/attachments/865730865745231892/873307655580573816/Karen.png"),
            new KeyValuePair<string,string>("Kelly", "https://cdn.discordapp.com/attachments/865730865745231892/873307660852797491/Kelly.png"),
            new KeyValuePair<string,string>("Liam", "https://cdn.discordapp.com/attachments/865730865745231892/873307665361686538/Liam.png"),
            new KeyValuePair<string,string>("Maureen", "https://cdn.discordapp.com/attachments/865730865745231892/873307670826877019/Maureen.png"),
            new KeyValuePair<string,string>("Neil", "https://cdn.discordapp.com/attachments/865730865745231892/873307675058905159/Neil.png"),
            new KeyValuePair<string,string>("Natasha", "https://cdn.discordapp.com/attachments/865730865745231892/873307674152947722/Natasha.png"),
            new KeyValuePair<string,string>("Paula", "https://cdn.discordapp.com/attachments/865730865745231892/873307677709721650/Paula.png"),
            new KeyValuePair<string,string>("Roger", "https://cdn.discordapp.com/attachments/865730865745231892/873307678095593513/Roger.png"),
            new KeyValuePair<string,string>("Shelly", "https://cdn.discordapp.com/attachments/865730865745231892/873307681664929792/Shelly.png"),
            new KeyValuePair<string,string>("Susan", "https://cdn.discordapp.com/attachments/865730865745231892/873307696965746688/Susan.png"),
            new KeyValuePair<string,string>("Steve", "https://cdn.discordapp.com/attachments/865730865745231892/873307700052754492/Steve.png"),
            new KeyValuePair<string,string>("Simone", "https://cdn.discordapp.com/attachments/865730865745231892/873307704398082098/Simone.png"),
            new KeyValuePair<string,string>("Tracy", "https://cdn.discordapp.com/attachments/865730865745231892/873307710018453584/Tracy.png")
         };

        [Command("whodat")]
        public async Task Play(CommandContext ctx)
        {
            var image = new Random();
            int imageId = image.Next(ImageUrls.Count);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Who Dat",
                Color = DiscordColor.Blurple,
                Url = "https://whodat.live/"
            };
            embed.AddField("You are playing as",$"{ImageUrls[imageId].Key}");

            await ctx.Member.SendMessageAsync(embed: embed.WithImageUrl(ImageUrls[imageId].Value));
        }
    }
}
