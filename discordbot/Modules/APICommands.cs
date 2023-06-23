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

        // To test if the bot is working: Ping Pong Test!
        public async Task ProcessSlashCommandAsync(SocketGuild guild)
        {
            var channel = guild.TextChannels.FirstOrDefault(x => x.Id == botChannel);
            if (channel != null)
            {
                await channel.SendMessageAsync($"Ping-Pong! I'm ready!");
            }
        }

        /// <summary>
        /// Give a role to a user
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
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
                            await channel.SendMessageAsync(guild.Name + ": " + $"The user (**{user.Mention}**) received the role: **{role.Mention}**");
                        else if (result == CommandHandler.Result.RoleNotAllowed)
                            await channel.SendMessageAsync(guild.Name + ": " + $"The role **{role.Mention}** is **not** allowed to be given!");
                        else if (result == CommandHandler.Result.AlreadyHasRole)
                            await channel.SendMessageAsync(guild.Name + ": " + $"The user (**{user.Mention}**) already has the role: **{role.Mention}**");
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync("Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Take roles
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
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
                        await channel.SendMessageAsync(guild.Name + ": " + $"The user (**{user.Mention}**) has been taken away the role **{role.Mention}**");
                    else if (result == CommandHandler.Result.DoesNotHaveRole)
                        await channel.SendMessageAsync(guild.Name + ": " + $"The user (**{user.Mention}**) does not have the role **{role.Mention}**");
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync("Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Get all roles from a server
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get all roles from a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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
