using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder2025.Helpers
{
    internal class Common
    {
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public static readonly string CONNSTR = "Server=localhost;Database=moviefinder;Uid=root;Pwd=12345;Charset=utf8;";

        public static IDialogCoordinator DIALOGCOORDINATOR;
    }
}
