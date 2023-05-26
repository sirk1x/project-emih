using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace project_emih
{
    internal class Invision
    {
        public static IAudioClient AudioClient;


        public static string Directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string ImageDir = Directory + "//img//";
        public static Authenticator<ulong> GuildAuthenticator = new Authenticator<ulong>(Directory + "//serverauth.json");
        public static Authenticator<ulong> AdminAuthenticator = new Authenticator<ulong>(Directory + "//admins.json");
        private DiscordSocketClient _client;
        private CommandHandler CommandHandler { get; set; }
        private CommandService CommandService { get; set; }

        public Invision()
        {
            AdminAuthenticator.Auth(0);
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });
            _client.Log += _client_Log;
            CommandService = new CommandService();
            CommandHandler = new CommandHandler(_client);
            Create();
            _client.Ready += _client_Ready;

            var invite = string.Format(
                "https://discord.com/api/oauth2/authorize?client_id={0}&permissions={1}&scope=bot%20applications.commands",
                InvisionConfig.Current.Discord_App_Id,
                534727096384);
            File.WriteAllText(Directory + "//invite.txt", invite);
        }

        private Task _client_Log(LogMessage msg)
        {

            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;

        }

        private async Task _client_Ready()
        {
            await _client.SetStatusAsync(InvisionConfig.Current.BotStatus);
            await _client.SetGameAsync(InvisionConfig.Current.BotGame);
            Console.WriteLine("Bot Online!");
        }

        private async void Create()
        {
            await _client.LoginAsync(Discord.TokenType.Bot, InvisionConfig.Current.Discord_Bot_Token);
            await _client.StartAsync();
        }

    }
}
