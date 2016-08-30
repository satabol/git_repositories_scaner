using Hardcodet.Wpf.TaskbarNotification;
using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

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

        // пути репозиториев:
        public static List<MenuItemData> list_context_menu_items = new List<MenuItemData>();
        public static Settings settings = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");

            _notifyIcon.ContextMenu.CommandBindings.Add(new CommandBinding(CustomRoutedCommand_ExecuteFile, ExecutedCustomCommand_ExecuteFile, CanExecuteCustomCommand));

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
            settings = JsonConvert.DeserializeObject<Settings>(settings_params);

            Console.WriteLine("Paths.Count=" + settings.Paths.Count);
            reloadContextMenu();
        }

        public static void reloadContextMenu()
        {
            foreach(MenuItemData item in list_context_menu_items)
            {
                _notifyIcon.ContextMenu.Items.Remove(item.mi);
            }
            list_context_menu_items.Clear();

            foreach (string path in settings.Paths)
            {
                Repository repo = new Repository(path);
                RepositoryStatus repositoryStatus = repo.RetrieveStatus();
                Console.WriteLine(path + ": " + repositoryStatus.IsDirty);
                MenuItemData menuItemData = new MenuItemData(path);
                list_context_menu_items.Add(menuItemData);
                _notifyIcon.ContextMenu.Items.Insert(0, menuItemData.mi);
            }

        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Control target = e.Source as Control;
            e.CanExecute = true;
        }

        // Пользовательская команда:
        public static RoutedCommand CustomRoutedCommand_ExecuteFile = new RoutedCommand();
        private void ExecutedCustomCommand_ExecuteFile(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            //MessageBox.Show("Custom Command Executed: "+ e.Parameter);
            Path_ObjectType obj = (Path_ObjectType)e.Parameter;
            Process.Start(obj.path, "");
            //*/
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
