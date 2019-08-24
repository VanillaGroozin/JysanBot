using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using JysanBot.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using JysanBot.Services.Navigation;
using JysanBot.DTOs;
using Telegram.Bot.Types;

namespace JysanBot
{
    class Program
    {
        private static readonly TelegramBotClient _telegramBot;
        private static readonly InsuranceService _insuranceService;
        private static NavigationService _navigationService;
        private static DTOs.User _currentUser;

        static void Main(string[] args)
        {
            _telegramBot.OnMessage += BotOnMessageReceived;
            _telegramBot.OnMessageEdited += BotOnMessageReceived;
            _telegramBot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _telegramBot.OnInlineQuery += BotOnInlineQueryReceived;
            _telegramBot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            _telegramBot.OnReceiveError += BotOnReceiveError;

            _telegramBot.StartReceiving(Array.Empty<UpdateType>());

            Console.WriteLine($"[Deployment of " +
                $"{_telegramBot.BotId} completed without errors at " +
                $"{DateTime.Now.ToShortTimeString()}]");

            Console.ReadLine();
            _telegramBot.StopReceiving();

            _telegramBot.StartReceiving();
            Thread.Sleep(int.MaxValue);
            
            Console.ReadLine();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {

            var messageBody = messageEventArgs.Message;
            string responseMessage = String.Empty;
            Console.WriteLine($"Received inline query from: {messageEventArgs.Message.Location}, {messageEventArgs.Message.Contact}");
            ReplyKeyboardMarkup ReplyKeyboard = new ReplyKeyboardMarkup { };
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup (new[] { InlineKeyboardButton.WithCallbackData(""),});
            var user = _insuranceService.GetUserInfo(messageEventArgs.Message.From.Id);
            var DTP = _insuranceService.GetDtpInfo(messageEventArgs.Message.From.Id);

            if (messageBody != null)
            {
                switch (messageBody as object)
                {
                    case Message l when l.Location != null:
                     
                        DTP.Location = l.Location;
                        user.DTPs = DTP;
                        _insuranceService.UserUpdate(user);

                        inlineKeyboard = _navigationService.CreateInlineKeyboard("Продолжить...|");
                        await _telegramBot.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Location added", replyMarkup: inlineKeyboard);                        

                        break;

                    case Message c when c.Contact != null:

                        user.Contact = c.Contact;
                        _insuranceService.UserUpdate(user);

                        inlineKeyboard = _navigationService.CreateInlineKeyboard("Продолжить...|");
                        await _telegramBot.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Contact added", replyMarkup: inlineKeyboard);
                        break;

                    case Message s when s.Text == "/start":
                        responseMessage = _insuranceService.GenerateHelloMessage(
                            messageEventArgs.Message.From.Username,
                            messageEventArgs.Message.From.Id);

                        await _telegramBot.SendTextMessageAsync(
                            chatId: messageEventArgs.Message.Chat,
                            text: responseMessage);

                        inlineKeyboard = _navigationService.CreateInlineKeyboard("Купить\\SOS ДТП|Связаться с оператором\\Еще что-то...|");

                        await _telegramBot.SendTextMessageAsync(
                        messageEventArgs.Message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
   
                        
                        EnvironmentVariables.ShowLastPrintedMessage = false;
                        break;

                    case Message s when _navigationService.IsStringIIN(s.Text):                  
                        await _navigationService.NavigateTo(s.Text, s.Chat.Id, s.From.Id, _telegramBot);
                        break;

                    case Message s when s.Text.Split(' ').Length == 3:
                        await _navigationService.NavigateTo(s.Text, s.Chat.Id, s.From.Id, _telegramBot);
                        break;


                    #region  insApplication   

                    case string s when s == "Оформить договор страхования":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        responseMessage = "Выберете интересующий вас продукт";

                        ReplyKeyboard = new[]
                        {
                            new[] { "Обязательные виды" },
                            new[] { "Личное страхование" },
                            new[] { "Имущество" },
                            new[] { "Каско" },
                            new[] { "Ответственность" },
                            new[] { "Вернуться в меню..." },
                        };

                        await _telegramBot.SendTextMessageAsync(
                            messageEventArgs.Message.Chat,
                            responseMessage,
                            replyMarkup: ReplyKeyboard);

                        EnvironmentVariables.LastPrintedMessage = responseMessage;
                        EnvironmentVariables.LastInputMessage = s;
                        EnvironmentVariables.ShowLastPrintedMessage = true;
                        EnvironmentVariables.MessagePath += s;
                        break;



                    case string s when s == "Обязательные виды":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Оформить договор страхования")
                        {
                            responseMessage = "Выберете интересующий вас продукт";

                            ReplyKeyboard = new[]
                            {              
                                new[] {"ОГПО ВТС"},
                                new[] {"ОГПО ПП"},
                                new[] {"ОГПО А"},
                                new[] {"ОГПО ЧН"},
                                new[] {"ОГПО Эко"},
                                new[] {"ОГПО ВОО"},
                                new[] { "Вернуться в меню..." },
                            };

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                "\nДля начала я задам несколько вопросов, чтобы посчитать стоимость полиса."+
                                responseMessage,
                                replyMarkup: ReplyKeyboard);

                            EnvironmentVariables.LastPrintedMessage = responseMessage;
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;
                            break;
                        }
                        goto default;


                    case string s when s == "ОГПО ВТС":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Обязательные виды")
                        {
                            responseMessage = "Выберите тип транспортного средства";

                            ReplyKeyboard = new[]
                            {
                                new[] {"Легковые машины"},
                                new[] {"Автобусы до 16 пассажирских мест (включительно)"},
                                new[] {"Автобусы свыше 16 пассажирских мест"},
                                new[] {"Грузовые"},
                                new[] {"Троллейбусы, трамваи"},
                                new[] {"Мототранспорт"},
                                new[] {"Прицепы, полуприцепы"},
                                new[] { "Вернуться в меню..." },
                            };

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: ReplyKeyboard);

                            

                            EnvironmentVariables.LastPrintedMessage = responseMessage;
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;
                            break;
                        }
                        goto default;



                    case string s when s == "Легковые машины" ||
                            s == "Автобусы до 16 пассажирских мест (включительно)" ||
                            s == "Автобусы свыше 16 пассажирских мест" ||
                            s == "Грузовые" ||
                            s == "Троллейбусы, трамваи" ||
                            s == "Мототранспорт" ||
                            s == "Прицепы, полуприцепы":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "ОГПО ВТС")
                        {
                            responseMessage = "Выберите регион регистрации автомобиля";

                            ReplyKeyboard = new[]
                            {
                                  new[] {"Алматинская область"},
                                  new[] {"Южно-Казахстанская область"},
                                  new[] {"Восточно-Казахстанская область"},
                                  new[] {"Костанайская область"},
                                  new[] {"Карагандинская область"},
                                  new[] {"Северо-Казахстанская область"},
                                  new[] {"Акмолинская область"},
                                  new[] {"Павлодарская область"},
                                  new[] {"Жамбылская область"},
                                  new[] {"Актюбинская область"},
                                  new[] {"Западно-Казахстанская область"},
                                  new[] {"Кызылординская область"},
                                  new[] {"Атырауская область"},
                                  new[] {"Мангистауская область"},
                                  new[] {"Алматы"},
                                  new[] {"Астана"},
                                  new[] {"ТС снятые с учета для последующей регистрации"},
                                  new[] {"Временный въезд"},
                                  new[] {"Шымкент"},
                                  new[] {"Туркестанская область"},
                                  new[] { "Вернуться в меню..." },
                            };
           
                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: ReplyKeyboard);

                            EnvironmentVariables.LastPrintedMessage = responseMessage;
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/"+s;
                            break;
                        }
                        goto default;


                    case string s when s == "Алматинская область" ||
                                   s == "Южно-Казахстанская область" ||
                                   s == "Восточно-Казахстанская область" ||
                                   s == "Костанайская область" ||
                                   s == "Карагандинская область" ||
                                   s == "Северо-Казахстанская область" ||
                                   s == "Акмолинская область" ||
                                   s == "Павлодарская область" ||
                                   s == "Жамбылская область" ||
                                   s == "Актюбинская область" ||
                                   s == "Западно-Казахстанская область" ||
                                   s == "Кызылординская область" ||
                                   s == "Атырауская область" ||
                                   s == "Мангистауская область" ||
                                   s == "Алматы" ||
                                   s == "Астана" ||
                                   s == "ТС снятые с  учета для последующей регистрации" ||
                                   s == "Временный въезд" ||
                                   s == "Шымкент" ||
                                   s == "Туркестанская область":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Легковые машины" ||
                            EnvironmentVariables.LastInputMessage == "Автобусы до 16 пассажирских мест (включительно)" ||
                            EnvironmentVariables.LastInputMessage == "Автобусы свыше 16 пассажирских мест" ||
                            EnvironmentVariables.LastInputMessage == "Грузовые" ||
                            EnvironmentVariables.LastInputMessage == "Троллейбусы, трамваи" ||
                            EnvironmentVariables.LastInputMessage == "Мототранспорт" ||
                            EnvironmentVariables.LastInputMessage == "Прицепы, полуприцепы")
                        {
                            responseMessage = "Гор.обл. или респ.значения";

                            ReplyKeyboard = new[]
                            {
                                  new[] {"Да"},
                                  new[] {"Нет"},
                                  new[] { "Вернуться в меню..." },
                            };

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: ReplyKeyboard);

                            EnvironmentVariables.LastPrintedMessage = responseMessage;
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;
                            break;
                        }
                        goto default;



                    case string s when s == "Да" || s == "Нет":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Алматинская область" ||
                            EnvironmentVariables.LastInputMessage == "Южно-Казахстанская область" ||
                            EnvironmentVariables.LastInputMessage == "Восточно-Казахстанская область" ||
                            EnvironmentVariables.LastInputMessage == "Костанайская область" ||
                            EnvironmentVariables.LastInputMessage == "Карагандинская область" ||
                            EnvironmentVariables.LastInputMessage == "Северо-Казахстанская область" ||
                            EnvironmentVariables.LastInputMessage == "Акмолинская область" ||
                            EnvironmentVariables.LastInputMessage == "Павлодарская область" ||
                            EnvironmentVariables.LastInputMessage == "Жамбылская область" ||
                            EnvironmentVariables.LastInputMessage == "Актюбинская область" ||
                            EnvironmentVariables.LastInputMessage == "Западно-Казахстанская область" ||
                            EnvironmentVariables.LastInputMessage == "Кызылординская область" ||
                            EnvironmentVariables.LastInputMessage == "Атырауская область" ||
                            EnvironmentVariables.LastInputMessage == "Мангистауская область" ||
                            EnvironmentVariables.LastInputMessage == "Алматы" ||
                            EnvironmentVariables.LastInputMessage == "Астана" ||
                            EnvironmentVariables.LastInputMessage == "ТС снятые с  учета для последующей регистрации" ||
                            EnvironmentVariables.LastInputMessage == "Временный въезд" ||
                            EnvironmentVariables.LastInputMessage == "Шымкент" ||
                            EnvironmentVariables.LastInputMessage == "Туркестанская область")
                        {
                            responseMessage = "Введите год выпуска тс";

                            ReplyKeyboard = new[]
                            {
                                  new[] { "Вернуться в меню..." },
                            };

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: ReplyKeyboard);

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: new ReplyKeyboardRemove());

                            EnvironmentVariables.LastPrintedMessage = responseMessage;
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;
                            break;
                        }
                        goto default;


                    case string s when int.TryParse(s, out int n) && s.Length == 4:
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Да" ||
                            EnvironmentVariables.LastInputMessage == "Нет")
                        {
                            EnvironmentVariables.LastPrintedMessage = "Стоимость вашего электронного полиса ";
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;

                            var splittedPath = EnvironmentVariables.MessagePath.Split('/');

                            EnvironmentVariables.MessagePath = string.Empty;
                            var ogpoVts = _insuranceService.CreateOgpoVts(splittedPath[3], splittedPath[4], splittedPath[5], Int32.Parse(splittedPath[6]));

                            responseMessage = $"Стоимость вашего полиса {Math.Round(ogpoVts.TotalPrice,2)} тнг. " +
                                "Спасибо вам, что вы настолько круты и сами оформляете через бота 🤖";

                            ReplyKeyboard = new[]
                                                        {
                                  new[] {$"Да, оформляем за {Math.Round(ogpoVts.TotalPrice, 2)} тнг."},
                                  new[] {"Вернуться в меню"},
                            };

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                responseMessage,
                                replyMarkup: ReplyKeyboard);
                            break;
                        }

                        goto default;

                    #endregion


#region InsurerRegistration

                    case string s when s == "Да, оформляем за ":
                        await _telegramBot.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);
                        if (EnvironmentVariables.LastInputMessage == "Стоимость вашего электронного полиса ")
                        {
                            EnvironmentVariables.LastPrintedMessage = "Да, оформляем за ";
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += s;

                            responseMessage = "Напишите вашу фамилию";

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                "Начинаем. Для оформления полиса заполните данные об авто и водителе," +
                                " которые обязательно указывать по Закону Джунглей. Время оформления ~ 7 минут\n\n" + responseMessage,
                                replyMarkup: new ReplyKeyboardRemove());
                            break;
                        }
                        goto default;

                    case string s when Regex.IsMatch(s, @"^-?\d+$"):
                        if (EnvironmentVariables.LastInputMessage == "Стоимость вашего электронного полиса ")
                        {
                            EnvironmentVariables.LastPrintedMessage = "Да, оформляем за ";
                            EnvironmentVariables.LastInputMessage = s;
                            EnvironmentVariables.ShowLastPrintedMessage = true;
                            EnvironmentVariables.MessagePath += "/" + s;

                            responseMessage = "Напишите вашу фамилию";

                            await _telegramBot.SendTextMessageAsync(
                                messageEventArgs.Message.Chat,
                                "Начинаем. Для оформление полиса заполните данные об авто и водителе," +
                                " которые обязательно указывать по Закону Джунглей. Время оформления ~ 7 минут\n\n" + responseMessage,
                                replyMarkup: new ReplyKeyboardRemove());
                            break;
                        }
                        goto default;

                    #endregion


                    default:
                        inlineKeyboard = _navigationService.CreateInlineKeyboard("Назад...\\Вернуться в меню|");
                            await _telegramBot.SendTextMessageAsync(
                                chatId: messageEventArgs.Message.Chat,
                                text: "К сожалению, мы не нашли нужное вам значение в базе. " +
                                "Попробуйте написать еще раз иначе.", replyMarkup: inlineKeyboard);
                        break;                
                }
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            
            string messageBody = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;

            await _navigationService.NavigateTo(messageBody, chatId, callbackQueryEventArgs.CallbackQuery.From.Id, _telegramBot);
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                new InlineQueryResultLocation(
                    id: "1",
                    latitude: 40.7058316f,
                    longitude: -74.2581888f,
                    title: "New York")   // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 40.7058316f,
                            longitude: -74.2581888f)    // message if result is selected
                    },

                new InlineQueryResultLocation(
                    id: "2",
                    latitude: 13.1449577f,
                    longitude: 52.507629f,
                    title: "Berlin") // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 13.1449577f,
                            longitude: 52.507629f)   // message if result is selected
                    }
            };

            await _telegramBot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
        static Program()
        {
            _telegramBot = new TelegramBotClient(EnvironmentVariables.BotToken);
            _insuranceService = new InsuranceService();
            _navigationService = new NavigationService();
            _currentUser = new DTOs.User();
        }
    }
}
