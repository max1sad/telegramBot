using System;
using System.Collections.Generic;
using System.Text;

namespace FootballTelegramBot.Model
{
    class NameTourney
    {
        public int idNameTourney { get; set; }
        public string nameTourney { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string statusTourney { get; set; }

    }
}
