using Dapper;
using FootballTelegramBot.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FootballTelegramBot
{
    class DBProvide
    {
        const string ConnectionString = "Data Source=DESKTOP-2MK3795;Initial Catalog=dbfootbal;Integrated Security=True";
        Model.Player player = new Model.Player();
        Model.Team team = new Model.Team();
        Model.League league = new Model.League();
        Model.PlayerTeams playerTeams = new Model.PlayerTeams();
        Model.NameTourney nameTourney = new Model.NameTourney();
        public static DBConnection connectsql = new DBConnection();
       
        public List<League> GetAllLeague()
        {           
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<League>("SELECT * FROM league").ToList();

            }                           
        }
        public void CreateTeams(Team team)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into Team (nameTeams) values (@nameTeams);";
                db.Execute(sqlQuery, team);
            }
        }
        public List<NameTourney> GetAllTourney()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<NameTourney>("SELECT * FROM nameTourney").ToList();

            }
        }
        

    }
}
