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
        public void SqlRead(String sqlstr)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlstr,sqlConnection))
                {
                    sqlConnection.Open();
                    Console.WriteLine(sqlCommand.CommandText);                   
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            Console.WriteLine(sqlDataReader.GetString(0));
                        }
                    }
                }
                
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            
                
        }

    }
}
