using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YamYamBusanApp.Helpers;
using YamYamBusanApp.Models;
namespace YamYamBusanApp.ViewModels
{
    public partial class YamYamBusanViewModel : ObservableObject
    {
        IDialogCoordinator dc;
        private List<PlaceData> _allItems = new();
        [ObservableProperty] private ObservableCollection<PlaceData> _items;
        [ObservableProperty] private ObservableCollection<BackData> _backItems;
        [ObservableProperty] private int _numOfRow = 10;
        [ObservableProperty] private int _pageNo = 1;
        [ObservableProperty] private int _totalCount;
        [ObservableProperty] private string _pageNoStr;
        private int allPage;

        public YamYamBusanViewModel(IDialogCoordinator _dc)
        {
            dc = _dc;
            allPage = (435+NumOfRow) / NumOfRow;
            PageNoStr = "현재 페이지 번호 : " + PageNo.ToString() + " / " + allPage.ToString();
            GetDataFromOpenApi();
        }

        private async void GetDataFromOpenApi()
        {
            string baseUri = "http://apis.data.go.kr/6260000/FoodService/getFoodKr";
            string apikey = ConfigLoader.LoadApiKey();
            StringBuilder strUri = new StringBuilder();
            strUri.Append($"serviceKey={apikey}&");
            strUri.Append($"pageNo={1}&");
            strUri.Append($"numOfRows={435}&");
            strUri.Append($"resultType=json&");
            string totalOpenApi = $"{baseUri}?{strUri}";
            Common.LOGGER.Info(totalOpenApi);

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(totalOpenApi);
                Items = new ObservableCollection<PlaceData>();
                BackItems = new ObservableCollection<BackData>();
                var result = JsonConvert.DeserializeObject<FoodResponse>(json);

                if (result?.getFoodKr?.item != null)
                {
                    _allItems = result.getFoodKr.item;
                    BackItems = result.getFoodKr.bitem;
                    TotalCount = Convert.ToInt32(result.getFoodKr.totalCount);
                    ApplyPage();
                }
            }
            catch (Exception ex)
            {
                await dc.ShowMessageAsync(this, "API 오류", ex.Message);
                Common.LOGGER.Error(ex.ToString());
            }

        }
        private void ApplyPage()
        {
            var pageItems = _allItems
                .Skip((PageNo - 1) * NumOfRow)
                .Take(NumOfRow)
                .ToList();
            PageNoStr = "현재 페이지 번호 : " + PageNo.ToString() + " / " + allPage.ToString();
            Items = new ObservableCollection<PlaceData>(pageItems);
        }

        partial void OnPageNoChanged(int value)
        {
            ApplyPage();
        }

        public class FoodResponse
        {
            public FoodResult getFoodKr { get; set; }
        }

        public class FoodResult
        {
            public List<PlaceData> item { get; set; }
            public ObservableCollection<BackData> bitem { get; set; }

            public int numOfRows { get; set; }
            public int pageNo { get; set; }
            public int totalCount { get; set; }
        }

    }
}
