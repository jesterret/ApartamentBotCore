using ApartamentBotCore.WebModules;
using ApartamentBotCore.WebModules.OfferData;
using ApartamentBotCore.WebModules.SearchProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ApartamentBotCore
{
    class TelegramManager : IDisposable
    {
        internal Telegram.Bot.Types.User BotInfo = null;
        TelegramBotClient bot = new TelegramBotClient(Program.TelegramApiKey);
        bool bShouldIgnore = false;
        OfferBase CurrentOffer = null;
        IEnumerable<OfferBase> Offers = Enumerable.Empty<OfferBase>();

        public TelegramManager()
        {
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            bot.OnReceiveError += Bot_OnReceiveError;
            bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;

            BotInfo = bot.GetMeAsync().GetAwaiter().GetResult();
            bot.StartReceiving(new UpdateType[] { UpdateType.CallbackQuery, UpdateType.Message });
        }
        private void Bot_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e)
        {
            if (!(e.Exception is System.Net.Http.HttpRequestException))
            {
                bot.SendTextMessageAsync(Program.PrivateChatID, e.Exception.ToString());
                bShouldIgnore = true;
            }
        }
        private void Bot_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e) 
            => bot.SendTextMessageAsync(Program.PrivateChatID, e.ApiRequestException.ToString());

        private async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }

            var callback = e.CallbackQuery;
            await bot.AnswerCallbackQueryAsync(callback.Id);
            if (OfferDispatcher.GetSearch(callback.Data) is SearchBase search)
            {
                var sent = await bot.SendTextMessageAsync(callback.Message.Chat.Id, "Please wait, processing...");
                Offers = search.GetLoaded()
                    .Where(x => x.Space > 15)
                    .Distinct()
                    .OrderBy(x => x.Price)
                    .ThenByDescending(x => x.Space)
                    .ToList() // Force load
                    ;
                CurrentOffer = Offers.First();
                await bot.DeleteMessageAsync(sent.Chat.Id, sent.MessageId);
                SetOffer(callback.Message);
            }
            else if(callback.Data == "NextOffer")
            {
                Offers = Offers.Skip(1);
                CurrentOffer = Offers.FirstOrDefault();
                if (CurrentOffer is null)
                    await bot.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId, "Hot damn, you just ran out of searches...");
                else
                    SetOffer(callback.Message);

            }
            else if(callback.Data == "MapDist")
            {
                if (!(CurrentOffer is null || CurrentOffer.Location is null))
                {
                    if (CurrentOffer.Location.GetData() is MapLocation.DistanceInfo info)
                        await bot.SendTextMessageAsync(callback.Message.Chat.Id, $"Distance: {info.Distance}\nDuration: {info.Duration}");
                }
            }
            else if(callback.Data == "SaveOffer" && !(CurrentOffer is null))
            {
                var path = Path.Combine(Program.GetDirectory(), "saved.log");
                File.AppendAllText(path, CurrentOffer.Url.ToString() + Environment.NewLine);
            }
        }

        private async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }
            var msg = e.Message;
            if (msg.EntityValues != null && msg.Entities != null)
            {
                var entVals = msg.EntityValues.ToList();
                foreach (var i in Enumerable.Range(0, msg.Entities.Count()))
                {
                    var ent = entVals[i];
                    var entity = msg.Entities[i];
                    if (entity.Type == MessageEntityType.BotCommand)
                    {
                        switch (ent)
                        {
                            case "/start":
                            case "/search":
                                await bot.SendTextMessageAsync(msg.Chat.Id, "Select the site you want to query.", replyMarkup:
                                    new InlineKeyboardMarkup(OfferDispatcher.Searches.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Key, s.Key) })));
                                break;
                            case "/list":
                                var path = Path.Combine(Program.GetDirectory(), "saved.log");
                                if (File.Exists(path))
                                    await bot.SendTextMessageAsync(msg.Chat.Id, string.Join(Environment.NewLine, File.ReadAllLines(path)));

                                break;

                            case "/hcf":
                                if (msg.Chat.Id == Program.PrivateChatID)
                                {
                                    await bot.SendTextMessageAsync(Program.PrivateChatID, "Quitting");
                                    Program.ShouldQuit.Set();
                                }
                                break;
                        }
                    }
                    else if (entity.Type == MessageEntityType.Url)
                    {
                        var text = msg.Text.Substring(entity.Offset, entity.Length);
                        if (await OfferDispatcher.GetOfferLoaded(text) is OfferBase offer)
                        {
                            CurrentOffer = offer;
                            await bot.SendTextMessageAsync(msg.Chat.Id, offer.GetHtml(), ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData("Get distance", "MapDist")
                                }));
                        }
                        else
                            await bot.SendTextMessageAsync(msg.Chat.Id, "Not supported offer.");
                    }
                }
            }
        }

        private void SetOffer(in Telegram.Bot.Types.Message msg)
        {
            bot.EditMessageTextAsync(msg.Chat.Id, msg.MessageId, CurrentOffer.GetHtml(), ParseMode.Html, replyMarkup: 
            new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl("Open", CurrentOffer.Url.ToString())
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Get distance", "MapDist")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Next", "NextOffer"),
                    InlineKeyboardButton.WithCallbackData("Save", "SaveOffer"),
                }
            }));
        }

        public void Dispose() => bot.StopReceiving();

        private string BotUsername => BotInfo.Username;
    }
}
