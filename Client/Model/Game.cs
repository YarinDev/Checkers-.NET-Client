using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    class Game
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int Length { get; set; }
        public String Winner { get; set; }
    }
}
