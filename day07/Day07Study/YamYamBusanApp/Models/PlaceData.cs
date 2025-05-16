using CommunityToolkit.Mvvm.ComponentModel;

namespace YamYamBusanApp.Models
{
    public partial class PlaceData : ObservableObject
    {
        [ObservableProperty] private int _uc_Seq;
        [ObservableProperty] private string _place;
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _addr1;
        [ObservableProperty] private string _itemcntnts;
        [ObservableProperty] private double _lat;
        [ObservableProperty] private double _lng;
    }
}
