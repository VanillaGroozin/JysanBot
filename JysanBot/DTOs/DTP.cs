using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace JysanBot.DTOs
{
    public class DTP
    {
        public int Id { get; internal set; }
        public User user { get; set; }
        public byte[] Photos  { get; set; }
        public Location Location { get; set; }
    }
}
