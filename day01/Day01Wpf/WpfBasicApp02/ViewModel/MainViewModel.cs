using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBasicApp02.Model;
using static System.Reflection.Metadata.BlobBuilder;
namespace WpfBasicApp02.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();
        public ObservableCollection<KeyValuePair<string,string>> Divisions { get; set; }

        private Book _selectedBook;
        
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
                
            }
        }

        public MainViewModel() {
            LoadControlFromDb();
            LoadGridFromDb();
        }
        private void LoadControlFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = "SELECT division, names FROM divtbl";

            ObservableCollection<KeyValuePair<string,string>> divisions = new ObservableCollection<KeyValuePair<string, string>>();

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
            Divisions = divisions;
            OnPropertyChanged(nameof(Divisions));
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
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        Books.Add(new Book
                        {
                            Idx = Convert.ToInt32(dr["Idx"]),
                            Division = Convert.ToString(dr["Division"]),
                            DNames = Convert.ToString(dr["dNames"]),
                            Names = Convert.ToString(dr["Names"]),
                            Author = Convert.ToString(dr["Author"]),
                            ISBN = Convert.ToString(dr["ISBN"]),
                            ReleaseDate = Convert.ToDateTime(dr["ReleaseDate"]),
                            Price = Convert.ToInt32(dr["Price"])
                        });
                    }
                    OnPropertyChanged(nameof(Books));
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
