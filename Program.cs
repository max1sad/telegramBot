using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

namespace FootballTelegramBot
{
    class Program
    {
        public static DBProvide dBProvide = new DBProvide();
        public static DBConnection connectsql = new DBConnection();
        static ITelegramBotClient bot = new TelegramBotClient("5444116745:AAEiHTg9bpEQLHA7sEGj_c7SpCTg6AXMVPY");
        //результат запроса к БД
        public static string resultSql = "";
        //строка запроса пишется
        public static string sqlStr = "";
        //метка, которая устнавливает числовое знгачение , определяет этапы диалогов для разных веток общения.
        //от 1-3 это этапы создания заявки на участие в турнире
        //4-5  это запрос на вывод турнирной таблицы.
        public static int levelAplly = 0;
        //метка на проверку была ли нажата кнопка в начале действий
        static bool labelClick = false;
        //название кнопки на которую совершили нажатие
        public static string actionChoice = "";       
        //фомируются исодные данные для принятия заявки на турнир
        public static string applicationTournament = "";
        public static string applicationChek = "";
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {
            StringParcers stringParcers = new StringParcers();
            var message = update.Message;
            //результат ответа от телеграмма выводится
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //бот случает чат телеграмма на новые сообщения
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {               
                if (message.Text == "/start")
                {
                    //очищаем все переменные, бот работает всегда, следовательно если новые действия будут, что бы не вело в заблуждение.
                    labelClick = true;
                    actionChoice = "";
                    sqlStr = "";
                    applicationChek = "";
                    levelAplly = 0;
                    applicationTournament = "";
                    await botClient.SendTextMessageAsync(message.Chat.Id,"Привет человек");
                    //вывод кнопок для совершения действий
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выбири действия", replyMarkup: buttonTest());
                    return;
                }

                //проверка что после нажатия кнопки было совершон ответ
                switch (levelAplly)
                {                                           
                    case 1:
                        Console.WriteLine(actionChoice);
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            applicationTournament = message.Text + ";";
                            //запрос на список турниров для участия.
                            resultSql = string.Join("\n", dBProvide.GetAllTourney().Select(l => l.idNameTourney +"-"+ l.nameTourney));                           
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, в котором планируешь учавствовать");
                            //вывод пользователю список турниров с номерами
                            await botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            //Console.WriteLine(applicationTournament);
                            actionChoice = "";
                            levelAplly = 2;
                            applicationChek = "";
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 1;
                        }                        
                        return;
                    case 2:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            //строка в котрой имеется номер лиги и номер турнира
                            applicationTournament += message.Text + ";";
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Введити данные о команде в формате \"Название команды:Игрок1;Игрок2;\" и т.д. Заканчивается знаком \";\"");
                            Console.WriteLine(applicationTournament);
                            levelAplly = 3;
                            applicationChek = "";
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 2;
                        }                       
                        return;
                    case 3:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            string idPlayers = "";
                            List<string> stringArray = new List<string>();
                            applicationTournament += message.Text;
                            Console.WriteLine(applicationTournament);
                            //вызываем метод для обработки строки в нужный формат.
                            stringArray = stringParcers.sqlParcerApply(applicationTournament);
                            Console.WriteLine(message.Chat.Id);                                                        
                            //добавляем команду в таблицу
                            dBProvide.CreateTeams(new Model.Team(){nameTeams = stringArray[2]});
                            string idTeams = string.Join("\n", dBProvide.GetAllTeams().Where(l => l.nameTeams == stringArray[2]).Select(l => l.idTeams));
                            //запрос с номером 100- это вывод 1-го поля с типом числовым.
                            // добавляем команду в турнир
                            dBProvide.CreateTourneyTeams(new Model.TourneyTeams() { idLeague = int.Parse(stringArray[0]),idNameTourney = int.Parse(stringArray[1]),idTeams = int.Parse(idTeams) });
                            //добавялем всех игроков в таблицу игроки и первым запросом считаекм что капитан подал заявку и его номер пишем тоже.
                            dBProvide.CreatePlayerCapitan(new Model.Player() { FIO = stringArray[3], idPhone = message.Chat.Id.ToString() });
                            idPlayers = string.Join("\n", dBProvide.GetAllPlayer().Where(l => l.FIO == stringArray[3]).Select(l => l.idPlayer));
                            Console.WriteLine(idPlayers);
                            dBProvide.CreatePlayerTeamsCapitan(new Model.PlayerTeams() { idTeams = int.Parse(idTeams), teamCaptain = "yes", idPlayer = int.Parse(idPlayers) });
                            for (int i = 4; i < stringArray.Count; i++)
                            {
                                dBProvide.CreatePlayer(new Model.Player() { FIO = stringArray[i] });
                                idPlayers = string.Join("\n", dBProvide.GetAllPlayer().Where(l => l.FIO == stringArray[i]).Select(l => l.idPlayer));
                                dBProvide.CreatePlayerTeams(new Model.PlayerTeams() { idTeams = int.Parse(idTeams), idPlayer = int.Parse(idPlayers) });
                            }

                            //написать запрос добавления игрока в команду , id команды уже получал в переменную str;
                            Console.WriteLine(sqlStr);

                            await botClient.SendTextMessageAsync(message.Chat.Id, "Спасибо, заявка будет рассмотренна в ближайшее время");
                            levelAplly = 0;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Не соответствует введенному формату, повторите ввод(в именах только русские буквы)");
                            levelAplly = 3;
                        }                        
                        return;
                    case 4:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, 1) == true)
                        {
                            applicationTournament = message.Text + ";";
                            sqlStr = "select idNameTourney,nameTourney from nameTourney where statusToutney is null";
                            resultSql = string.Join("\n", dBProvide.GetAllTourney().Select(l => l.idNameTourney + "-" + l.nameTourney));
                            //actionChoice = "apply1";
                            resultSql = connectsql.SqlRead(sqlStr, 1);
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, для просмотра турнирной таблицы");
                            //вывод пользователю список турниров с номерами
                            await botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            //Console.WriteLine(applicationTournament);
                            actionChoice = "";
                            levelAplly = 5;
                            applicationChek = "";
                            //levelAplly1 = true;
                            sqlStr = "";
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 4;
                        }
                        return;
                    case 5:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, 1) == true)
                        {
                            applicationTournament += message.Text + ";";
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Запрос к базе на вывод турнирной таблицы");
                            Console.WriteLine(applicationTournament);
                            levelAplly = 0;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 5;
                        }
                        return;
                    default:
                        Console.WriteLine("Не указали параметр");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала работы напиши \"/start\"");
                        return;
                }              
                //await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала работы напиши \"/start\"");                
            }
            Console.WriteLine(labelClick);
            //проверка на какую кнопку нажали, происходит формирвоание наименования кнопки, а так же в переменную levlApply указывается номер по которому проверка в какой диалог входить.
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
                        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery & actionChoice == "tourneyTable")
                        {
                            await bot.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, "Выбери лигу, указав номер");
                            //вывод списка лиг из бд
                            //sqlStr = "SELECT nameLeague FROM league";
                            //вызом метода для вывода из данных из БД. передача двух параметров. строка запроса и название кнопки на которую нажали
                            resultSql = string.Join("\n", dBProvide.GetAllLeague().Select(l => $"{l.nameLeague} -Лига"));
                            await bot.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, resultSql);
                            levelAplly = 4;
                            //Console.WriteLine(s);
                        }
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
                            //вызом метода для вывода из данных из БД
                            resultSql = string.Join("\n", dBProvide.GetAllLeague().Select(l => $"{l.nameLeague} -Лига")) ;
                            await bot.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, resultSql);
                            levelAplly = 1;
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
            //connectsql.OpenConnect();
           var dat =  "05-08-22 18:55";
            DateTime dar = DateTime.Parse(dat);
            Console.WriteLine(dar);
            //string rez = 
            //Console.WriteLine(rez);
            //Console.WriteLine(dBProvide.GetAllPlayer());
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
