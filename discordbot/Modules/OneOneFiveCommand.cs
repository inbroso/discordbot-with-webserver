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
    public class OneOneFiveCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private CommandHandler _handler;

        public OneOneFiveCommands(CommandHandler handler)
        {
            _handler = handler;
        }

        // Commands immer public machen!! :)

        //Test-Commands, um zu gucken ob der Bot funktioniert
        [SlashCommand("janeinfrage", "Stell hier eine Ja/Nein Frage!")]
        public async Task Test(string parameter)
        {
            var replies = new List<string>();

            var user = Context.User;

            replies.Add("JA");
            replies.Add("NEIN");

            var answer = replies[new Random().Next(replies.Count - 1)];

            await RespondAsync(Context.User.Mention + ": " + $"Deine Antwort auf die Frage (**{parameter}**) lautet: **{answer}**");
        }

        [SlashCommand("setbotstatus", "Setzt den Botstatus")]
        public async Task SetBotStatus(string status, ActivityType activityType, UserStatus userStatus)
        {
            var answer = await _handler.SetBotStatusAsync(status, activityType, userStatus);
            switch(answer)
            {
                case CommandHandler.Result.Success:
                    await RespondAsync(Context.User.Mention + ": " + $"Der Botstatus wurde erfolgreich geändert auf: \nAktivitätsstatus: {activityType.ToString()} - Status: {status} - UserStatus: {userStatus.ToString()}");
                    break;
                case CommandHandler.Result.Error:
                    await RespondAsync(Context.User.Mention + ": " + $"Der Botstatus konnte nicht geändert werden!");
                    break;
            }
        }

        [SlashCommand("ping", "TestCommand für API")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }


        [SlashCommand("kick", "Kickt einen User vom Gameserver!")]
        public async Task Kick(IGuildUser user, string reason)
        {
            //hier Logik für den Kick einbauen
        }

        [SlashCommand("ban", "Bannt einen User vom Gameserver!")]
        public async Task Ban(string userID, string reason, string time)
        {
            //hier Logik für den Ban einbauen
        }

        [SlashCommand("restart", "Restartet den Gameserver!")]
        public async Task RestartServer()
        {
            //hier Logik für den Server-Restart einbauen
        }

        [SlashCommand("giverole", "Gibt einem User eine Rolle")]
        public async Task GiveRole(IGuildUser user, IRole role)
        {
            try
            {
                CommandHandler.Result result = await _handler.GiveRoleToUserAsync(user, role);
                switch (result)
                {
                    case CommandHandler.Result.Success:
                        await RespondAsync(Context.User.Mention + ": " + $"Der User (**{user.Mention}**) bekam die Rolle: **{role.Mention}**");
                        break;
                    case CommandHandler.Result.RoleNotAllowed:
                        await RespondAsync(Context.User.Mention + ": " + $"Die Rolle **{role.Mention}** darf **nicht** vergeben werden!");
                        break;
                    case CommandHandler.Result.AlreadyHasRole:
                        await RespondAsync(Context.User.Mention + ": " + $"Der User (**{user.Mention}**) hat bereits die Rolle: **{role.Mention}**");
                        break;
                }
            }
            catch (Exception ex)
            {
                await RespondAsync("Fehler: " +  ex.Message );
            }
        }

        [SlashCommand("takerole", "Nimmt einem user eine Rolle")]
        public async Task TakeRole(IGuildUser user, IRole role)
        {
            try
            {
                CommandHandler.Result result = await _handler.TakeRoleFromUserAsync(user, role);
                switch (result)
                {
                    case CommandHandler.Result.Success:
                        await RespondAsync(Context.User.Mention + ": " + $"Der User (**{user.Mention}**) hat die Rolle **{role.Mention}** weggenommen bekommen");
                        break;
                    case CommandHandler.Result.DoesNotHaveRole:
                        await RespondAsync(Context.User.Mention + ": " + $"Der User (**{user.Mention}**) hat die Rolle **{role.Mention}** nicht");
                        break;
                }
            }
            catch (Exception ex)
            {
                await RespondAsync("Fehler: " + ex.Message);
            }
        }

        [SlashCommand("getroles", "Zeigt die Rollen an")]
        public async Task GetRoles(IGuildUser user)
        {
            try
            {
                String answer = Context.User.Mention + ": " + $"Der User (**{user.Mention}**) hat die Rollen:";
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
                await RespondAsync("Fehler: " + ex.Message);
            }
        }

        [SlashCommand("clearchannel", "EXPERIMENTELL: Löscht alle Nachrichten im Channel")]
        public async Task ClearChannel()
        {
            try
            {
                await RespondAsync("Wird ausgeführt...");
                var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
                foreach (IMessage message in messages)
                {
                    await message.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                await RespondAsync("Fehler: " + ex.Message);
            }
        }
    }
}