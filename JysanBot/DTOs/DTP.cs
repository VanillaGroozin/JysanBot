using System;
using System.Collections.Generic;
using System.Drawing;
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
        public List <Image> Photos { get; set; }
        public Location Location { get; set; }
        public DateTime LocationGetTime { get; set; }

        public bool IsLocationNeeded ()
        {
            TimeSpan span = DateTime.Now.Subtract(LocationGetTime);
            return span.TotalMinutes > 5;
        }
    }
}
