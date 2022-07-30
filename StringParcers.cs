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
        //разбивает строку с заявкой на элементы и записывает в массив 
        //первый элемент это id номер лиги
        //второй элемент id номер турнира в котором учавствовать будет.
        //дальше идет 3 параметров название команды, а все остальные элементы массива это Имена игроков команды.
        public List<string> sqlParcerApply (string messageString)
        {
            int startPosition = 0;
            List<string> stringArray = new List<string>();
            for (int i = 0;i < messageString.Length; i++)
            {
                if (messageString[i] == ':')
                {
                    stringArray.Add (messageString.Substring(startPosition, i - startPosition));
                    //strCount++;
                    startPosition = i + 1;
                }
                if (messageString[i] == ';')
                {
                    stringArray.Add(messageString.Substring(startPosition, i - startPosition));
                    startPosition = i + 1;
                }
                
            }
            /*for (int i = 0;i < stringArray.Count ; i++)
            {
                Console.WriteLine(stringArray[i]);
            }*/
            return stringArray;
        }
    }
}
