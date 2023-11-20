using Discord_Bot.Engine.LevelSystem;
using Discord_Bot.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
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

        [Command("profile")]
        public async Task ProfileCommand(CommandContext ctx)
        {
            string username = ctx.User.Username;
            ulong guildID = ctx.Guild.Id;

            var userDetails = new DUser()
            {
                Username = ctx.User.Username,
                guildID = ctx.Guild.Id,
                avatarURL = ctx.User.AvatarUrl,
                Level = 1,
                XP = 0
            };

            var levelEngine = new LevelEngine();
            var doesExist = levelEngine.CheckUserExists(username,guildID);

            if(doesExist == false)
            {
               var isStored = levelEngine.StoreUserDetails(userDetails);

                if(isStored)
                {
                    var successMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Successfully created profile",
                        Description = "Please execute profile again to view your profile",
                        Color = DiscordColor.Green
                    };

                    await ctx.Channel.SendMessageAsync(embed: successMessage);

                    var pulledUser = levelEngine.GetUser(username, guildID);

                    var profile = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Blue)
                            .WithTitle(pulledUser.Username + "'s Profile")
                            .WithThumbnail(pulledUser.avatarURL)
                            .AddField("Level", pulledUser.Level.ToString(), true)
                            .AddField("XP", pulledUser.XP.ToString(), true)
                            );

                    await ctx.Channel.SendMessageAsync(profile);
                }
                else
                {
                    var failedMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Something went wrong when creating your profile",
                        Color = DiscordColor.Red
                    };

                    await ctx.Channel.SendMessageAsync(embed: failedMessage);
                }
            }
            else
            {
                var pulledUser = levelEngine.GetUser(username,guildID);

                var profile = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Blue)
                .WithTitle(pulledUser.Username + "'s Profile")
                .WithThumbnail(pulledUser.avatarURL)
                .AddField("Level", pulledUser.Level.ToString(), true)
                .AddField("XP", pulledUser.XP.ToString(), true)
                );

                await ctx.Channel.SendMessageAsync(profile);
            }
        }
    }
}
