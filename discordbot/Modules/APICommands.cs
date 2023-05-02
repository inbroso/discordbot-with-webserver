using Discord.WebSocket;
using Discord;
using discordbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static discordbot.Services.CommandHandler;

namespace discordbot.Modules
{
    public class APICommands
    {
        private readonly ulong botChannel = (ulong)1084525461943615609;

        private readonly CommandHandler _handler;
        private readonly DiscordBot _discordBot;

        public APICommands(CommandHandler handler, DiscordBot discordBot) {
            _handler = handler;
            _discordBot = discordBot;
        }

        //zum Testen erstmal im DiscordBot.cs drin lassen
        public async Task ProcessSlashCommandAsync(SocketGuild guild)
        {
            // Hier kannst du den Slash-Befehl als Textnachricht in den Kanal senden:
            var channel = guild.TextChannels.FirstOrDefault(x => x.Id == botChannel);
            if (channel != null)
            {
                await channel.SendMessageAsync($"Ping-Pong Test! Ich bin erreichbar!");
            }
        }

        public async Task ProcessGiveRolesCommandAsync(SocketGuild guild, ulong userId, ulong roleId)
        {
            var channel = guild.TextChannels.FirstOrDefault(x => x.Id == botChannel);
            if (channel != null)
            {
                try
                {
                    IRole role = guild.GetRole(roleId);
                    IGuildUser user = guild.GetUser(userId);
                    CommandHandler.Result result = await _handler.GiveRoleToUserAsync(user, role);
                
                        if (result == CommandHandler.Result.Success)
                            await channel.SendMessageAsync(guild.Name + ": " + $"Der User (**{user.Mention}**) bekam die Rolle: **{role.Mention}**");
                        else if (result == CommandHandler.Result.RoleNotAllowed)
                            await channel.SendMessageAsync(guild.Name + ": " + $"Die Rolle **{role.Mention}** darf **nicht** vergeben werden!");
                        else if (result == CommandHandler.Result.AlreadyHasRole)
                            await channel.SendMessageAsync(guild.Name + ": " + $"Der User (**{user.Mention}**) hat bereits die Rolle: **{role.Mention}**");
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync("Fehler: " + ex.Message);
                }
            }
        }
        public async Task ProcessTakeRolesCommandAsync(SocketGuild guild, ulong userId, ulong roleId)
        {
            var channel = guild.TextChannels.FirstOrDefault(x => x.Id == botChannel);
            if (channel != null)
            {
                try
                {
                    IRole role = guild.GetRole(roleId);
                    IGuildUser user = guild.GetUser(userId);
                    CommandHandler.Result result = await _handler.TakeRoleFromUserAsync(user, role);
                    if (result == CommandHandler.Result.Success)
                        await channel.SendMessageAsync(guild.Name + ": " + $"Der User (**{user.Mention}**) hat die Rolle **{role.Mention}** weggenommen bekommen");
                    else if (result == CommandHandler.Result.DoesNotHaveRole)
                        await channel.SendMessageAsync(guild.Name + ": " + $"Der User (**{user.Mention}**) hat die Rolle **{role.Mention}** nicht");
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync("Fehler: " + ex.Message);
                }
            }
        }

        public async Task<IEnumerable<RoleInfo>> ProcessGetServerRolesCommandAsync()
        {
            DiscordSocketClient client = _discordBot.GetDiscordSocketClient();
            ulong guildId = _discordBot.GetGuildId();

            SocketGuild guild = client.GetGuild(guildId);
            if (guild == null)
            {
                return null;
            }

            IReadOnlyCollection<SocketRole> roles = guild.Roles;

            var roleInfo = roles.Where(role => role.Name != "@everyone").OrderByDescending(role => role.Position).Select(role => new RoleInfo
            {
                Name = role.Name,
                Color = $"#{role.Color.R:X2}{role.Color.G:X2}{role.Color.B:X2}",
                RoleId = role.Id,
                Position = role.Position
            });

            return roleInfo;
        }

        public async Task<IEnumerable<RoleInfo>> ProcessGetUserRolesCommandAsync(ulong userId)
        {
            DiscordSocketClient client = _discordBot.GetDiscordSocketClient();
            ulong guildId = _discordBot.GetGuildId();

            SocketGuild guild = client.GetGuild(guildId);
            IGuildUser _user = guild.GetUser(userId);
            
            if (guild == null)
            {
                return null;
            }

            IReadOnlyCollection<SocketRole> roles = await _handler.GetUserRolesAsync(_user);

            var roleInfo = roles.Where(role => role.Name != "@everyone").OrderByDescending(role => role.Position).Select(role => new RoleInfo
            {
                Name = role.Name,
                Color = $"#{role.Color.R:X2}{role.Color.G:X2}{role.Color.B:X2}",
                RoleId = role.Id,
                Position = role.Position
            });
           

            return roleInfo;
        }

    }
}
