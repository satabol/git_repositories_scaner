using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace git_repositories_scanner
{
    public class MenuItemData
    {
        public static int menuitem_header_length = 30;
        private static int _i = 0;
        public static int i
        {
            get
            {
                _i++;
                return _i;
            }
        }

        public DateTime date_time = DateTime.Now; // Дата/время регистрации события.
        public Int32 index;
        public MenuItem mi;

        public string repository_path = null;

        public MenuItemData(string _path)
        {
            this.index = MenuItemData.i; // index
            this.repository_path = _path;
            createMenuItem();
        }

        // Проверить, а есть ли директорий с этим репозиторием. Если нет, то сделать меню недоступным (серым цветом):
        public void CheckPath()
        {
            bool bool_object_exists = false;
            // Если объект существует, то активировать его меню:
            if (Directory.Exists(repository_path))
            {
                bool_object_exists = true;
            }

            {
                Grid grid = (Grid)mi.Template.FindName("mi_grid", mi);
                if (grid != null)
                {
                    foreach (Object _obj in grid.Children)
                    {
                        if (_obj is MenuItem)
                        {
                            MenuItem mi_tmp = (MenuItem)_obj;
                            if (mi_tmp != null)
                            {
                                switch (mi_tmp.Name)
                                {
                                    case "mi_main":
                                    case "mi_commit":
                                        //mi_tmp.Background = System.Windows.Media.Brushes.DarkGray;
                                        mi_tmp.IsEnabled = bool_object_exists;
                                        break;
                                    case "mi_url":
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void createMenuItem()
        {
            mi = new MenuItem()
            {
                Name = "mi_main"
            };
            mi.Header = ShortText(repository_path);

            bool bool_object_exists = false;
            // Если объект существует, то активировать его меню:
            if (Directory.Exists(repository_path))
            {
                bool_object_exists = true;
            }

            mi.Command = App.CustomRoutedCommand_GotoFolderExplorer;
            mi.CommandParameter = repository_path;

            if (bool_object_exists == true)
            {
                try
                {
                    Repository repo = new Repository(repository_path);
                    RepositoryStatus repositoryStatus = repo.RetrieveStatus();
                    bool isDirty = repositoryStatus.IsDirty;

                    if (isDirty == false)
                    {
                        mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("pack://application:,,,/icons/NormalIcon.ico", UriKind.Absolute)) };
                    }
                    else
                    {
                        mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("pack://application:,,,/icons/ModifiedIcon.ico", UriKind.Absolute)) };
                    }

                    mi.ToolTip = "open path";

                    {
                        // Так определять Grid гораздо проще: http://stackoverflow.com/questions/5755455/how-to-set-control-template-in-code
                        string str_template = @"
                            <ControlTemplate 
                                                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                                xmlns:tb='http://www.hardcodet.net/taskbar'
                                                xmlns:local='clr-namespace:FileChangesWatcher'
                             >
                                <Grid x:Name='mi_grid'>
                                    <Grid.ColumnDefinitions>
                                        <ColumnDefinition Width='*'/>
                                        <ColumnDefinition Width='20'/>
                                        <ColumnDefinition Width='20'/>
                                    </Grid.ColumnDefinitions>
                                </Grid>
                            </ControlTemplate>
                        ";
                        MenuItem _mi = new MenuItem();
                        Grid mi_grid = null; // new Grid();
                        ControlTemplate ct = (ControlTemplate)XamlReader.Parse(str_template);
                        _mi.Template = ct;
                        if (_mi.ApplyTemplate())
                        {
                            mi_grid = (Grid)ct.FindName("mi_grid", _mi);
                        }
                        //*
                        BranchCollection bc = repo.Branches;
                        string url_path = null;
                        string url_name = null;
                        foreach (Remote b in repo.Network.Remotes)
                        {
                            url_name = b.Name;
                            url_path = b.Url;
                            break; // Использовать только первую ссылку. Остальные пока не знаю как лучше выложить.
                        }
                        if (url_name != null)
                        {
                            MenuItem mi_url = new MenuItem()
                            {
                                Name = "mi_url"
                            };
                            mi_url.Icon = new System.Windows.Controls.Image
                            {
                                Source = new BitmapImage(
                                new Uri("pack://application:,,,/icons/url_external.ico"))
                            };
                            mi_url.ToolTip = "Goto "+url_name+": "+url_path;
                            //mi_url.Command = App.CustomRoutedCommand_CopyTextToClipboard;
                            //mi_url.CommandParameter = _e.FullPath;
                            mi_url.Click += (sender, args) =>
                            {
                                System.Diagnostics.Process.Start(url_path);
                            };
                            Grid.SetColumn(mi_url, 1);
                            Grid.SetRow(mi_url, 0);
                            mi_grid.Children.Add(mi_url);
                        }
                        //*/

                        //*
                        MenuItem mi_commit = new MenuItem()
                        {
                            Name = "mi_commit"
                        };
                        mi_commit.Icon = new System.Windows.Controls.Image
                        {
                            Source = new BitmapImage(
                            new Uri("pack://application:,,,/icons/commit.ico"))
                        };
                        mi_commit.ToolTip = "Execute TortoiseGitProc.exe /command:commit";
                        mi_commit.Command = App.CustomRoutedCommand_TortoiseGitProc;
                        mi_commit.CommandParameter = this.repository_path;

                        Grid.SetColumn(mi_commit, 2);
                        Grid.SetRow(mi_commit, 0);
                        mi_grid.Children.Add(mi_commit);
                        //*/

                        Grid.SetColumn(mi, 0);
                        Grid.SetRow(mi, 0);
                        mi_grid.Children.Add(mi);
                        mi = _mi;
                    }
                }
                catch( Exception _ex)
                {
                    mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("pack://application:,,,/icons/delete_icon.ico", UriKind.Absolute)) };
                    mi.ToolTip = "Exception: "+_ex.Message;
                }
            }
            else
            {
                mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("pack://application:,,,/icons/delete_icon.ico", UriKind.Absolute)) };
                mi.ToolTip = "Path do not exist.";
            }
            return;
        }

        // Укоротить строку, если она превышает максимальное количество символов и вставить в середину многоточие.
        public static string ShortText(string text)
        {
            string result = text.Length > (menuitem_header_length * 2 + 5) ? text.Substring(0, menuitem_header_length) + " ... " + text.Substring(text.Length - menuitem_header_length) : text;
            return result;
        }
    }
}
