using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace FootballTelegramBot
{
    class StringParcers
    {
        public  bool inputNumberLeague ( string applicationChek,int levelAplly)
        {
            bool check = false;
            if (levelAplly == 1 || levelAplly == 2)
            {
                string pattern = @"^[0-9]{1,2}$";
                if (Regex.IsMatch(applicationChek, pattern))
                {
                    check = true;
                }
            }
            if (levelAplly == 3)
            {
                string pattern = @"^[\wа-яёА-ЯЁ0-9][\wа-яёА-ЯЁ0-9\s\-]+\:[а-яёА-ЯЁ\s\;]+\;{1}$";
                if (Regex.IsMatch(applicationChek, pattern))
                {
                    check = true;
                }
            }
            Console.WriteLine(check);
            return check;
        }
    }
}
