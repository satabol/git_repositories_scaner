using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace git_repositories_watcher
{
    class NotifyIconViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ICommand ReloadRepositoriesCommand
        {
            get
            {
                //return new DelegateCommand { CommandAction = () => { Application.Current.Shutdown(); } };
                return new DelegateCommand { CommandAction = () => { App.reloadContextMenu(); } };
            }
        }

        public ICommand GotoSettingsFileCommand
        {
            get
            {
                //return new DelegateCommand { CommandAction = () => { Application.Current.Shutdown(); } };
                string settings_file_path = App.getSettingsFilePath();
                return new DelegateCommand { CommandAction = () => { App.gotoPathByWindowsExplorer(settings_file_path); } };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                //return new DelegateCommand { CommandAction = () => { Application.Current.Shutdown(); } };
                return new DelegateCommand { CommandAction = () => { App.Current.Shutdown(0); } };
            }
        }
    }
    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }    
}
