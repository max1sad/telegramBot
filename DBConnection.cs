using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FootballTelegramBot
{
    class DBConnection
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-2MK3795;Initial Catalog=dbfootbal;Integrated Security=True");
        public void OpenConnect()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("Соединение");
                sqlConnection.Open();
            }
        }
        public void CloseConnet()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public void SqlWrite(String sqlstr)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlstr, sqlConnection))
                {
                    //sqlCommand.Parameters.Add(sqlstr);                   
                    sqlCommand.ExecuteNonQuery();                    
                   Console.WriteLine("Запись в БД");                                       
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
        public  string  SqlRead(String sqlstr, int numberOperation)
        {
            string sqlStr = "";
            
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlstr,sqlConnection))
                {
                    //OpenConnect();
                    //sqlConnection.Open();
                    //Console.WriteLine(sqlCommand.CommandText);
                    
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {   //вывод какие лиги есть в списках
                            if (numberOperation == 0)
                            {
                                sqlStr = sqlStr + sqlDataReader.GetString(0) + " -Лига \n";
                            }
                            //получения ид номера турнира и его название.
                            if (numberOperation == 1)
                            {
                                sqlStr = sqlStr + sqlDataReader.GetInt32(0).ToString() + " --- "+ sqlDataReader.GetString(1) + " \n";
                                //Console.WriteLine(sqlDataReader.GetInt32(0).ToString());

                            }
                            //запрос на то, что бы узнать ид из какой либо таблицы.
                            if (numberOperation == 100)
                            {
                                sqlStr = sqlDataReader.GetInt32(0).ToString();
                            }
                            //Console.WriteLine(sqlStr);
                            //Console.WriteLine(sqlDataReader.GetString(0));
                        }
                        //Console.WriteLine(sqlStr);
                        
                    }
                }
                
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return sqlStr;

        }

    }
}
