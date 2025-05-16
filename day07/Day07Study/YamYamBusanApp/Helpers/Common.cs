using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamYamBusanApp.Helpers
{
    public class Common
    {
        // NLog 인스턴스
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        // MahApps.Metro 다이얼로그
        public static IDialogCoordinator DIALOGCOORDINATOR;
    }
}
