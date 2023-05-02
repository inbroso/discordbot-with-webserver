using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(typeof(DiscordBot).Assembly, _services);

            // process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;

            // process the command execution results 
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }

        public async Task<Result> GiveRoleToUserAsync(IGuildUser user, IRole role)
        {
            if (user.RoleIds.Contains(role.Id))
                return Result.AlreadyHasRole;
            if (role.Name.Contains("root"))
                return Result.RoleNotAllowed;

            await user.AddRoleAsync(role);
            return Result.Success;
        }

        public async Task<Result> TakeRoleFromUserAsync(IGuildUser user, IRole role)
        {
            if (user.RoleIds.Contains(role.Id))
            {
                await user.RemoveRoleAsync(role);
                return Result.Success;
            }
            else
                return Result.DoesNotHaveRole;
        }

        public async Task<IReadOnlyCollection<SocketRole>> GetUserRolesAsync(IGuildUser user)
        {
            var _user = (SocketGuildUser)user;
            IReadOnlyCollection<SocketRole> _roles = _user.Roles;
            return _roles;
        }

        public async Task<Result> SetBotStatusAsync(string status, ActivityType activityType, UserStatus userStatus)
        {
            try
            {
                await _client.SetStatusAsync(userStatus);
                await _client.SetGameAsync(status, null, activityType);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Error;
            }
        }

        public enum Result : byte
        {
            Success,
            //UserNotFound, <- für später falls benötigt
            //RoleNotFound, <- für später falls benötigt
            RoleNotAllowed,
            AlreadyHasRole,
            DoesNotHaveRole,
            Error
        }

        public class Role
        {
            public Role(string roleName, ulong roleId)
            {
                this.roleName = roleName;
                this.roleId = roleId;
            }

            public string roleName { get; set; }
            public ulong roleId { get; set; }
        }
    }
}