using Hardcodet.Wpf.TaskbarNotification;
using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace git_repositories_scaner
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
            reloadContextMenu();


            _notifyIcon.ContextMenu.CommandBindings.Add(new CommandBinding(CustomRoutedCommand_ExecuteFile, ExecutedCustomRoutedCommand_ExecuteFile, CanExecuteCustomCommand));
            _notifyIcon.ContextMenu.CommandBindings.Add(new CommandBinding(CustomRoutedCommand_TortoiseGitProc, ExecutedCustomRoutedCommand_TortoiseGitProc, CanExecuteCustomCommand));
            _notifyIcon.ContextMenu.CommandBindings.Add(new CommandBinding(CustomRoutedCommand_GotoFolderExplorer, CustomRoutedCommand_ExecuteGotoFolderExplorer, CanExecuteCustomCommand));
        }

        // Перезагрузка контекстного меню вместе с настройками программы:
        public static void reloadContextMenu()
        {
            String settingsFilePath = getSettingsFilePath();

            string settings_params = null;
            if (File.Exists(settingsFilePath) == false)
            {
                settings_params = @"{ 
    paths : [],
    version : '0.0'
}";
                File.WriteAllText(settingsFilePath, settings_params);
            }
            settings_params = File.ReadAllText(settingsFilePath);
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(settings_params);
            }
            catch ( Exception _ex)
            {
                MessageBox.Show("Ошибка чтения файла ini:\n" + _ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (MenuItemData item in list_context_menu_items)
            {
                _notifyIcon.ContextMenu.Items.Remove(item.mi);
            }
            list_context_menu_items.Clear();

            foreach (string path in settings.Paths.Reverse() )
            {
                MenuItemData menuItemData = new MenuItemData(path);
                list_context_menu_items.Add(menuItemData);
                _notifyIcon.ContextMenu.Items.Insert(0, menuItemData.mi);
            }

            string init_text_message = "Repositories reloaded. Open context menu.";
            TrayPopupMessage popup = new TrayPopupMessage("\n" + init_text_message.ToString(), "Initial initialization", App.NotifyIcon, TrayPopupMessage.ControlButtons.Close);
            popup.MouseDown += (sender, args) =>
            {
                App.NotifyIcon.CustomBalloon.IsOpen = false;
            };
            App.NotifyIcon.ShowCustomBalloon(popup, PopupAnimation.Fade, 4000);
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Control target = e.Source as Control;
            e.CanExecute = true;
        }

        // Пользовательская команда:
        public static RoutedCommand CustomRoutedCommand_TortoiseGitProc = new RoutedCommand();
        private void ExecutedCustomRoutedCommand_TortoiseGitProc(object sender, ExecutedRoutedEventArgs e)
        {
            //*
            string repository_path = (string)e.Parameter;
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = repository_path;
            startInfo.FileName = "TortoiseGitProc.exe";
            startInfo.Arguments = "/command:commit";
            Process.Start(startInfo); //, "/command:commit");
            //*/
        }

        public static RoutedCommand CustomRoutedCommand_ExecuteFile = new RoutedCommand();
        private void ExecutedCustomRoutedCommand_ExecuteFile(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            string repository_path = (string)e.Parameter;
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = repository_path;
            startInfo.FileName = "TortoiseGitProc.exe";
            startInfo.Arguments = "/command:commit";
            Process.Start(startInfo); //, "/command:commit");
            //*/
        }

        public static RoutedCommand CustomRoutedCommand_GotoFolderExplorer = new RoutedCommand();
        private void CustomRoutedCommand_ExecuteGotoFolderExplorer(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Custom Command Executed: "+ e.Parameter);
            string folder_path = (string)e.Parameter;
            gotoPathByWindowsExplorer(folder_path);
        }

        public static void gotoPathByWindowsExplorer(string _path)
        {
            if (Directory.Exists( _path )==true )
            {
                Process.Start("explorer.exe", "\"" + _path + "\"");
            }
            else if (File.Exists(_path) == true)
            {
                Process.Start("explorer.exe", "/select,\"" + _path + "\"");
            }
            else
            {
                Process.Start("explorer.exe");
            }

        }

        // Определить имя файла конфигурации:
        public static String getSettingsFilePath()
        {
            String iniFilePath = null;
            string exe_file = typeof(git_repositories_scaner.App).Assembly.Location; // http://stackoverflow.com/questions/4764680/how-to-get-the-location-of-the-dll-currently-executing
            iniFilePath = System.IO.Path.GetDirectoryName(exe_file) + "\\" + System.IO.Path.GetFileNameWithoutExtension(exe_file) + ".json";
            return iniFilePath;
        }

    }
}
