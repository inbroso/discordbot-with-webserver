using Microsoft.AspNetCore.Mvc;
using discordbot;
using Discord.WebSocket;
using Discord;
using discordbot.Modules;

namespace YourWebApiProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly DiscordBot _discordBot;

        public CommandsController(DiscordBot discordBot)
        {
            _discordBot = discordBot;
        }

        //curl -X POST https://localhost:50001/commands/ping
        [HttpPost("ping")]
        public async Task<IActionResult> ExecuteSlashCommand()
        {
            DiscordSocketClient client = _discordBot.GetDiscordSocketClient();
            ulong guildId = _discordBot.GetGuildId();
            var guild = client.GetGuild((ulong)1083143635547193425);

            await _discordBot._apiHandler.ProcessSlashCommandAsync(guild);

            //Antwort an den Client
            return Ok("Slash command executed successfully.");
        }

        //curl -X POST "https://localhost:50001/commands/giverole?userId= &roleId= "
        [HttpPost("giverole")]
        public async Task<IActionResult> GiveRoleCommand([FromQuery] ulong userId, [FromQuery] ulong roleId)
        {
            DiscordSocketClient client = _discordBot.GetDiscordSocketClient();
            ulong guildId = _discordBot.GetGuildId();
            var guild = client.GetGuild((ulong)1083143635547193425);

            await _discordBot._apiHandler.ProcessGiveRolesCommandAsync(guild, (ulong)userId, (ulong)roleId);

            return Ok("Give Role Command executed successfully.");
        }

        //curl -X POST "https://localhost:50001/commands/takerole?userId= &roleId= "
        [HttpPost("takerole")]
        public async Task<IActionResult> TakeRoleCommand([FromQuery] ulong userId, [FromQuery] ulong roleId)
        {
            DiscordSocketClient client = _discordBot.GetDiscordSocketClient();
            ulong guildId = _discordBot.GetGuildId();
            var guild = client.GetGuild((ulong)1083143635547193425);
            await _discordBot._apiHandler.ProcessTakeRolesCommandAsync(guild, (ulong)userId, (ulong)roleId);
            return Ok("Take Role Command executed successfully.");
        }

        //curl https://localhost:50001/commands/getserverroles - or go to the website
        //gets all Discord roles of the server - with name, color and roleId
        [HttpGet("getserverroles")]
        public async Task<IActionResult> GetServerRoles()
        {
            var roleInfo = await _discordBot._apiHandler.ProcessGetServerRolesCommandAsync();
            if (roleInfo == null)
                return NotFound("Error");
            return Ok(roleInfo);
        }

        [HttpGet("getuserroles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] ulong userId)
        {
            var roleInfo = await _discordBot._apiHandler.ProcessGetUserRolesCommandAsync(userId);
            if (roleInfo == null)
                return NotFound("Error");
            return Ok(roleInfo);
        }
    }
}
