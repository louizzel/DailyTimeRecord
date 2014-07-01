using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Security.Principal;

namespace DailyTimeRecord
{
    class DBConnector
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnector()
        {
            Initialize();
        }

        public string GetDirectory()
        {
            return "C:\\Users\\" + WindowsIdentity.GetCurrent().Name.Split('\\')[1] + "\\My Documents\\Dropbox\\GDTR\\";
        }

        public void Initialize()
        {
            server = "localhost";
            database = "garapatadtr";
            uid = "root";
            password = "";
            string connectionString = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + uid + ";PWD=" + password;
            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public void Insert(string name, string logType)
        {
            this.OpenConnection();
            string query = "INSERT INTO tbldtr (Name, LogType, LogTime, LogDate) values ('" + name + "','" + logType + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "')";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }

        public List<string> Select()
        {
            string query = "SELECT * FROM tbldtr";
            var result = new List<string>();

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    result.Add(dataReader["LogID"].ToString());
                }

                dataReader.Close();
                this.CloseConnection();
                return result;
            }
            else
            {
                return result;
            }
        }

        /***** STATEMENT FOR THE TABLE CREATION IN SQL
            SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
            SET time_zone = "+00:00";
         
            CREATE TABLE IF NOT EXISTS `tbldtr` (
            `LogID` int(11) NOT NULL AUTO_INCREMENT,
            `Name` text NOT NULL,
            `LogType` text NOT NULL,
            `LogTime` time NOT NULL,
            `LogDate` date NOT NULL,
            PRIMARY KEY (`LogID`)
            ) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=6 ;
        *****/
    }
}
