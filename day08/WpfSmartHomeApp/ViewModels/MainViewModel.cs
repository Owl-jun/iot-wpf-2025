using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel() { 
        
        }

        [ObservableProperty] private double _homeTemp;
        [ObservableProperty] private double _homeHumid;

        [ObservableProperty] private string _detectResult;
        [ObservableProperty] private string _detectRain;
        [ObservableProperty] private string _detectAirCon;
        [ObservableProperty] private string _detectLight;
        [ObservableProperty] private bool _isDetectHuman;
        [ObservableProperty] private bool _isDetectRain;
        [ObservableProperty] private bool _isDetectAirCon;
        [ObservableProperty] private bool _isDetectLight;
        
        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 27;
            HomeHumid = 64;
            DetectResult = "Detected Human";
            DetectRain = "Detected Rain";
            DetectAirCon = "Detected AirCon";
            DetectLight = "Detected Light";
            IsDetectHuman = true;
            IsDetectRain = true;
            IsDetectAirCon = true;
            IsDetectLight = true;
        }
    }
}
