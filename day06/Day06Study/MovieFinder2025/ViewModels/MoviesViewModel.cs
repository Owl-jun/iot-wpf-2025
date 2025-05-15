using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using MovieFinder2025.Views;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Threading;

namespace MovieFinder2025.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        public MoviesViewModel(IDialogCoordinator _cd)
        {
            dialogCoordinator = _cd;
            MovieItems = new ObservableCollection<MovieItem>();
            PosterUri = new Uri("/Views/nopicture.png",UriKind.Relative);
            CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();
            Common.LOGGER.Info("PROGRAM START");
        }

        #region 멤버 변수&속성
        
        [ObservableProperty] private string _searchResult;
        [ObservableProperty] private string _currDateTime;
        [ObservableProperty] private string _movieName;
        [ObservableProperty] private ObservableCollection<MovieItem> _movieItems;
        [ObservableProperty] private Uri _posterUri;
        
        private readonly IDialogCoordinator dialogCoordinator;
        private readonly DispatcherTimer _timer;
        private MovieItem _selectedMovieItem;
        public MovieItem SelectedMovieItem { 
            get => _selectedMovieItem; 
            set
            {
                string base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                SetProperty(ref _selectedMovieItem, value);
                PosterUri = new Uri($"{base_url}{value.Poster_path}",UriKind.Absolute);
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async Task ViewTrailer()
        {
            if (SelectedMovieItem == null) { await this.dialogCoordinator.ShowMessageAsync(this, "예고편 보기", "영화를 선택하세요."); return; }
            var viewModel = new TrailerViewModel(SelectedMovieItem.Title,Common.DIALOGCOORDINATOR);
            var view = new TrailerView()
            {
                DataContext = viewModel
            };
            view.Owner = Application.Current.MainWindow;
            view.ShowDialog();

        }
        [RelayCommand]
        public async Task AddStar()
        {
            if (SelectedMovieItem == null) {  return; }
            try
            {
                string query = string.Empty;
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    query = @"INSERT INTO movieitem (
                                            id,
                                            adult,
                                            backdrop_path,
                                            original_language,
                                            original_title,
                                            overview,
                                            popularity,
                                            poster_path,
                                            release_date,
                                            title,
                                            video,
                                            vote_average,
                                            vote_count
                                        ) VALUES (
                                            @id,
                                            @adult,
                                            @backdrop_path,
                                            @original_language,
                                            @original_title,
                                            @overview,
                                            @popularity,
                                            @poster_path,
                                            @release_date,
                                            @title,
                                            @video,
                                            @vote_average,
                                            @vote_count
                                        );";


                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", SelectedMovieItem.Id);
                    cmd.Parameters.AddWithValue("@adult", SelectedMovieItem.Adult);
                    cmd.Parameters.AddWithValue("@backdrop_path", SelectedMovieItem.Backdrop_path);
                    cmd.Parameters.AddWithValue("@original_language", SelectedMovieItem.Original_language);
                    cmd.Parameters.AddWithValue("@original_title", SelectedMovieItem.Original_title);
                    cmd.Parameters.AddWithValue("@overview", SelectedMovieItem.Overview);
                    cmd.Parameters.AddWithValue("@popularity", SelectedMovieItem.Popularity);
                    cmd.Parameters.AddWithValue("@poster_path", SelectedMovieItem.Poster_path);
                    cmd.Parameters.AddWithValue("@release_date", SelectedMovieItem.Release_date);
                    cmd.Parameters.AddWithValue("@title", SelectedMovieItem.Title);
                    cmd.Parameters.AddWithValue("@video", SelectedMovieItem.Video);
                    cmd.Parameters.AddWithValue("@vote_average", SelectedMovieItem.Vote_average);
                    cmd.Parameters.AddWithValue("@vote_count", SelectedMovieItem.Vote_count);

                    var resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("즐겨찾기 저장완료");
                        //MessageBox.Show("저장성공~");
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "추가성공!");
                    }
                    else
                    {
                        Common.LOGGER.Warn("즐겨찾기 저장실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "추가실패~!");
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Message.ToUpper().Contains("DUPLICATE ENTRY"))
                {
                    await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "이미 저장된 영화입니다.");
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }
        [RelayCommand]
        public async Task ViewStar()
        {
            clearItems();
            try
            {
                string query = string.Empty;
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    query = @"SELECT * FROM movieitem";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var item = new MovieItem
                        {
                            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                            Adult = reader.IsDBNull(reader.GetOrdinal("adult")) ? false : reader.GetBoolean(reader.GetOrdinal("adult")),
                            Backdrop_path = reader.IsDBNull(reader.GetOrdinal("backdrop_path")) ? "" : reader.GetString(reader.GetOrdinal("backdrop_path")),
                            Original_language = reader.IsDBNull(reader.GetOrdinal("original_language")) ? "" : reader.GetString(reader.GetOrdinal("original_language")),
                            Original_title = reader.IsDBNull(reader.GetOrdinal("original_title")) ? "" : reader.GetString(reader.GetOrdinal("original_title")),
                            Overview = reader.IsDBNull(reader.GetOrdinal("overview")) ? "" : reader.GetString(reader.GetOrdinal("overview")),
                            Popularity = reader.IsDBNull(reader.GetOrdinal("popularity")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("popularity")),
                            Poster_path = reader.IsDBNull(reader.GetOrdinal("poster_path")) ? "" : reader.GetString(reader.GetOrdinal("poster_path")),
                            Release_date = reader.IsDBNull(reader.GetOrdinal("release_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("release_date")),
                            Title = reader.IsDBNull(reader.GetOrdinal("title")) ? "" : reader.GetString(reader.GetOrdinal("title")),
                            Video = reader.IsDBNull(reader.GetOrdinal("video")) ? false : reader.GetBoolean(reader.GetOrdinal("video")),
                            Vote_average = reader.IsDBNull(reader.GetOrdinal("vote_average")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("vote_average")),
                            Vote_count = reader.IsDBNull(reader.GetOrdinal("vote_count")) ? 0 : reader.GetInt32(reader.GetOrdinal("vote_count")),
                        };

                        MovieItems.Add(item);
                    }
                    SearchResult = $"검색된 항목 수 : {MovieItems.Count.ToString()}";
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }
        [RelayCommand]
        public async Task DelStar()
        {
            if (SelectedMovieItem == null) {
                await this.dialogCoordinator.ShowMessageAsync(this, "알림", "삭제할 영화를 선택하십시오.");
                return; 
            }

            var respone = await this.dialogCoordinator.ShowMessageAsync(this, "삭제", $"삭제할 영화제목 : {SelectedMovieItem.Title} \n\n정말 삭제하시겠습니까?",MessageDialogStyle.AffirmativeAndNegative);
            if (respone == MessageDialogResult.Affirmative)
            {
                try
                {
                    string query = "DELETE FROM movieitem WHERE id = @id";

                    using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                    {
                        conn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", SelectedMovieItem.Id);

                            var resultCnt = await cmd.ExecuteNonQueryAsync();
                            if (resultCnt > 0)
                            {
                                Common.LOGGER.Info("삭제 완료");
                                await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 성공!");
                            }
                            else
                            {
                                Common.LOGGER.Warn("삭제 실패: 대상 없음");
                                await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제할 항목이 없습니다!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.LOGGER.Error(ex.ToString());
                    await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                }
                await ViewStar();
            }
            else 
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "알림", "사용자가 삭제를 취소했습니다.");
                return;
            }
        }
        [RelayCommand]
        public void MouseLeftButtonDown()
        {
            clearItems();
        }
        [RelayCommand]
        public async Task MovieRowDoubleClick() 
        {
            if (SelectedMovieItem.Title == SelectedMovieItem.Original_title)
            {
                await dialogCoordinator.ShowMessageAsync(this, "영화 상세정보",
                $"제목\t\t {SelectedMovieItem.Title}\n" +
                $"개봉일자\t {SelectedMovieItem.Release_date.ToString("yyyy-MM-dd")}\n" +
                $"평점\t\t {SelectedMovieItem.Vote_average.ToString("F2")}\n\n" +
                $"{SelectedMovieItem.Overview}"
                );
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "영화 상세정보",
                    $"제목\t\t {SelectedMovieItem.Title}\n" +
                    $"\t\t ({SelectedMovieItem.Original_title})\n" +
                    $"개봉일자\t {SelectedMovieItem.Release_date.ToString("yyyy-MM-dd")}\n" +
                    $"평점\t\t {SelectedMovieItem.Vote_average.ToString("F2")}\n\n" +
                    $"{SelectedMovieItem.Overview}"
                    );
            }
        }
        [RelayCommand]
        public async Task SearchMovie() 
        {
            if (string.IsNullOrEmpty(MovieName))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "검색", "영화이름을 입력하세요!");
            }
            var controller = await dialogCoordinator.ShowProgressAsync(this, "대기중", "검색 중...");
            controller.SetIndeterminate();
            SearchMovie(MovieName);
            await controller.CloseAsync();
        }
        #endregion

        #region Privates
        private void clearItems()
        {
            MovieItems.Clear();
            SearchResult = "";
            PosterUri = new Uri("/Views/nopicture.png", UriKind.Relative);
        }

        private async Task SearchMovie(string movieName)
        {
            clearItems();

            string tmdb_apikey = ConfigLoader.LoadApiKey("config2.json");   // TMDB API KEY HERE !!
            string encoding_moviename = HttpUtility.UrlEncode(movieName,Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_moviename}";
            Common.LOGGER.Info($"TMDB URI : {openApiUri}");
            string result = string.Empty;

            // OpenAPI 실행할 웹 객체
            HttpClient httpClient = new HttpClient();
            MovieSearchResponse? response;
            StreamReader reader = null;
            try
            {
                response = await httpClient.GetFromJsonAsync<MovieSearchResponse?>(openApiUri);
                foreach (var movie in response.Results)
                {
                    Common.LOGGER.Info($"{movie.Title} : {movie.Release_date.ToString("yyyy-MM-dd")}");
                    MovieItems.Add(movie);
                }
            }
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                Common.LOGGER.Fatal($"API FATAL ERROR : {ex.Message}");
            }
            SearchResult = $"검색된 항목 수 : {MovieItems.Count.ToString()}";
        }
        #endregion
    }
}
