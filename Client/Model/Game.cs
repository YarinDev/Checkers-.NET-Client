using System;


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
