using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBasicApp01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 메인윈도우 로드 후 이벤트처리 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // DB연결
            // 데이터그리드에 데이터를 바인딩
            LoadControlFromDb();
            LoadGridFromDb();
        }

        private async void LoadControlFromDb()
        {
            // 1. 연결 문자열
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = "SELECT division, names FROM divtbl";

            List<KeyValuePair<string,string>> divisions = new List<KeyValuePair<string,string>>();

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
                    await this.ShowMessageAsync($"에러 ㅋㅋㅋㅋ {ex.Message}", "에러");
                }
            }
            CboDivisions.ItemsSource = divisions;
        }

        private void LoadGridFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query =  @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price,
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

        /// <summary>
        /// 데이터그리드 더블클릭 이벤트핸들러
        /// 선택한 그리드의 레코드값이 오른쪽 상세에 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrdBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GrdBooks.SelectedItems.Count == 1)
            {
                var item = GrdBooks.SelectedItems[0] as DataRowView;

                NumIdx.Value = Convert.ToDouble(item.Row["Idx"]);
                CboDivisions.SelectedValue = Convert.ToString(item.Row["Division"]);
                TxtNames.Text = Convert.ToString(item.Row["Names"]);
                TxtIsbn.Text = Convert.ToString(item.Row["ISBN"]);
                TxtAuthor.Text = Convert.ToString(item.Row["Author"]);
                TxtPrice.Text = Convert.ToString(item.Row["Price"]);
                DpcReleaseDate.Text = Convert.ToString(item.Row["ReleaseDate"]);

            }
        }
    }
}