using MoneyOrganizer.Model.DBModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MoneyOrganizer.Model
{
    public class DBController
    {
        private static string _dbFileName = "money_db.db";
        SQLiteConnection _connection;

        public DBController()
        {
            _connection = null;
        }

        public void OpenConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    _connection.Open();
                } catch(Exception ex)
                {
                    // Obsługa błędów
                }
            }   
        }

        public void CloseConnection()
        {
            if(_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    _connection.Close();
                } catch (Exception ex)
                {
                    // Obsługa błędów
                }
            }
        }

        public string SelectSingleColumnOneResult(string name, string conditions, string columnResult)
        {
            OpenConnection();
            string result = "";
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            cmd.CommandText =  "SELECT * FROM `" + name + "` ";
            cmd.CommandText += conditions;
            cmd.CommandType = System.Data.CommandType.Text;

            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result = (string)reader[columnResult].ToString();

                
            } catch (Exception ex)
            {
                // Obsługa błędów
            }
            CloseConnection();
            return result;
        }

        public List<string> SelectSingleColumnMany(string name, string conditions, string columnResult)
        {
            OpenConnection();
            List<string> result = new List<string>();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT * FROM `" + name + "` ";
            cmd.CommandText += conditions;
            cmd.CommandType = System.Data.CommandType.Text;

            try
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    result.Add((string)reader[columnResult].ToString());
                }

            } catch (Exception ex)
            {
                // Obsługa błędów
            }
            CloseConnection();
            return result;
        }

        public void CreateTable(string name, string[,] values, int numbOfValues)
        {
            OpenConnection();
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = _connection;
                // value 0 - nazwa kolumny
                // value 1 - typ kolumny
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS `" + name + "`(" + values[0, 0] + " " + values[0, 1];
                for (int i = 1; i < numbOfValues; i++)
                {
                    cmd.CommandText += ", " + values[i, 0] + " " + values[i, 1];
                }
                cmd.CommandText += ");";
                ExecuteQuerry(cmd);
            }
            else
            {
                // Obsługa błędów
            }
            CloseConnection();
        }

        public void ExecuteQuerry(SQLiteCommand cmd)
        {
            try
            {
                if(_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    cmd.ExecuteNonQuery();
                }
            } catch(Exception ex)
            {
                // Obsługa błędów
            }
        }

        public void InsertIntoTable(string tableName, string[] columns, string[] values)
        {
            OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "INSERT INTO `" + tableName + "`(";
            for (int i = 0; i < columns.Count(); i++)
            {
                if (columns.Count() - i <= 1) cmd.CommandText += columns[i];
                else cmd.CommandText += columns[i] + ", ";
            }
            cmd.CommandText += ") VALUES(";
            for (int i = 0; i < values.Count(); i++)
            {
                if (values.Count() - i <= 1) cmd.CommandText += "'" + values[i] + "'";
                else cmd.CommandText += "'" + values[i] + "'" + ", ";
            }
            cmd.CommandText += ");";
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                ExecuteQuerry(cmd);
            }
            else
            {
                // Obsługa błędów
            }
            CloseConnection();
        }

        public void UpdateTable(string tableName, string [] columns, string[] values, string[] colCond, string[] valCond, string[] eqCond, string[] andor)
        {
            OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "UPDATE `" + tableName + "` SET ";
            for(int i=0; i<columns.Count(); i++)
            {
                if (columns.Count() - i <= 1)
                    cmd.CommandText += columns[i] + " = '" + values[i] + "' ";
                else
                    cmd.CommandText += columns[i] + " = '" + values[i] + "', ";
            }
            cmd.CommandText += "WHERE ";
            for(int i=0; i < colCond.Count(); i++)
            {
                if (colCond.Count() - i <= 1)
                    cmd.CommandText += colCond[i] + " " + eqCond[i] + " " + valCond[i] + ";";
                else
                    cmd.CommandText += colCond[i] + " " + eqCond[i] + " " + valCond[i] + " " + andor[i];
            }
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                ExecuteQuerry(cmd);
            }
            else
            {
                // Obsługa błędów
            }
            CloseConnection();
        }

        public void DeleteSingleRow(string name, string column, string value)
        {
            OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "DELETE FROM `" + name + "` WHERE " + column + " = '" + value + "';";
            ExecuteQuerry(cmd);
            CloseConnection();
        }

        public bool LoadDB()
        {
            if(CheckPath(""))
            {
                if(!CheckFile(GetPath(), _dbFileName))
                {
                    SQLiteConnection.CreateFile(GetPath() + _dbFileName);
                    if (!CheckFile(GetPath(), _dbFileName))
                    {
                        // Obsługa błędów
                        return false;
                    }
                    else
                    {
                        // Nowa baza
                        _connection = new SQLiteConnection(GetConnectionString(GetPath() + _dbFileName));

                        string[,] userInfo = { { "Id", "INTEGER PRIMARY KEY" }, { "FirstName", "TEXT" }, { "LastName", "TEXT" }, { "Balance", "TEXT" } };
                        int userInfoColumns = 4;

                        string[,] kategorie = { {"Id", "INTEGER PRIMARY KEY" }, { "Name", "TEXT" }, { "Color", "TEXT" }, { "Priority", "INTEGER" }, { "ImageUrl", "TEXT" } };
                        int kategorieColumns = 5;

                        string[,] wydatki = { { "Id", "INTEGER PRIMARY KEY" }, { "CategoryId", "INTEGER" }, { "Name", "TEXT" }, { "Cost", "TEXT" }, { "Date", "TEXT" } };
                        int wydatkiColumnds = 5;

                        string[,] przychody = { { "Id", "INTEGER PRIMARY KEY" }, { "Name", "TEXT" }, { "Amount", "TEXT" }, { "Date", "TEXT" } };
                        int przychodyColumns = 4;

                        CreateTable("UserInfo", userInfo, userInfoColumns);
                        CreateTable("Categories", kategorie, kategorieColumns);
                        CreateTable("Expenses", wydatki, wydatkiColumnds);
                        CreateTable("Income", przychody, przychodyColumns);

                        string[] newUIColumns = { "FirstName", "LastName", "Balance" };
                        string[] newUIValues = { "New", "User", "0" };

                        InsertIntoTable("UserInfo", newUIColumns, newUIValues);

                        string[] newCatColumns = { "Name", "Color", "Priority", "ImageUrl" };
                        string[] newCatValues1 = { "Żywność", "White", "1", "../Resources/Grocery.png" };
                        string[] newCatValues2 = { "Zabawki", "White", "1", "../Resources/Toys.png" };
                        string[] newCatValues3 = { "Rozrywka", "White", "1", "../Resources/Entertainment.png" };
                        string[] newCatValues4 = { "Elektronika", "White", "1", "../Resources/Electronics.png" };
                        string[] newCatValues5 = { "Zdrowie", "White", "1", "../Resources/Health.png" };
                        string[] newCatValues6 = { "Motoryzacja", "White", "1", "../Resources/Automotive.png" };
                        string[] newCatValues7 = { "Dom", "White", "1", "../Resources/Home.png" };
                        string[] newCatValues8 = { "Ogród", "White", "1", "../Resources/Garden.png" };
                        string[] newCatValues9 = { "Biżuteria", "White", "1", "../Resources/Luxury.png" };
                        string[] newCatValues10 = { "Kosmetyka", "White", "1", "../Resources/Beauty.png" };
                        string[] newCatValues11 = { "Sport", "White", "1", "../Resources/Sport.png" };
                        string[] newCatValues12 = { "Oprogramowanie", "White", "1", "../Resources/Software.png" };
                        string[] newCatValues13 = { "Podróże", "White", "1", "../Resources/Travel.png" };
                        string[] newCatValues14 = { "Majsterkowanie", "White", "1", "../Resources/DIY.png" };
                        string[] newCatValues15 = { "Transport", "White", "1", "../Resources/Transport.png" };
                        string[] newCatValues16 = { "Ubrania", "White", "1", "../Resources/Clothes.png" };
                        string[] newCatValues17 = { "Inne", "White", "1", "../Resources/Others.png" };

                        InsertIntoTable("Categories", newCatColumns, newCatValues1);
                        InsertIntoTable("Categories", newCatColumns, newCatValues2);
                        InsertIntoTable("Categories", newCatColumns, newCatValues3);
                        InsertIntoTable("Categories", newCatColumns, newCatValues4);
                        InsertIntoTable("Categories", newCatColumns, newCatValues5);
                        InsertIntoTable("Categories", newCatColumns, newCatValues6);
                        InsertIntoTable("Categories", newCatColumns, newCatValues7);
                        InsertIntoTable("Categories", newCatColumns, newCatValues8);
                        InsertIntoTable("Categories", newCatColumns, newCatValues9);
                        InsertIntoTable("Categories", newCatColumns, newCatValues10);
                        InsertIntoTable("Categories", newCatColumns, newCatValues11);
                        InsertIntoTable("Categories", newCatColumns, newCatValues12);
                        InsertIntoTable("Categories", newCatColumns, newCatValues13);
                        InsertIntoTable("Categories", newCatColumns, newCatValues14);
                        InsertIntoTable("Categories", newCatColumns, newCatValues15);
                        InsertIntoTable("Categories", newCatColumns, newCatValues16);
                        InsertIntoTable("Categories", newCatColumns, newCatValues17);

                        return true;
                    }
                }
                else
                {
                    // Istniejąca baza
                    _connection = new SQLiteConnection(GetConnectionString(GetPath() + _dbFileName));

                    return true;
                }
            }
            else
            {
                // Obsługa błędów
                return false;
            }
        }

        private string GetConnectionString(string plik)
        {
            return "Data Source=" + plik + ";"
                + "Version=3;"
                + "PRAGMA count_changes=OFF;"
                + "PRAGMA temp_store=MEMORY;"
                + "PRAGMA encoding=\"UTF-8\";"
                + "PRAGMA locking_mode=EXCLUSIVE;"
                + "UseUTF8Encoding=True;";
        }

        private bool CheckFile(string path, string name)
        {
            path += name;
            if (!File.Exists(path))
                return false;
            else
                return true;
        }

        private bool CheckPath(string name)
        {
            string path = GetPath();
            if (!Directory.Exists(path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                if (!String.IsNullOrEmpty(name))
                {
                    DirectoryInfo dir2 = Directory.CreateDirectory(path + name + "\\");
                }
                return true;
            }
            else
            {
                if (!Directory.Exists(path + name + "\\"))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(path + name + "\\");
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        private string GetPath()
        {
            return Environment.CurrentDirectory + "\\AppData\\";
        }
    }
}
