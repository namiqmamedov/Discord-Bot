using Discord_Bot.commands;
using Discord_Bot.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {
            var jsonReader = new JsonReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;
            Client.MessageCreated += Client_MessageCreated;
            Client.VoiceStateUpdated += VoiceChannelHandler;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.CommandErrored += Commands_CommandErrored;

            Commands.RegisterCommands<WrapperCommand>();

            await Client.ConnectAsync();
            await Task.Delay(-1); // working infinite 
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if(e.Exception is ChecksFailedException exception)
            {
                string timeLeft = string.Empty;

                foreach (var check in exception.FailedChecks)
                {
                    var cooldown = (CooldownAttribute)check;
                    timeLeft = cooldown.GetRemainingCooldown(e.Context).ToString(@"hh\:mm\:ss");
                }

                var coolDownMessage = new DiscordEmbedBuilder
                {
                     Color = DiscordColor.Red,
                     Title = "Please wait for the cooldown to end",
                     Description = $"Time : {timeLeft}"
                };

                await e.Context.Channel.SendMessageAsync(embed: coolDownMessage);
            }
        }

        private static async Task VoiceChannelHandler(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if(e.Before == null && e.Channel.Name == "Create")
            {
                await e.Channel.SendMessageAsync($"{e.User.Username} joined the channel");
            }
        }

        private static Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
