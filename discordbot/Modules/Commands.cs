using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using discordbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.Modules
{
    public class Commands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService _Commands { get; set; }
        private CommandHandler _handler;

        public Commands(CommandHandler handler)
        {
            _handler = handler;
        }


        // Commands have to be public!! :)

        //Test-Commands
        [SlashCommand("yesno", "Yes/No Question!")]
        public async Task Test(string parameter)
        {
            var replies = new List<string>();

            var user = Context.User;

            replies.Add("YES");
            replies.Add("NO");

            var answer = replies[new Random().Next(replies.Count - 1)];

            await RespondAsync(Context.User.Mention + ": " + $"The answer to your question (**{parameter}**) is: **{answer}**");
        }

        [SlashCommand("ping", "Ping-Pong Test!")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [SlashCommand("setbotstatus", "Sets bot status")]
        public async Task SetBotStatus(string status, ActivityType activityType, UserStatus userStatus)
        {
            var answer = await _handler.SetBotStatusAsync(status, activityType, userStatus);
            switch(answer)
            {
                case CommandHandler.Result.Success:
                    await RespondAsync(Context.User.Mention + ": " + $"Successfully changed bot status to: \nActivitytype: {activityType.ToString()} - Status: {status} - UserStatus: {userStatus.ToString()}");
                    break;
                case CommandHandler.Result.Error:
                    await RespondAsync(Context.User.Mention + ": " + $"The Bot Status could not be set!");
                    break;
            }
        }

        // These are example commands...
        [SlashCommand("kick", "Kicks a user from the game server!")]
        public async Task Kick(IGuildUser user, string reason)
        {
            //implement yourself
        }

        [SlashCommand("ban", "Bans a user from the game server!")]
        public async Task Ban(string userID, string reason, string time)
        {
            //implement yourself
        }

        [SlashCommand("restart", "Restarts game server!")]
        public async Task RestartServer()
        {
            //implement yourself
        }
        
        [SlashCommand("giverole", "Gives user a role")]
        public async Task GiveRole(IGuildUser user, IRole role)
        {
            try
            {
                CommandHandler.Result result = await _handler.GiveRoleToUserAsync(user, role);
                switch (result)
                {
                    case CommandHandler.Result.Success:
                        await RespondAsync(Context.User.Mention + ": " + $"The user (**{user.Mention}**) received the role: **{role.Mention}**");
                        break;
                    case CommandHandler.Result.RoleNotAllowed:
                        await RespondAsync(Context.User.Mention + ": " + $"The role **{role.Mention}** is **not** allowed to be given!");
                        break;
                    case CommandHandler.Result.AlreadyHasRole:
                        await RespondAsync(Context.User.Mention + ": " + $"The user (**{user.Mention}**) already has the role: **{role.Mention}**");
                        break;
                }
            }
            catch (Exception ex)
            {
                await RespondAsync("Error: " +  ex.Message );
            }
        }

        [SlashCommand("takerole", "Takes role from a user")]
        public async Task TakeRole(IGuildUser user, IRole role)
        {
            try
            {
                CommandHandler.Result result = await _handler.TakeRoleFromUserAsync(user, role);
                switch (result)
                {
                    case CommandHandler.Result.Success:
                        await RespondAsync(Context.User.Mention + ": " + $"The user (**{user.Mention}**) has been taken away the role **{role.Mention}**");
                        break;
                    case CommandHandler.Result.DoesNotHaveRole:
                        await RespondAsync(Context.User.Mention + ": " + $"The user (**{user.Mention}**) does not have the role **{role.Mention}**");
                        break;
                }
            }
            catch (Exception ex)
            {
                await RespondAsync("Error: " + ex.Message);
            }
        }

        [SlashCommand("getroles", "Shows users roles")]
        public async Task GetRoles(IGuildUser user)
        {
            try
            {
                String answer = Context.User.Mention + ": " + $"The user (**{user.Mention}**) has the roles:";
                IReadOnlyCollection<SocketRole> _roles = await _handler.GetUserRolesAsync(user);
                foreach (SocketRole i in _roles)
                {
                    if(!i.IsEveryone)
                        answer = answer + $"**{i.Mention}** ";
                }

                await RespondAsync(answer);
            }
            catch (Exception ex)
            {
                await RespondAsync("Error: " + ex.Message);
            }
        }


        // EXPERIMENTAL: Does not work correctly/uses the Discord API too much...
        //[SlashCommand("clearchannel", "EXPERIMENTAL: Clears all messages in the channel")]
        //public async Task ClearChannel()
        //{
        //    try
        //    {
        //        await RespondAsync("Wird ausgeführt...");
        //        var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
        //        foreach (IMessage message in messages)
        //        {
        //            await message.DeleteAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await RespondAsync("Error: " + ex.Message);
        //    }
        //}
    }
}