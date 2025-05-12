﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfBookRentalShop01.Views;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private string _greeting;
        public string Greeting { 
            get => _greeting;             
            set => SetProperty(ref _greeting, value); 
        }

        public MainViewModel()
        {
            Greeting = "hehehe!!";
        }

        private UserControl _currentview;
        private string _currentStatus;
        public UserControl Currentview { 
            get => _currentview; 
            set => SetProperty(ref _currentview, value); 
        }
        public string CurrentStatus { 
            get => _currentStatus; 
            set => SetProperty(ref _currentStatus, value); 
        }


        #region 화면 기능(이벤트처리)

        [RelayCommand]
        public void AppExit()
        {
            MessageBox.Show("ㅇㅇ", "ㄴㄴ");
        }

        [RelayCommand]
        public void ShowBookGenre()
        {
            var viewModel = new BookGenreViewModel();
            var view = new BookGenreView();
            view.DataContext = viewModel;
            CurrentStatus = "책장르 관리 화면 입니다.";
            Currentview = view;
        }

        [RelayCommand]
        public void ShowBooks()
        {
            var viewModel = new BooksViewModel();
            var view = new BooksView();
            view.DataContext = viewModel;
            CurrentStatus = "책 관리 화면 입니다.";
            Currentview = view;
        }
        #endregion
    }
}
