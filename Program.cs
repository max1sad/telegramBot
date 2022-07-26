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
        static bool labelClick = false;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {   string actionChoice = "";
            
            var message = update.Message;
            //результат ответа от телеграмма выводится
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //бот случает чат телеграмма на новые сообщения
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                
                //переменная где хранится информация об сообщении.которое поступило в чат
                //var message = update.Message;
                //сообщение которое приходит от бота 
                //await botClient.SendTextMessageAsync(message.Chat,message.Text);
                if (message.Text == "/start")
                {
                     labelClick = true;
                    //сообщение от бота с выделением конкретного сообщения, кто отправил
                    //await botClient.SendTextMessageAsync(message.Chat.Id, message.Text,replyToMessageId: message.MessageId);
                    //создание кнопки в телеграмме
                    await botClient.SendTextMessageAsync(message.Chat.Id,"Привет человек");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выбири действия", replyMarkup: buttonTest());
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала работы напиши \"/start\"");
            }
            Console.WriteLine(labelClick);
            //проверка на какую кнопку нажали
            if (labelClick == true)
            {
                //Console.WriteLine("Не заходит в условие");
                labelClick = false;
                CallbackQuery callback = update.CallbackQuery;
                //проверка было ли какое то действие связнное с CallbackQuery, проверка на какую кнопку нажато- 
                var updateCallbackQuery = update.CallbackQuery;
                switch (callback.Data)
                {
                    case "tourneyTable":
                        Console.WriteLine("кнопка нажата");
                        actionChoice = "tourneyTable";
                        break;
                    case "gameSchedule":
                        Console.WriteLine("кнопка нажата1");
                        actionChoice = "gameSchedule";
                        break;
                    case "apply":
                        Console.WriteLine("кнопка нажата2");
                        actionChoice = "apply";
                        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery & actionChoice == "apply")
                        {
                            await bot.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, "Выбери лигу, указав номер");
                            //вывод списка лиг из бд
                            Console.WriteLine(actionChoice);
                        }
                        break;
                }

                
            }
            
            
           
        }
       
        //кнопки для стартового выбора действий 
        private static IReplyMarkup buttonTest()
        {
            var ikm = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Турнирная таблица", "tourneyTable"),
                    InlineKeyboardButton.WithCallbackData("Расписание игр", "gameSchedule"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Подать заявку", "apply"),
                },
            });
            return ikm;
        }

        
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        
        static void Main(string[] args)
        {
            DBConnection connectsql = new DBConnection();
            //connectsql.OpenConnect();
            String sqlstr = "SELECT nameLeague FROM league";
            connectsql.SqlRead(sqlstr);
            //блок запуска бота и методов проверки сообщений и обработки ошибок
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
