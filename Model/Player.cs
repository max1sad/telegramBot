using System;
using System.Collections.Generic;
using System.Text;

namespace FootballTelegramBot.Model
{
    class Player
    {
        private int idPlayer;
        private string FIO = "";
        private string idPhone = "";
        public int IdPlayer
        {
            get; set;
        }
        public string Fio
        {
            get; set;
        }
        public string IdPhone
        {
            get; set;
        }
    }
}
