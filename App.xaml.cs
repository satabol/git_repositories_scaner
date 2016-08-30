using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace git_repositories_watcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static TaskbarIcon _notifyIcon = null;
        public static TaskbarIcon NotifyIcon
        {
            get
            {
                return _notifyIcon;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
            String settingsFilePath = getSettingsFilePath();

            string settings_params = null;
            if (File.Exists(settingsFilePath) == false)
            {
                settings_params = @"{ 
    paths : []
}";
                File.WriteAllText(settingsFilePath, settings_params);
            }
            settings_params = File.ReadAllText(settingsFilePath);
            Settings settings = JsonConvert.DeserializeObject<Settings>(settings_params);

            Console.WriteLine("Paths.Count=" + settings.Paths.Count);
        }

        // Определить имя файла конфигурации:
        public static String getSettingsFilePath()
        {
            String iniFilePath = null;
            string exe_file = typeof(git_repositories_watcher.App).Assembly.Location; // http://stackoverflow.com/questions/4764680/how-to-get-the-location-of-the-dll-currently-executing
            iniFilePath = System.IO.Path.GetDirectoryName(exe_file) + "\\" + System.IO.Path.GetFileNameWithoutExtension(exe_file) + ".json";
            return iniFilePath;
        }

    }
}
