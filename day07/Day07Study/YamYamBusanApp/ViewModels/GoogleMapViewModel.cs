using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamYamBusanApp.ViewModels
{
    public partial class GoogleMapViewModel : ObservableObject
    {
        IDialogCoordinator dc;

        public GoogleMapViewModel(IDialogCoordinator _dc)
        {
            dc = _dc;
        }
    }
}
