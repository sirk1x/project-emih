using project_emih;
using dcord_ivisionAI_emih_dalle2;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Images;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace project_emih
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += _client_SlashCommandExecuted;
        }

        private async Task _client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            await SlashCommandHandler(arg);
        }

        private async Task RegisterCommands(SocketGuild guild)
        {
            var guildCommand = new SlashCommandBuilder().WithName("vision").WithDescription("Let Dall-e 2 visionize it.").AddOption("prompt", ApplicationCommandOptionType.String, "Text only", true);
            var guildCommand2 = new SlashCommandBuilder().WithName("think").WithDescription("Let ChatGPT think about.").AddOption("prompt", ApplicationCommandOptionType.String, "Text only", true);
            var guildCommand3 = new SlashCommandBuilder().WithName("auth").WithDescription("Authenticate thist server.");

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
                await guild.CreateApplicationCommandAsync(guildCommand2.Build());
                await guild.CreateApplicationCommandAsync(guildCommand3.Build());
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public async Task Client_Ready()
        {
            foreach (var item in _client.Guilds)
            {
                await RegisterCommands(item);
                
                await item.CurrentUser.ModifyAsync(x => x.Nickname = InvisionConfig.Current.BotName);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "vision":
                    await Vision(command);
                    break;
                case "think":
                    await Think(command);
                    break;
                case "auth":
                    await AuthServer(command);
                    break;
            }
        }

        private async Task AuthServer(SocketSlashCommand command)
        {
            if (!Invision.AdminAuthenticator.HasAuth(command.User.Id))
            {
                await command.RespondAsync(embed: IVHelper.ReturnError(
                    string.Format("User {0} is not an administrator", command.User.Id)
                    ));
                return;
            }
            
            Invision.GuildAuthenticator.Auth((ulong)command.GuildId);
            
            var guild = _client.GetGuild((ulong)command.GuildId);
            
            await command.RespondAsync(embed: IVHelper.ReturnMessage(
                "Success",
                string.Format("Server {0} has been authenticated!", guild.Name),
                Color.Gold));

        }

        private async Task Vision(SocketSlashCommand command)
        {
            if (!Invision.GuildAuthenticator.HasAuth((ulong)command.GuildId))
            {
                await command.RespondAsync(embed: IVHelper.ReturnError("Server has no license"));
                return;
            }

            await command.RespondAsync(text: "Working... one moment pls");
            
            OpenAIAPI api = new OpenAIAPI(InvisionConfig.Current.OpenAI_Api_Key);
            
            List<FileAttachment> embeds = new List<FileAttachment>();
            
            try
            {
                var result = await api.ImageGenerations.CreateImageAsync(
                    new ImageGenerationRequest(
                        command.Data.Options.First().Value.ToString(), 
                        InvisionConfig.Current.Dalle_Images_Per_Request, 
                        size: InvisionConfig.Current.Dalle_Image_Size_Internal
                        ));
                
                var _files = new string[result.Data.Count];
                
                for (int i = 0; i < result.Data.Count; i++)
                {
                    var file = await IVHelper.DownloadImage(result.Data[i].Url);
                    _files[i] = file;
                    embeds.Add(new FileAttachment(file));
                }
                
                await command.FollowupWithFilesAsync(
                    text: "This one goes out to " + command.User.Mention + command.Data.Options.First().Value.ToString() + "\n",
                    attachments: embeds.ToArray());
                
                IVHelper.CleanUp(_files);
            }
            catch (Exception e)
            {

                await command.FollowupAsync(embed: IVHelper.ReturnError("Error: " + e.GetType().ToString()));
                Console.WriteLine(e.ToString());
            }
            await command.DeleteOriginalResponseAsync();
        }

        private async Task Think(SocketSlashCommand command)
        {
            if (!Invision.GuildAuthenticator.HasAuth((ulong)command.GuildId))
            {
                await command.RespondAsync(embed: IVHelper.ReturnError("Server has no license"));
                return;
            }

            await command.RespondAsync(text: "Working... one moment pls");
            
            OpenAIAPI api = new OpenAIAPI(InvisionConfig.Current.OpenAI_Api_Key);
           
            try
            {
                var prompt = command.Data.Options.First().Value.ToString();
                
                var result = await api.Chat.CreateChatCompletionAsync(prompt);
               
                var reply = IVHelper.Chunk(result.Choices[0].Message.Content, 2000).ToArray();
               
                for (int i = 0; i < reply.Length; i++)
                {
                
                    var embed = new EmbedBuilder().WithDescription("```" + reply[0] + "```");
                
                    if (i == 0)
                        embed.WithTitle(prompt);
               
                    var mention = command.User.Mention;
               
                    var fUp = await command.FollowupAsync(text: "I thought about it " + mention + ":",
                        embed: embed.Build());
                }

            }
            catch (Exception e)
            {
                await command.FollowupAsync(embed: IVHelper.ReturnError("Error: " + e.GetType().ToString()));
                
                Console.WriteLine(e.ToString());
            }

            await command.DeleteOriginalResponseAsync();
        }
    }
}
