using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace FootballTelegramBot
{
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5444116745:AAEiHTg9bpEQLHA7sEGj_c7SpCTg6AXMVPY");
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {   string actionChoice = "";
            var message = update.Message;
            //await botClient.SendTextMessageAsync(message.Chat.Id, "П");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                
                //переменная где хранится информация об сообщении.которое поступило в чат
                //var message = update.Message;
                //сообщение которое приходит от бота 
                //await botClient.SendTextMessageAsync(message.Chat,message.Text);
                if (message.Text != null)
                {
                    //сообщение от бота с выделением конкретного сообщения, кто отправил
                    //await botClient.SendTextMessageAsync(message.Chat.Id, message.Text,replyToMessageId: message.MessageId);
                    //создание кнопки в телеграмме
                   
                    await botClient.SendTextMessageAsync(message.Chat.Id,message.Text,replyMarkup: buttonTest());
                    
                    //Console.WriteLine(message.Contact.PhoneNumber);
                    return;
                   
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет");
            }
           //проверка на какую кнопку нажали
            CallbackQuery callback = update.CallbackQuery;
            switch (callback.Data)
            {
                case "myCommand1":
                    Console.WriteLine("кнопка нажата");
                    actionChoice = "myCommand1";
                    break;
                case "myCommand2":
                    Console.WriteLine("кнопка нажата1");
                    actionChoice = "myCommand2";
                    break;
                case "myCommand3":
                    Console.WriteLine("кнопка нажата2");
                    actionChoice = "myCommand3";
                    break;
            }
            //проверка было ли какое то действие связнное с CallbackQuery, проверка на какую кнопку нажато
            var updateCallbackQuery = update.CallbackQuery;
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                if (actionChoice == "myCommand1")
                {
                    await botClient.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id,actionChoice);
                }
            }
            
            /*if (callback.Data == "myCommand1")
            {
                Console.WriteLine("кнопка нажата");
                //await botClient.SendTextMessageAsync(message.Chat, "ffdfdgdgf");
                //return;

            }*/
            //Console.WriteLine(update.CallbackQuery.Data);


        }
       
        //кнопки формируются тут 
        private static IReplyMarkup buttonTest()
        {
            var ikm = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("создать", "myCommand1"),
                    InlineKeyboardButton.WithCallbackData("скрыть1", "myCommand2"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("скрыть", "myCommand3"),
                },
            });
            return ikm;
        }

        //метод создания кнопики

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
        }
    }

}
