using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using firebasesample.Models;
using firebasesample.Services.FirebaseAuth;
using firebasesample.Services.FirebaseDB;
using firebasesample.ViewModels.Base;
using firebasesample.ViewModels.Login;
using Xamarin.Forms;

namespace firebasesample.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        private ICommand _logoutCommand;
        private ICommand _saveTextCommand;
        private ICommand _deleteCommand;
        private IFirebaseAuthService _firebaseAuthService;
        private IFirebaseDBService _firebaseDatabaseService;
        public MainViewModel()
        {
            _firebaseAuthService = DependencyService.Get<IFirebaseAuthService>();
            _firebaseDatabaseService = DependencyService.Get<IFirebaseDBService>();
            _firebaseDatabaseService.Connect();
            _firebaseDatabaseService.GetMessage();
            MessagingCenter.Subscribe<String, ObservableCollection<Homework>>(this, _firebaseDatabaseService.GetMessageKey(), (sender, args) =>
            {
                //Message = (args);
                List = (args);
            });
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand = _logoutCommand ?? new DelegateCommandAsync(LogoutCommandExecute); }
        }

        private async Task LogoutCommandExecute()
        {
            if (await _firebaseAuthService.Logout())
            {
                await NavigationService.NavigateToAsync<LoginViewModel>();
            }


        }

        public ICommand SaveTextCommand
        {
            get { return _saveTextCommand = _saveTextCommand ?? new DelegateCommandAsync(SaveTextCommandExecute); }
        }
        private async Task SaveTextCommandExecute()
        {
            _firebaseDatabaseService.SetMessage(Message);
            _firebaseDatabaseService.GetMessage();
        }


        public ICommand DeleteCommand
        {
            get { return _deleteCommand = _deleteCommand ?? new DelegateCommandAsync<string>(DeleteCommandExecute); }
        }
        private async Task DeleteCommandExecute(string key)
        {
            _firebaseDatabaseService.DeleteItem(key);
            _firebaseDatabaseService.GetMessage();
        }

        private String _message;
        public String Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Homework> _list;
        public ObservableCollection<Homework> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                RaisePropertyChanged();
            }
        }
    }
}
