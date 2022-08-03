using System;
using System.Collections.Generic;
using System.Text;

namespace FootballTelegramBot.Model
{
    class PlayerTeams
    {
        private int idPlayerTeams;
        private int idTeams;
        private string teamCaptain = "";
        private int idPlaeyr;
        public int IdPlayerTeams
        {
            get; set;
        }
        public int IdTeams
        {
            get; set;
        }
        public string TeamCaptain
        {
            get; set;
        }
        public int IdPlaeyr
        {
            get; set;
        }
    }
}
