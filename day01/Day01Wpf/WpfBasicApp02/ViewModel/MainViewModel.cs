using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBasicApp02.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel() {
            LoadControlFromDb();
            LoadGridFromDb();
        }
        private void LoadControlFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = "SELECT division, names FROM divtbl";

            List<KeyValuePair<string, string>> divisions = new List<KeyValuePair<string, string>>();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");

                        divisions.Add(new KeyValuePair<string, string>(division, names));
                    }
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        private void LoadGridFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price,
                            d.Names AS dNames
                            FROM bookstbl AS b, divtbl AS d
                            WHERE b.Division = d.Division
                            ORDER by b.Idx";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    GrdBooks.ItemsSource = dt.DefaultView;
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
