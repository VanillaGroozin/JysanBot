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
using Telegram.Bot.Types;
using System.Collections.Generic;

namespace JysanBot.DTOs
{
    public class User
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public string FIO { get; internal set; }
        public string IIN { get; set; }
        public Contact Contact { get; set; }
        public DTP DTPs { get; set; }
    }
}
