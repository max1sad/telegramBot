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
        public static DBConnection connectsql = new DBConnection();
        static ITelegramBotClient bot = new TelegramBotClient("5444116745:AAEiHTg9bpEQLHA7sEGj_c7SpCTg6AXMVPY");
        //результат запроса к БД
        public static string resultSql = "";
        //строка запроса пишется
        public static string sqlStr = "";
        //метка на проверку была ли нажата кнопка в начале действий
        static bool labelClick = false;
        //название кнопки на которую совершили нажатие
        public static string actionChoice = "";
        //проверка сообщения на заявку при выборе какой турнир
        static bool levelAplly1 = false;
       // public static bool apply
        //фомируются исодные данные для принятия заявки на турнир
        public static string applicationTournament = "";
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {   
            
            var message = update.Message;
            //результат ответа от телеграмма выводится
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //бот случает чат телеграмма на новые сообщения
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {               
                if (message.Text == "/start")
                {
                     labelClick = true;
                    //сообщение от бота с выделением конкретного сообщения, кто отправил
                    //await botClient.SendTextMessageAsync(message.Chat.Id, message.Text,replyToMessageId: message.MessageId);
                    //создание кнопки в телеграмме
                    await botClient.SendTextMessageAsync(message.Chat.Id,"Привет человек");
                    //вывод кнопок для совершения действий
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выбири действия", replyMarkup: buttonTest());
                    return;
                }
                //проверка что после нажатия кнопки было совершон ответ
                if (actionChoice == "apply")
                {
                    applicationTournament = message.Text + ";";
                    sqlStr = "select idNameTourney,nameTourney from nameTourney where statusToutney is null";
                    //запрос на список турниров для участия.
                    actionChoice = "apply1";
                    resultSql = connectsql.SqlRead(sqlStr, actionChoice);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, в котором планируешь учавствовать");
                    //вывод пользователю список турниров с номерами
                    await botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                    //Console.WriteLine(applicationTournament);
                    actionChoice = "";
                    levelAplly1 = true;
                    sqlStr = "";
                    return;
                }
                //проверка что идет выбор турнира в котором будут учавствовать команда
                if (levelAplly1 == true)
                {
                    //строка в котрой имеется номер лиги и номер турнира
                    applicationTournament += message.Text + ";";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введити данные о команде в формате \"Название команды:Игрок1;Игрок2;\" и т.д. Заканчивается знаком \";\"");
                    Console.WriteLine(applicationTournament);
                    levelAplly1 = false;
                    return;
                }
                //дальше нужно считать сообщение с названием команды и списком игроков, 
                //
                //
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
                            sqlStr = "SELECT nameLeague FROM league";
                            //вызом метода для вывода из данных из БД. передача двух параметров. строка запроса и название кнопки на которую нажали
                            resultSql = connectsql.SqlRead(sqlStr, actionChoice);
                            await bot.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, resultSql);

                            //Console.WriteLine(s);
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
            //вызывает соединение с базой данных
            connectsql.OpenConnect();
            
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
