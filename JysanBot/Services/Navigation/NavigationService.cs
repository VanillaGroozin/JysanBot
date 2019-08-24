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
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace JysanBot.Services.Navigation
{
    public class NavigationService
    {
        private static readonly InsuranceService _insuranceService = new InsuranceService();
        public static string lastPrintedMessage = string.Empty;
        public static string lastInputMessage = string.Empty;
        public static bool showLastPrintedMessage = false;
        public static string messagePath = string.Empty;
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData(""), });
        ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup();

        public bool IsStringIIN(string s)
        {
            return Regex.IsMatch(s, @"^\d+$") && s.Length == 12;
        }
        public bool IsStringPhonenumber(string s)
        {
            return Regex.IsMatch(s, @"^\d+$") && s.Length == 10 && s[0] == 7 && s[1] == 7;
        }
        public InlineKeyboardMarkup CreateInlineKeyboard (string s)
        {
            //1\1.1\1.2|
            //  2\2.1|  
            //    3|    
            var charArray = s.ToCharArray();
            var rows = new List<InlineKeyboardButton[]>();
            var cols = new List<InlineKeyboardButton>();
            string buttonName = string.Empty;
            
            for (var i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == '\\' || charArray[i] == '|')
                {
                    cols.Add(InlineKeyboardButton.WithCallbackData(buttonName));
                    buttonName = string.Empty;
                    if (charArray[i] == '|')
                    {
                        rows.Add(cols.ToArray());
                        cols = new List<InlineKeyboardButton>();
                    }
                }           
                else
                {
                    buttonName += charArray[i];
                }
            }
            return new InlineKeyboardMarkup(rows.ToArray());
        }

        private void CutMessagePath(string[] splittedMessageBody, int i = 0)
        {
            try
            {
                if (splittedMessageBody.Length > i)
                {
                    messagePath = messagePath.Substring(0, messagePath.LastIndexOf('\\'));
                }
            }
            catch
            {
                messagePath = string.Empty;
            }
        }

        public async Task NavigateTo(string messageBody, long chatId, int userId, TelegramBotClient _telegramBot)
        {
            await _telegramBot.SendChatActionAsync(chatId, ChatAction.Typing);

            if (messageBody != "SOS ДТП" ^
                messageBody != "Купить" ^
                messageBody != "Связаться с оператором")
                messageBody = '\\' + messageBody;
            else messagePath = string.Empty;

            var responseMessage = string.Empty;
            var splittedMessageBody = messagePath.Split('\\');

            if (messageBody != "\\Назад...")
            {
                if (splittedMessageBody[splittedMessageBody.Length - 1] != messageBody) messagePath += messageBody;
            }
            else
            {
                CutMessagePath(splittedMessageBody, 0);
            }

            splittedMessageBody = messagePath.Split('\\');

            switch (splittedMessageBody[0])
                {
                case "Купить":
                    {
                        if (splittedMessageBody.Length > 1)
                        {
                            switch (splittedMessageBody[1])
                            {
                                case "Sam takoi":
                                    {
                                        await _telegramBot.SendTextMessageAsync(
                                            chatId,
                                            "Ty durak?");
                                        break;
                                    }
                                case "Izvinis":
                                    {
                                        await _telegramBot.SendTextMessageAsync(
                                            chatId,
                                            "izvini");
                                        break;
                                    }
                            }

                        }
                        inlineKeyboard = CreateInlineKeyboard("Назад...|");

                        await _telegramBot.SendTextMessageAsync(
                            chatId,
                            "otvet",
                            replyMarkup: inlineKeyboard);
                        break;
                        
                    }


                case "SOS ДТП":

                    if (splittedMessageBody.Length > 1)
                    {
                        switch (splittedMessageBody[1])
                        {
                            case "Контактные номера телефонов":
                                {
                                    responseMessage = "Вызовите сотрудников административной полиции – 102\n" +
                                    "1) В случае необходимости:\n" +
                                    "\t- 101 – пожарная служба; \n" +
                                    "\t- 103 – скорая медицинская помощь; \n" +
                                    "\t- 104 – газовая служба; \n" +
                                    "\t- 112 – служба спасения.\n" +
                                    "2) Сообщите другим участникам ДТП номер Вашего полиса ОГПО ВТС и телефон Цесна Гарант\n" +
                                    "\t- Контакты для консультации: \n" +
                                    "\tс 8:00 до 22:00 – call центр: 3264 с мобильных бесплатно, +7(727) 357 25 25\n" +
                                    "\tс 22:00 до 8:00 – аварийный комиссар +7 701 529 80 48\n";

                                    inlineKeyboard = CreateInlineKeyboard("Назад...|");
                                    await _telegramBot.SendTextMessageAsync(
                                        chatId,
                                        responseMessage,
                                        replyMarkup: inlineKeyboard);
                                    break;
                                }
                            case "Список документов":
                                {
                                    inlineKeyboard = CreateInlineKeyboard("Назад...|");
                                    await _telegramBot.SendTextMessageAsync(
                                        chatId,
                                        "Потом доделаю",
                                        replyMarkup: inlineKeyboard);
                                    break;
                                }
                            case "Действия клиента":
                                {
                                    if (splittedMessageBody.Length > 2)
                                    {
                                        switch (splittedMessageBody[2])
                                        {
                                            case "Виновник":
                                                {
                                                    responseMessage = "1) заявление о страховом случае;\n" +
                                                        "2) страховой полис(его дубликат) виновника ДТП;\n" +
                                                        "3) документ компетентных органов, подтверждающий факт наступления страхового случая и размер вреда, причиненного потерпевшим;\n" +
                                                        "4) копию удостоверения личности;\n" +
                                                        "5) копия водительского удостоверения(временных прав);заключение медицинского освидетельствования.\n";
                                                    break;
                                                }
                                            case "Потерпевший":
                                                {
                                                    responseMessage = "1) (Прямое урегулирование)\n" +
                                                        "2) К вышеперечисленному списку документов + полис виновника ДТП\n" +
                                                        "3) Более полную информацию по перечню предоставляемых документов можно получить у специалиста\n" +
                                                        "4) Отдела страховых выплат(список аварийных комиссаров)\n";
                                                    break;
                                                }
                                        }
                                        CutMessagePath(splittedMessageBody);
                                        await _telegramBot.SendTextMessageAsync(chatId, responseMessage,replyMarkup: inlineKeyboard);
                                        break;
                                    }
                                    inlineKeyboard = CreateInlineKeyboard("Виновник|Потерпевший|Назад...|");

                                    await _telegramBot.SendTextMessageAsync(
                                        chatId,
                                        "izvini",
                                        replyMarkup: inlineKeyboard);
                                    break;

                                }
                            case string s when s ==  "Заявить о ДТП":
                                var user = _insuranceService.GetUserInfo(userId);
                                if (splittedMessageBody.Length > 2)
                                {
                                    switch (splittedMessageBody[2])
                                    {
                                        case string st when st == "❌ ФИО" || st == "✔️ ФИО":
                                            if (splittedMessageBody.Length == 3)
                                            {
                                                await _telegramBot.SendTextMessageAsync(chatId, "Введите ФИО", replyMarkup: new ReplyKeyboardRemove());
                                            }
                                            else if (splittedMessageBody[3].Split(' ').Length == 3)
                                            {
                                                user.FIO = st;
                                                _insuranceService.UserUpdate(user);
                                                CutMessagePath(splittedMessageBody, 0);
                                            }
                                            else
                                            {
                                                await _telegramBot.SendTextMessageAsync(chatId, "Неверное ФИО", replyMarkup: new ReplyKeyboardRemove());
                                                CutMessagePath(splittedMessageBody);
                                            }                                      
                                            break;

                                        case string st when st == "❌ ИИН" || st == "✔️ ИИН" || IsStringIIN(st):
                                            if (splittedMessageBody.Length==3)
                                            {
                                                await _telegramBot.SendTextMessageAsync(chatId, "Введите ИИН", replyMarkup: new ReplyKeyboardRemove());
                                            }
                                            else if (IsStringIIN(splittedMessageBody[3]))
                                            {                                              
                                                user.IIN = splittedMessageBody[3];
                                                _insuranceService.UserUpdate(user);
                                                CutMessagePath(splittedMessageBody);
                                            }
                                            else
                                            {
                                                await _telegramBot.SendTextMessageAsync(chatId, "Неверный ИИН", replyMarkup: new ReplyKeyboardRemove());
                                                CutMessagePath(splittedMessageBody);
                                            }
                                            break;

                                        case string st when st == "❌ Телефон" || st == "✔️ Телефон":
                                            if (splittedMessageBody.Length == 3)
                                            {
                                                replyKeyboardMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestContact("Отправить телефон"));
                                                replyKeyboardMarkup.ResizeKeyboard = true;
                                                await _telegramBot.SendTextMessageAsync(chatId, "📲 Поделитесь номером телефона", 
                                                    parseMode: ParseMode.Html,replyMarkup: replyKeyboardMarkup);
                                            }
                                            break;

                                        case string st when st == "❌ Местоположение" || st == "✔️ Местоположение":
                                            if (splittedMessageBody.Length == 3)
                                            {
                                                replyKeyboardMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("🌏 Отправить местоположение"));
                                                replyKeyboardMarkup.ResizeKeyboard = true;
                                                await _telegramBot.SendTextMessageAsync(chatId, "🌏 Поделитесь местоположением",
                                                    parseMode: ParseMode.Html, replyMarkup: replyKeyboardMarkup);
                                            }
                                            break;
                                        case string st when st == "❌ Фото" || st == "✔️ Фото":
                                            if (splittedMessageBody.Length == 3)
                                            {
                                                await _telegramBot.SendTextMessageAsync(chatId, "Отправьте фото <b>ДТП</b>",
                                                    parseMode: ParseMode.Html);
                                            }
                                            break;
                                    }
                                    CutMessagePath(splittedMessageBody, 3);
                                }
                                if (splittedMessageBody.Length != 3)
                                {
                                    string FIO = string.Empty,
                                           IIN = string.Empty,
                                           phoneNum = string.Empty,
                                           location = string.Empty,
                                           photos = string.Empty,
                                           fullKeyboardString = string.Empty;

                                    if (user.FIO == null) FIO += "❌ ФИО";
                                    else FIO += "✔️ ФИО";
                                    if (user.IIN == null) IIN += "❌ ИИН";
                                    else IIN += "✔️ ИИН";
                                    if (user.Contact == null) phoneNum += "❌ Телефон";
                                    else phoneNum += "✔️ Телефон";
                                    if (user.DTPs == null) user.DTPs = new DTOs.DTP();
                                    if (user.DTPs.Location == null) location += "❌ Местоположение";
                                    else location += "✔️ Местоположение";
                                    if (user.DTPs.Photos == null) photos += "❌ Фото";
                                    else photos += "✔️ Фото";
                                    fullKeyboardString = $"{FIO}\\{IIN}\\{phoneNum}|{location}\\{photos}|❓Как сделать фото?|";
                                    if (fullKeyboardString.Contains("❌"))
                                    {
                                        inlineKeyboard = CreateInlineKeyboard(fullKeyboardString);
                                    }
                                    else
                                    {
                                        inlineKeyboard = CreateInlineKeyboard(fullKeyboardString + "Заявить о ДТП|");
                                    }


                                    await _telegramBot.SendTextMessageAsync(chatId, "Заполните все поля чтобы продолжить", replyMarkup: inlineKeyboard);
                                }
                                break;

                            case "Вернуться в меню":
                                messagePath = string.Empty;
                                break;
                        }
                        break;
                    }
                    else
                    {
                        inlineKeyboard = CreateInlineKeyboard("Контактные номера телефонов|Список документов|Заявить о ДТП|Действия клиента|Назад...|");
                    
                        await _telegramBot.SendTextMessageAsync(
                                chatId,
                                "otvet",
                                replyMarkup: inlineKeyboard);
                        break;
                    }


                case "Связаться с оператором":
                    responseMessage = "Вошел в \"Связаться с оператором\"";

                    await _telegramBot.SendTextMessageAsync(chatId, responseMessage);
                    break;


                default:
                    inlineKeyboard = CreateInlineKeyboard("Купить\\SOS ДТП|Связаться с оператором|");
                    responseMessage = "<b>Главное меню</b>..";
                    await _telegramBot.SendTextMessageAsync(chatId, responseMessage, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                    break;
            }

            }
        }
    }
