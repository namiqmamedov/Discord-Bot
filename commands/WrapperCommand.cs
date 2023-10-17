using Discord_Bot.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Discord_Bot.commands
{
    public class WrapperCommand : BaseCommandModule
    {
        [Command("test")]
        public async Task Command(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello {ctx.User.Username}");
        }

        [Command("embed")]
        public async Task EmbedMessage(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = "This is my first Discord Embed",
                Description = $"This comamnd was executed by {ctx.User.Username}",
                Color = DiscordColor.SpringGreen
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("cardgame")]
        public async Task CardGame(CommandContext ctx)
        {
            var userCard = new CardSystem();

            var userCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"Your card is {userCard.SelectedCard}",
                Color = DiscordColor.Lilac
            };

            await ctx.Channel.SendMessageAsync(embed: userCardEmbed);

            var botCard = new CardSystem();

            var botCardEmbed = new DiscordEmbedBuilder
            {
               Title = $"The bot drew a {botCard.SelectedCard}",
               Color = DiscordColor.Orange
            };

            await ctx.Channel.SendMessageAsync(embed: botCardEmbed);

            if(userCard.SelectedNumber > botCard.SelectedNumber)
            {
                var winMessage = new DiscordEmbedBuilder
                {
                    Title = "Congratulations, You Win!!!",
                    Color = DiscordColor.Green
                };

                await ctx.Channel.SendMessageAsync(embed: winMessage);
            }
            else
            {
                var loseMessage = new DiscordEmbedBuilder
                {
                    Title = "You lost the game.",
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: loseMessage);
            }
        }

        [Command("activity")]
        public async Task HelloCommand(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();
                 
            var messageToRetrieve = await interactivity.WaitForMessageAsync(message => message.Content == "Hello");
            
            if(messageToRetrieve.Result.Content == "Hello")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} said Hello");
            }
        }

        [Command("poll")]
        public async Task ReactContent(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(10);


            DiscordEmoji[] emojiOptions = { DiscordEmoji.FromName(Program.Client, ":one:"),
                                            DiscordEmoji.FromName(Program.Client, ":two:"),
                                            DiscordEmoji.FromName(Program.Client, ":three:"),
                                            DiscordEmoji.FromName(Program.Client, ":four:") };

            string optionsDescription =  $"{emojiOptions[0]} | {option1} \n" +
                                         $"{emojiOptions[0]} | {option2} \n" +
                                         $"{emojiOptions[0]} | {option3} \n" +
                                         $"{emojiOptions[0]} | {option4}";

            var pollMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = pollTitle,
                Description = optionsDescription,
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(embed: pollMessage);

            foreach (var emoji in emojiOptions)
            {
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll,pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if(emoji.Emoji == emojiOptions[0])
                {
                    count1++;
                }   
                if(emoji.Emoji == emojiOptions[1])
                {
                    count2++;
                }          
                if(emoji.Emoji == emojiOptions[2])
                {
                    count3++;
                }    
                if(emoji.Emoji == emojiOptions[3])
                {
                    count4 ++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;
            string resultsDescription =  $"{emojiOptions[0]}: {count1} Votes \n" +
                                         $"{emojiOptions[1]}: {count2} Votes \n" +
                                         $"{emojiOptions[2]}: {count3} Votes \n" +
                                         $"{emojiOptions[3]}: {count4} Votes \n\n" +
                                         $"Total Votes: {totalVotes}";

            var resultsEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = "Results of the Poll",
                Description = resultsDescription
            };

            await ctx.Channel.SendMessageAsync(embed: resultsEmbed);
        }

        [Command("text")]
        [Cooldown(5, 10, CooldownBucketType.User)]
        public async Task TextCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Text Message");
        }
    }
}
