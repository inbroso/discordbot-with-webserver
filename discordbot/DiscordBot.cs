using Discord;
using Discord.Interactions;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using discordbot.Services;
using discordbot.Modules;
using System.Reflection.Metadata;
using System.Reflection;
using static discordbot.Services.CommandHandler;

namespace discordbot
{
    public class DiscordBot
    {
        private string _backupToken;
        private string _backupGuildId;

        private IConfiguration _config;
        private DiscordSocketClient _client;
        private InteractionService _commands;
        private CommandHandler _handler;

        public APICommands _apiHandler;

        private ServiceProvider _service;


        private ulong _guildId;

        public static Task Main(string[] args) => new DiscordBot().StartAsync();

        public async Task MainAsync(string[] args) { }


        public DiscordBot() { }

        public async Task StartAsync()
        {
            try
            {
                // Environment Variables have to be set in order to run the bot:
                // DISCORD_BOT_TOKEN = The Bot Token found in the developer portal
                // DISCORD_GUILD_ID = The Guild ID of the server the bot is supposed to run on

                // The Bot uses a config.json file to store the token and guild id, so you can also create a config.json if you don't know how to use environment variables.
                //{
                //  "Token": "TOKEN HERE",
                //  "GuildId": "GUILD ID HERE"
                //}

                if (File.Exists(AppContext.BaseDirectory + "config.json"))
                {
                    Console.WriteLine("Config exists! Continuing...");
                }
                else
                {
                    try
                    {
                        _backupToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
                        _backupGuildId = Environment.GetEnvironmentVariable("DISCORD_GUILD_ID");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Environment.Exit(0);
                    }
                    if (_backupToken == null || _backupGuildId == null)
                    {
                        Console.WriteLine("Error: No Environment Variables found!");
                        Environment.Exit(0);
                    }
                    Console.WriteLine("Config doesn't exist! Creating from Environment Variables...");
                    File.WriteAllText(AppContext.BaseDirectory + "config.json", "{\n  \"Token\": \"" + _backupToken + "\",\n  \"GuildId\": \"" + _backupGuildId + "\"\n}\n");
                }

                var _builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json");

                _config = _builder.Build();
                _guildId = ulong.Parse(_config["GuildId"]);
                
                using (_service = ConfigureServices())
                {
                    // Client assignen
                    _client = _service.GetRequiredService<DiscordSocketClient>();
                    _commands = _service.GetRequiredService<InteractionService>();
                    _handler = _service.GetRequiredService<CommandHandler>();

                    // logging
                    _client.Log += LogAsync;
                    _commands.Log += LogAsync;
                    _client.Ready += ReadyAsync;

                    // Taking the token from the config
                    await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                    await _client.StartAsync();

                    // Start CommandHandler
                    await _service.GetRequiredService<CommandHandler>().InitializeAsync();
                    _apiHandler = new APICommands(_handler, this);

                    await Task.Delay(Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
        private async Task SetBotProfilePictureAsync(DiscordSocketClient client, string imagePath)
        {
            using (var stream = File.OpenRead(imagePath))
            {
                var image = new Image(stream);
                await client.CurrentUser.ModifyAsync(profile => profile.Avatar = image);
            }
        }

        private async Task ReadyAsync()
        {
            await _commands.RegisterCommandsToGuildAsync(_guildId);

            Console.WriteLine($"Connected as {_client.CurrentUser}");


            // Sets the bot's status
            //await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            
            // Sets the bot's activity
            // ActivityType.Playing, Listening, Watching, Streaming
            //await _client.SetGameAsync("im Code rum...", null, ActivityType.Playing);
            
            // Sets the bot's profile picture
            //await SetBotProfilePictureAsync(_client, "INSERT PATH HERE!");
        }

        // builds the Service Provider
        private ServiceProvider ConfigureServices()
        {
            // returns the Service Provider
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        public ulong GetGuildId()
        {
            return _guildId;
        }

        public DiscordSocketClient GetDiscordSocketClient()
        {
            return _client;
        }

        
    }
}