using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using static MovieFinder2025.Models.YoutubeSearchResult;

namespace MovieFinder2025.ViewModels
{
    public partial class TrailerViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        [ObservableProperty] private string _movieTitle;
        [ObservableProperty] private string _addressStr;
        [ObservableProperty] private Item _selectedItem;
        [ObservableProperty] private ObservableCollection<Item> _videoList = new();

        public TrailerViewModel(string movieTitle, IDialogCoordinator _dialogCoordinator)
        {
           
            MovieTitle = movieTitle;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            var data = await SearchTrailerFromYoutube();

            VideoList.Clear();

            if (data?.Items != null && data.Items.Any(i => i.Id.Kind == "youtube#video"))
            {
                foreach (var item in data.Items.Where(i => i.Id.Kind == "youtube#video"))
                {
                    VideoList.Add(item);
                }

                SelectedItem = VideoList.First();
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "알림", "관련 영상이 없습니다.");
            }
        }

        partial void OnSelectedItemChanged(Item value)
        {
            if (value != null)
            {
                AddressStr = $"https://www.youtube.com/embed/{value.Id.VideoId}";
            }
        }

        private async Task<YoutubeSearchResult> SearchTrailerFromYoutube()
        {
            string apiKey = ConfigLoader.LoadApiKey();  // API KET HERE!
            string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&type=video&maxResults=5&q={Uri.EscapeDataString($"{MovieTitle} Official Trailer")}&key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<YoutubeSearchResult>(json);
                return data;
            }
        }


    }
}
