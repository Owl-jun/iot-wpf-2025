using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace MovieFinder2025.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        public MoviesViewModel(IDialogCoordinator _cd)
        {
            dialogCoordinator = _cd;
            MovieItems = new ObservableCollection<MovieItem>();
            PosterUri = new Uri("/Views/nopicture.png",UriKind.Relative);
            Common.LOGGER.Info("PROGRAM START");
        }

        #region 멤버 변수&속성
        private readonly IDialogCoordinator dialogCoordinator;
        [ObservableProperty] private string _movieName;
        [ObservableProperty] private ObservableCollection<MovieItem> _movieItems;
        private MovieItem _selectedMovieItem;
        [ObservableProperty] private Uri _posterUri;
        
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
        public async Task MovieRowDoubleClick() 
        {
            await dialogCoordinator.ShowMessageAsync(this, "영화 상세정보", 
                $"제목\t\t {SelectedMovieItem.Title}\n" +
                $"\t\t ({SelectedMovieItem.Original_title})\n" +
                $"개봉일자\t {SelectedMovieItem.Release_date.ToString("yyyy-MM-dd")}\n" +
                $"평점\t\t {SelectedMovieItem.Vote_average.ToString("F2")}\n\n" +
                $"{SelectedMovieItem.Overview}"
                );
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
            PosterUri = new Uri("/Views/nopicture.png", UriKind.Relative);
        }

        private async Task SearchMovie(string movieName)
        {
            clearItems();

            string tmdb_apikey = "b6f3cfa101372de6784dd227f907e4a9";
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
        }
        #endregion
    }
}
